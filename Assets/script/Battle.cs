using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum Game_state{Prepare,Battle,Finish};
public enum Player_state{Draw_card,Play_card,Play_role,End};

public class Battle : MonoBehaviour {
	static public float frame_height = 0.03f;
	static public float mask_height = 0.02f;
	static public int max_card = 6;
	static public int init_card_num = 5;

	static public GameObject[,] location_roles;
	static public Map map;//紀錄地板種類
	static public Game_state game_stat;
	static public Player_state player_stat;

	public GameObject[] player_roles;
	public GameObject[] ai_roles;
	public GameObject blue_frame_pref;
	public GameObject red_frame_pref;
	public GameObject blue_mask_pref;
	public GameObject red_mask_pref;
	public GameObject skill_mask_pref;
	public List<GameObject> blue_mask = new List<GameObject> ();
	public List<GameObject> red_frame = new List<GameObject> ();
	public List<GameObject> skill_mask = new List<GameObject> ();
	public GameObject finish;
	public GameObject[] card_pref;
	public GameObject[] cards;
	int card_pref_counter=0;

	private GameObject target;
	private GameObject[,] blue_frames;
	private GameObject red_mask;
	bool[,] attack_range;
	bool[,] skill_range;
	bool has_choose;
	bool isPlayer_turn;
	int role_turn;
	bool moved;
	bool attacked;
	bool start;
	bool skill_prepared;
	Button sk ;


	void Awake(){
		map = new Map ();
	}

	// Use this for initialization
	void Start () {
		cards = new GameObject[max_card];
		sk = GameObject.Find ("Button_skill").GetComponent<Button> ();
		attack_range = new bool[map.lenth, map.lenth];
		skill_range = new bool[map.lenth, map.lenth];
		for (int i=0; i < map.lenth; i++) {
			for (int j = 0; j < map.lenth; j++) {
				attack_range [i, j] = false;
			}
		}
		game_stat = Game_state.Prepare;
		//初始化玩家角色範圍提示
		has_choose = false;
		blue_frames = new GameObject[3, 3];
		for(int i=0;i<3;i++){
			for (int j = 0; j < 3; j++) {
				Vector3 pos = new Vector3 (i, frame_height, j);
				Quaternion rot = new Quaternion (0, 0, 0, 0);
				blue_frames[i,j] = (GameObject)Instantiate (blue_frame_pref, pos, rot);
			}
		}
		Vector3 pos1 = new Vector3 (0, mask_height, 0);
		Quaternion rot1 = new Quaternion (0, 0, 0, 0);
		red_mask= (GameObject)Instantiate (red_mask_pref, pos1, rot1);
		red_mask.SetActive (false);

		//初始化角色站位
		location_roles = new GameObject[map.lenth, map.lenth];
		for(int i=0;i<map.lenth;i++){
			for (int j = 0; j < map.lenth; j++) {
				location_roles [i, j] = null;
			}
		}
		for (int i = 0; i < player_roles.Length; i++) {
			GameObject role = player_roles [i];
			Role role_info = role.GetComponent<Role> ();
			role_info.changePos (i % 3, i / 3);
			location_roles[role_info.pos_x,role_info.pos_y]=role;
		}
		for (int i = 0; i < ai_roles.Length; i++) {
			GameObject role = ai_roles [i];
			Role role_info = role.GetComponent<Role> ();
			location_roles[role_info.pos_x,role_info.pos_y]=role;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (game_stat == Game_state.Prepare) {
			//改站位
			if (Input.GetMouseButtonDown (0)) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit = new RaycastHit ();
				if (Physics.Raycast (ray, out hit)) {
					GameObject choosed = hit.collider.gameObject;
					print ("choose" + choosed);
					if (!has_choose) {
						if (choosed.tag == "Player") { // 檢查物件tag是否為role 不然可能會點到平面
							target = choosed;
							Role choosed_role=choosed.GetComponent<Role> ();
							int pos_x = choosed_role.pos_x;
							int pos_y = choosed_role.pos_y;
							Vector3 pos = new Vector3 (pos_x, mask_height, pos_y);
							red_mask.transform.position = pos;
							red_mask.SetActive (true);
							has_choose = true;
						}
					} else {
						if (choosed.tag == "Player") { // 檢查物件tag是否為role 不然可能會點到平面
							Role target_role = target.GetComponent<Role> ();
							Role choosed_role=choosed.GetComponent<Role> ();
							int pos_x = choosed_role.pos_x;
							int pos_y = choosed_role.pos_y;
							Vector3 role_pos = new Vector3 (pos_x, Role.role_height, pos_y);
							choosed_role.onMove (target_role.pos_x, target_role.pos_y);
							target_role.onMove (pos_x,pos_y);
						} else if (choosed.tag == "range_hint") {
							int pos_x = (int)choosed.transform.position.x;
							int pos_y = (int)choosed.transform.position.z;
							Role target_role = target.GetComponent<Role> ();
							target_role.onMove (pos_x,pos_y);
						} 
						red_mask.SetActive (false);
						target = null;
						has_choose = false;

					}
				}else if(has_choose){
					red_mask.SetActive (false);
					target = null;
					has_choose = false;
				}
			} 
		}

		if (game_stat == Game_state.Battle) {
			
			if (isPlayer_turn) {
				//玩家回合
				if(player_stat==Player_state.Draw_card){
					draw_card ();
					//draw card
					player_stat = Player_state.Play_card;
				}else if (player_stat==Player_state.Play_card) {
					finish.SetActive (true);
				} else{
						
					if (player_roles [role_turn] == null) {
						role_turn++;
						if (role_turn == player_roles.Length) {
							role_turn = 0;
							sk.interactable = false;
							update_coldown ();
							isPlayer_turn = false;
						}
					} else {
						GameObject cur_role = player_roles [role_turn];
						Role role = cur_role.GetComponent<Role> ();
						if (start) {
							print ("start:" + role_turn);
							create_move_range (cur_role);
							create_attack_range (cur_role);
							create_skill_range (cur_role);
							enable_skill_range (false);
							start = false;
							moved = false;
							attacked = false;
						} else {
							if (!moved && !(role.moving || role.attacking || role.casting) && !skill_prepared) {
								if (!skill_prepared && !CardInfo.using_card)
									enable_move_range (true);
								if (Input.GetMouseButtonDown (0)) {
									Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
									RaycastHit hit = new RaycastHit ();
									if (Physics.Raycast (ray, out hit)) {
										GameObject choose = hit.collider.gameObject;
										print ("choose" + choose);
										if (choose.tag == "range_hint") {
											print ("move");
											role.onMove ((int)choose.transform.position.x, (int)choose.transform.position.z);
											moved = true;
											destroy_move_range ();
											destroy_attack_range ();
											create_attack_range (cur_role);
											enable_attack_range (false);
											destroy_skill_range ();
											create_skill_range (cur_role);
											enable_skill_range (false);
										}
									}
								}
							}
							if (!attacked && !(role.moving || role.attacking || role.casting)) {
								if (!skill_prepared && !CardInfo.using_card)
									enable_attack_range (true);
								if (Input.GetMouseButtonDown (0)) {
									Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
									RaycastHit hit = new RaycastHit ();
									if (Physics.Raycast (ray, out hit)) {
										GameObject choose = hit.collider.gameObject;
										print ("choose" + choose);
										if (skill_prepared && skill_range [(int)choose.transform.position.x, (int)choose.transform.position.z]) {
											if (role.skill_info.target == Target.Single && choose.tag == "enemy") {
												print ("use skill");
												attacked = true;
												role.onSkill (choose.GetComponent<Role> ());
												destroy_skill_range ();
												destroy_attack_range ();
												foreach (GameObject ob in blue_mask) {
													ob.SetActive (true);
												}
												skill_prepared = false;
											} else if (role.skill_info.target == Target.AOE) {
												print ("use skill");
												attacked = true;
												role.onSkill ();
												destroy_skill_range ();
												destroy_attack_range ();
												foreach (GameObject ob in blue_mask) {
													ob.SetActive (true);
												}
												skill_prepared = false;
											}
										} else if (choose.tag == "enemy" && attack_range [(int)choose.transform.position.x, (int)choose.transform.position.z]) {
											print ("attack");
											attacked = true;
											role.onAttack (choose.GetComponent<Role> ());
											destroy_attack_range ();
										}
									}
								}

							}
							/*skill button active*/
							if (role.canUseSkill () && !attacked) {
								sk.interactable = true;
							} else {
								sk.interactable = false;
							}

							/* pass function*/
							if ((!moved || !attacked) && !(role.moving || role.attacking || role.casting) && !skill_prepared && !CardInfo.using_card) {
								cur_role.GetComponent<role_UI> ().Pass_text.gameObject.SetActive (true);
							} else {
								cur_role.GetComponent<role_UI> ().Pass_text.gameObject.SetActive (false);
							}


							if (moved && attacked && !(role.moving || role.attacking || role.casting)) {
								role_turn++;
								start = true;
								destroy_move_range ();
								destroy_attack_range ();
								destroy_skill_range ();
								if (role_turn == player_roles.Length) {
									role_turn = 0;
									sk.interactable = false;
									update_coldown ();
									isPlayer_turn = false;
									player_stat = Player_state.Draw_card;
								}
							}
						}
					}
				}
			} else {
				//AI
				if (ai_roles [role_turn] == null) {
					role_turn++;
					if (role_turn == ai_roles.Length) {
						role_turn = 0;
						update_coldown ();
						isPlayer_turn = true;
					}
				} else {
					AI ai=ai_roles[role_turn].GetComponent<AI>();
					Role ai_role=ai_roles[role_turn].GetComponent<Role>();
					if (start) {
						print("AI turn:"+role_turn);
						start=false;
						ai.acted = false;
					} else {
						if (!ai.acted) {
							ai.aiAction ();
							ai.acted = true;
						} else {
							if (!(ai_role.moving || ai_role.attacking || ai_role.casting)) {
								role_turn++;
								start = true;
								if (role_turn == ai_roles.Length){
									role_turn = 0;
									update_coldown ();
									isPlayer_turn = true;
									player_stat = Player_state.Draw_card;
								}
							}
						}
					}
				}
			}
			//win or lose
			if (isWin ()) {
				game_stat = Game_state.Finish;
				GameObject panel=GameObject.Find ("defeat");
				panel.SetActive (false);
				print ("win");
			}
			if (isLose ()) {
				game_stat = Game_state.Finish;
				GameObject panel=GameObject.Find ("victory");
				panel.SetActive (false);
				print ("lose");
			}
		}

		if (game_stat == Game_state.Finish) {
			GameObject end=GameObject.Find ("end");
			end.GetComponent<Canvas>().enabled=true;

			//finish
		}
	}
		

	bool isWin(){
		for (int i=0; i < ai_roles.Length; i++) {
			if (ai_roles [i] != null)
				return false;
		}
		return true;
	}

	bool isLose(){
		for (int i=0; i < player_roles.Length; i++) {
			if (player_roles [i] != null)
				return false;
		}
		return true;
	}

	void draw_card(){
		int card_num=0;
		int empty_index=0;
		for (int i = 0; i < cards.Length; i++) {
			if (cards [i] != null)
				card_num++;
			else
				empty_index=i;
		}
		if (card_num < max_card) {
			Vector3 pos = new Vector3 (CardInfo.card_x_offset+CardInfo.card_x_rate*empty_index, CardInfo.card_y,CardInfo.card_z);
			Quaternion rot =Quaternion.identity;
			cards[empty_index]=(GameObject)Instantiate (card_pref[card_pref_counter], pos, rot);
			card_pref_counter = (card_pref_counter + 1) % card_pref.Length;
		}
	}

	void update_coldown(){
		for (int i = 0; i < player_roles.Length; i++) {
			if (player_roles [i] != null) {
				Role role = player_roles [i].GetComponent<Role> ();
				if (role.battle_cd > 0)
					role.battle_cd--;
			}
		}
		for (int i = 0; i < ai_roles.Length; i++) {
			if(ai_roles [i]!=null){
				Role role = ai_roles [i].GetComponent<Role> ();
				if (role.battle_cd > 0)
					role.battle_cd--;
			}
		}
	}

	void create_move_range(GameObject role){
		print ("create Move range:" + role);
		Role info = role.GetComponent<Role> ();
		int role_posx = info.pos_x;
		int role_posy = info.pos_y;
		int range = info.battle_move_range;
		Vector3 pos = new Vector3 (role_posx, mask_height, role_posy);
		Quaternion rot = new Quaternion (0, 0, 0, 0);
		blue_mask.Add((GameObject)Instantiate (blue_mask_pref, pos, rot));
		for (int i = 1; i <= range; i++) {
			Vector3 pos1 = new Vector3 (role_posx+i, mask_height, role_posy);
			Vector3 pos2 = new Vector3 (role_posx-i, mask_height, role_posy);
			Vector3 pos3 = new Vector3 (role_posx, mask_height, role_posy+i);
			Vector3 pos4 = new Vector3 (role_posx, mask_height, role_posy-i);
			if (isOnMap (pos1)) {
				blue_mask.Add((GameObject)Instantiate (blue_mask_pref, pos1, rot));
			}
			if (isOnMap (pos2)) {
				blue_mask.Add((GameObject)Instantiate (blue_mask_pref, pos2, rot));
			}
			if (isOnMap (pos3)) {
				blue_mask.Add((GameObject)Instantiate (blue_mask_pref, pos3, rot));
			}
			if (isOnMap (pos4)) {
				blue_mask.Add((GameObject)Instantiate (blue_mask_pref, pos4, rot));
			}
			for (int j = 1; j <= range - i; j++) {
				Vector3 pos5 = new Vector3 (role_posx+i, mask_height, role_posy+j);
				Vector3 pos6 = new Vector3 (role_posx+i, mask_height, role_posy-j);
				Vector3 pos7 = new Vector3 (role_posx-i, mask_height, role_posy+j);
				Vector3 pos8 = new Vector3 (role_posx-i, mask_height, role_posy-j);
				if (isOnMap (pos5)) {
					blue_mask.Add((GameObject)Instantiate (blue_mask_pref, pos5, rot));
				}
				if (isOnMap (pos6)) {
					blue_mask.Add((GameObject)Instantiate (blue_mask_pref, pos6, rot));
				}
				if (isOnMap (pos7)) {
					blue_mask.Add((GameObject)Instantiate (blue_mask_pref, pos7, rot));
				}
				if (isOnMap (pos8)) {
					blue_mask.Add((GameObject)Instantiate (blue_mask_pref, pos8, rot));
				}
			}
		}
	}
	
	void destroy_move_range(){
		print ("destroy move range");
		foreach (GameObject ob in blue_mask) {
			Destroy (ob);
		}
		blue_mask.Clear ();
	}

	void enable_move_range(bool active){
		print ("enable_move_range:"+active);
		foreach (GameObject ob in blue_mask) {
			ob.SetActive (active);
		}
	}


	bool hasEnemyInAttackRange(GameObject role){
		Role info = role.GetComponent<Role> ();
		int role_posx = info.pos_x;
		int role_posy = info.pos_y;
		int range = info.battle_attack_range;
		Vector3 pos = new Vector3 (role_posx, frame_height, role_posy);
		for (int i = 1; i <= range; i++) {
			Vector3 pos1 = new Vector3 (role_posx+i, frame_height, role_posy);
			Vector3 pos2 = new Vector3 (role_posx-i, frame_height, role_posy);
			Vector3 pos3 = new Vector3 (role_posx, frame_height, role_posy+i);
			Vector3 pos4 = new Vector3 (role_posx, frame_height, role_posy-i);
			if (isOnMap (pos1) && location_roles[(int)pos1.x,(int)pos1.z]!=null && location_roles[(int)pos1.x,(int)pos1.z].tag=="enemy") {
				print ("pos1 true");
				return true;
			}
			if (isOnMap (pos2) && location_roles[(int)pos2.x,(int)pos2.z]!=null && location_roles[(int)pos2.x,(int)pos2.z].tag=="enemy") {
				print ("pos2 true");
				return true;
			}
			if (isOnMap (pos3) && location_roles[(int)pos3.x,(int)pos3.z]!=null && location_roles[(int)pos3.x,(int)pos3.z].tag=="enemy") {
				print ("pos3 true");
				return true;
			}
			if (isOnMap (pos4) && location_roles[(int)pos4.x,(int)pos4.z]!=null && location_roles[(int)pos4.x,(int)pos4.z].tag=="enemy") {
				print ("pos4 true");
				return true;
			}
			for (int j = 1; j <= range - i; j++) {
				Vector3 pos5 = new Vector3 (role_posx+i, frame_height, role_posy+j);
				Vector3 pos6 = new Vector3 (role_posx+i, frame_height, role_posy-j);
				Vector3 pos7 = new Vector3 (role_posx-i, frame_height, role_posy+j);
				Vector3 pos8 = new Vector3 (role_posx-i, frame_height, role_posy-j);
				if (isOnMap (pos5) && location_roles[(int)pos5.x,(int)pos5.z]!=null && location_roles[(int)pos5.x,(int)pos5.z].tag=="enemy") {
					print ("pos5 true");
					return true;
				}
				if (isOnMap (pos6) && location_roles[(int)pos6.x,(int)pos6.z]!=null && location_roles[(int)pos6.x,(int)pos6.z].tag=="enemy") {
					print ("pos6 true");
					return true;
				}
				if (isOnMap (pos7) && location_roles[(int)pos7.x,(int)pos7.z]!=null && location_roles[(int)pos7.x,(int)pos7.z].tag=="enemy") {
					print ("pos7 true");
					return true;
				}
				if (isOnMap (pos8) && location_roles[(int)pos8.x,(int)pos8.z]!=null && location_roles[(int)pos8.x,(int)pos8.z].tag=="enemy") {
					print ("pos8 true");
					return true;
				}
			}
		}
		return false;
	}

	void create_attack_range(GameObject role){
		
		Role info = role.GetComponent<Role> ();
		int role_posx = info.pos_x;
		int role_posy = info.pos_y;
		int range = info.battle_attack_range;
		Vector3 pos = new Vector3 (role_posx, frame_height, role_posy);
		Quaternion rot = new Quaternion (0, 0, 0, 0);
		red_frame.Add((GameObject)Instantiate (red_frame_pref, pos, rot));
		for (int i = 1; i <= range; i++) {
			Vector3 pos1 = new Vector3 (role_posx+i, frame_height, role_posy);
			Vector3 pos2 = new Vector3 (role_posx-i, frame_height, role_posy);
			Vector3 pos3 = new Vector3 (role_posx, frame_height, role_posy+i);
			Vector3 pos4 = new Vector3 (role_posx, frame_height, role_posy-i);
			if (isOnMap (pos1)) {
				red_frame.Add((GameObject)Instantiate (red_frame_pref, pos1, rot));
				attack_range[(int)pos1.x,(int)pos1.z]=true;
			}
			if (isOnMap (pos2)) {
				red_frame.Add((GameObject)Instantiate (red_frame_pref, pos2, rot));
				attack_range[(int)pos2.x,(int)pos2.z]=true;
			}
			if (isOnMap (pos3)) {
				red_frame.Add((GameObject)Instantiate (red_frame_pref, pos3, rot));
				attack_range[(int)pos3.x,(int)pos3.z]=true;
			}
			if (isOnMap (pos4)) {
				red_frame.Add((GameObject)Instantiate (red_frame_pref, pos4, rot));
				attack_range[(int)pos4.x,(int)pos4.z]=true;
			}
			for (int j = 1; j <= range - i; j++) {
				Vector3 pos5 = new Vector3 (role_posx+i, frame_height, role_posy+j);
				Vector3 pos6 = new Vector3 (role_posx+i, frame_height, role_posy-j);
				Vector3 pos7 = new Vector3 (role_posx-i, frame_height, role_posy+j);
				Vector3 pos8 = new Vector3 (role_posx-i, frame_height, role_posy-j);
				if (isOnMap (pos5)) {
					red_frame.Add((GameObject)Instantiate (red_frame_pref, pos5, rot));
					attack_range[(int)pos5.x,(int)pos5.z]=true;
				}
				if (isOnMap (pos6)) {
					red_frame.Add((GameObject)Instantiate (red_frame_pref, pos6, rot));
					attack_range[(int)pos6.x,(int)pos6.z]=true;
				}
				if (isOnMap (pos7)) {
					red_frame.Add((GameObject)Instantiate (red_frame_pref, pos7, rot));
					attack_range[(int)pos7.x,(int)pos7.z]=true;
				}
				if (isOnMap (pos8)) {
					red_frame.Add((GameObject)Instantiate (red_frame_pref, pos8, rot));
					attack_range[(int)pos8.x,(int)pos8.z]=true;
				}
			}
		}
		print ("create Attack range:" + red_frame.Count);
	}

	void destroy_attack_range(){
		print ("destroy attack range:"+red_frame.Count);
		foreach (GameObject ob in red_frame.ToArray()) {
			print ("destroy attack range:" + ob);
			Destroy (ob);
		}
		red_frame.Clear ();
		for (int i=0; i < map.lenth; i++) {
			for (int j = 0; j < map.lenth; j++) {
				attack_range [i, j] = false;
			}
		}
	}

	void enable_attack_range(bool active){
		print ("enable_attack_range:"+active);
		foreach (GameObject ob in red_frame) {
			ob.SetActive (active);
		}
	}
		
	static public bool isOnMap(Vector3 pos){
		if (pos.x < 0 || pos.x >= map.lenth)
			return false;
		if (pos.z < 0 || pos.z >= map.lenth)
			return false;
		return true;
	}

	void create_skill_range(GameObject role){

		Role info=role.GetComponent<Role>();
		SkillInfo sk_info = info.skill_info;
		int range = sk_info.basic_range;
		int role_posx = info.pos_x;
		int role_posy = info.pos_y;
		Vector3 pos = new Vector3 (role_posx, mask_height, role_posy);
		Quaternion rot = new Quaternion (0, 0, 0, 0);
		skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos, rot));
		skill_range[(int)pos.x,(int)pos.z]=true;
		if (sk_info.target == Target.Single) {
			for (int i = 1; i <= range; i++) {
				Vector3 pos1 = new Vector3 (role_posx+i,  mask_height, role_posy);
				Vector3 pos2 = new Vector3 (role_posx-i,  mask_height, role_posy);
				Vector3 pos3 = new Vector3 (role_posx,  mask_height, role_posy+i);
				Vector3 pos4 = new Vector3 (role_posx,  mask_height, role_posy-i);
				if (isOnMap (pos1)) {
					skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos1, rot));
					skill_range[(int)pos1.x,(int)pos1.z]=true;
				}
				if (isOnMap (pos2)) {
					skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos2, rot));
					skill_range[(int)pos2.x,(int)pos2.z]=true;
				}
				if (isOnMap (pos3)) {
					skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos3, rot));
					skill_range[(int)pos3.x,(int)pos3.z]=true;
				}
				if (isOnMap (pos4)) {
					skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos4, rot));
					skill_range[(int)pos4.x,(int)pos4.z]=true;
				}
				for (int j = 1; j <= range - i; j++) {
					Vector3 pos5 = new Vector3 (role_posx+i,  mask_height, role_posy+j);
					Vector3 pos6 = new Vector3 (role_posx+i,  mask_height, role_posy-j);
					Vector3 pos7 = new Vector3 (role_posx-i,  mask_height, role_posy+j);
					Vector3 pos8 = new Vector3 (role_posx-i,  mask_height, role_posy-j);
					if (isOnMap (pos5)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos5, rot));
						skill_range[(int)pos5.x,(int)pos5.z]=true;
					}
					if (isOnMap (pos6)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos6, rot));
						skill_range[(int)pos6.x,(int)pos6.z]=true;
					}
					if (isOnMap (pos7)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos7, rot));
						skill_range[(int)pos7.x,(int)pos7.z]=true;
					}
					if (isOnMap (pos8)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos8, rot));
						skill_range[(int)pos8.x,(int)pos8.z]=true;
					}
				}
			}
		} else if (sk_info.target == Target.AOE) {
			if (sk_info.shape == Shape.Diamond) {
				for (int i = 1; i <= range; i++) {
					Vector3 pos1 = new Vector3 (role_posx+i,  mask_height, role_posy);
					Vector3 pos2 = new Vector3 (role_posx-i,  mask_height, role_posy);
					Vector3 pos3 = new Vector3 (role_posx,  mask_height, role_posy+i);
					Vector3 pos4 = new Vector3 (role_posx,  mask_height, role_posy-i);
					if (isOnMap (pos1)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos1, rot));
						skill_range[(int)pos1.x,(int)pos1.z]=true;
					}
					if (isOnMap (pos2)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos2, rot));
						skill_range[(int)pos2.x,(int)pos2.z]=true;
					}
					if (isOnMap (pos3)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos3, rot));
						skill_range[(int)pos3.x,(int)pos3.z]=true;
					}
					if (isOnMap (pos4)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos4, rot));
						skill_range[(int)pos4.x,(int)pos4.z]=true;
					}
					for (int j = 1; j <= range - i; j++) {
						Vector3 pos5 = new Vector3 (role_posx+i,  mask_height, role_posy+j);
						Vector3 pos6 = new Vector3 (role_posx+i,  mask_height, role_posy-j);
						Vector3 pos7 = new Vector3 (role_posx-i,  mask_height, role_posy+j);
						Vector3 pos8 = new Vector3 (role_posx-i,  mask_height, role_posy-j);
						if (isOnMap (pos5)) {
							skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos5, rot));
							skill_range[(int)pos5.x,(int)pos5.z]=true;
						}
						if (isOnMap (pos6)) {
							skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos6, rot));
							skill_range[(int)pos6.x,(int)pos6.z]=true;
						}
						if (isOnMap (pos7)) {
							skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos7, rot));
							skill_range[(int)pos7.x,(int)pos7.z]=true;
						}
						if (isOnMap (pos8)) {
							skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos8, rot));
							skill_range[(int)pos8.x,(int)pos8.z]=true;
						}
					}
				}
			} else if (sk_info.shape == Shape.Square) {
				for (int i = role_posx-range; i <= role_posx+range; i++) {
					for (int j = role_posy-range; j <= role_posy+range; j++) {
						Vector3 pos1 = new Vector3 (i,  mask_height, j);
						if (isOnMap (pos1)) {
							skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos1, rot));
							skill_range[i,j]=true;
						}
					}
				}
			} else if (sk_info.shape == Shape.Cross) {
				for (int i = 1; i<= range; i++) {
					Vector3 pos1 = new Vector3 (role_posx+i,  mask_height, role_posy);
					Vector3 pos2 = new Vector3 (role_posx-i,  mask_height, role_posy);
					Vector3 pos3 = new Vector3 (role_posx,  mask_height, role_posy+i);
					Vector3 pos4 = new Vector3 (role_posx,  mask_height, role_posy-i);
					if (isOnMap (pos1)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos1, rot));
						skill_range[(int)pos1.x,(int)pos1.z]=true;
					}
					if (isOnMap (pos2)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos2, rot));
						skill_range[(int)pos2.x,(int)pos2.z]=true;
					}
					if (isOnMap (pos3)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos3, rot));
						skill_range[(int)pos3.x,(int)pos3.z]=true;
					}
					if (isOnMap (pos4)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos4, rot));
						skill_range[(int)pos4.x,(int)pos4.z]=true;
					}
				}
			} else if (sk_info.shape == Shape.Intersect) {
				for (int i = 1; i <= range ; i++) {
					Vector3 pos5 = new Vector3 (role_posx+i,  mask_height, role_posy+i);
					Vector3 pos6 = new Vector3 (role_posx+i,  mask_height, role_posy-i);
					Vector3 pos7 = new Vector3 (role_posx-i,  mask_height, role_posy+i);
					Vector3 pos8 = new Vector3 (role_posx-i,  mask_height, role_posy-i);
					if (isOnMap (pos5)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos5, rot));
						skill_range[(int)pos5.x,(int)pos5.z]=true;
					}
					if (isOnMap (pos6)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos6, rot));
						skill_range[(int)pos6.x,(int)pos6.z]=true;
					}
					if (isOnMap (pos7)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos7, rot));
						skill_range[(int)pos7.x,(int)pos7.z]=true;
					}
					if (isOnMap (pos8)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos8, rot));
						skill_range[(int)pos8.x,(int)pos8.z]=true;
					}
				}
			} else if (sk_info.shape == Shape.Horizontal) {
				for (int i = 1; i<= range; i++) {
					Vector3 pos1 = new Vector3 (role_posx+i,  mask_height, role_posy);
					Vector3 pos2 = new Vector3 (role_posx-i,  mask_height, role_posy);
					if (isOnMap (pos1)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos1, rot));
						skill_range[(int)pos1.x,(int)pos1.z]=true;
					}
					if (isOnMap (pos2)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos2, rot));
						skill_range[(int)pos2.x,(int)pos2.z]=true;
					}
				}
			} else if (sk_info.shape == Shape.Vertical) {
				for (int i = 1; i<= range; i++) {
					Vector3 pos3 = new Vector3 (role_posx,  mask_height, role_posy+i);
					Vector3 pos4 = new Vector3 (role_posx,  mask_height, role_posy-i);
					if (isOnMap (pos3)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos3, rot));
						skill_range[(int)pos3.x,(int)pos3.z]=true;
					}
					if (isOnMap (pos4)) {
						skill_mask.Add((GameObject)Instantiate (skill_mask_pref, pos4, rot));
						skill_range[(int)pos4.x,(int)pos4.z]=true;
					}
				}
			}
		}
		print ("create Skill range:" + skill_mask.Count);
	}

	void destroy_skill_range(){
		print ("destroy skill range:"+skill_mask.Count);
		foreach (GameObject ob in skill_mask) {
			print ("destroy skill range:" + ob);
			Destroy (ob);
		}
		skill_mask.Clear ();
	}

	void enable_skill_range(bool active){
		print ("enable_skill_range:"+active);
		foreach (GameObject ob in skill_mask) {
			ob.SetActive (active);
		}
	}

	public void button_skill(){
		if (!skill_prepared) {
			enable_skill_range (true);
			enable_move_range (false);
			enable_attack_range (false);
			skill_prepared = true;
		} else {
			enable_skill_range (false);
			skill_prepared = false;
		}
	}

	public void button_finish_card(){
		if (player_stat==Player_state.Play_card) {
			enable_move_range (true);
			enable_attack_range (true);
			player_stat= Player_state.Play_role;
			finish.SetActive (false);
		} 
	}

	public void button_pass(){
		moved = true;
		attacked = true;
	}

	public void button_ready(){
		print ("ready");
		if (game_stat == Game_state.Prepare) {
			game_stat = Game_state.Battle;
			player_stat = Player_state.Draw_card;
			isPlayer_turn = true;
			role_turn = 0;
			start = true;
			GameObject button = GameObject.Find ("Button_ready");
			button.SetActive (false);
			foreach (GameObject ob in blue_frames) {
				ob.SetActive (false);
			}
			red_mask.SetActive (false);

			for (int i = 0; i < player_roles.Length; i++) {
				Vector3 targetDir = Vector3.forward;
				player_roles[i].transform.rotation = Quaternion.LookRotation (targetDir);
				print ("rotate");
			}
		}
		for (int i = 0; i < init_card_num; i++) {
			draw_card ();
		}
	}


}
