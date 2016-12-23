using UnityEngine;
using System.Collections;

public class Role : MonoBehaviour {
	static public float role_height = 0.3f;
	static public float posxy_offset = 0.1f;
	public GameObject skill_particle;
	public RoleInfo role_info;
	public SkillInfo skill_info;
	public int battle_life;
	public int battle_damage;
	public int battle_move_range;
	public int battle_attack_range;
	public int battle_cd;
	public bool moving;
	public bool attacking;
	public bool casting;
	public int pos_x;
	public int pos_y;
	float moving_rate=5f;
	Animator ani;
	float cast_timer;
	float cast_time=2.0f;
	float attack_timer;
	float attack_time=2.0f;

	// Use this for initialization
	void Start () {
		initialize ();
	}
	
	// Update is called once per frame
	void Update () {
		if (battle_life == 0) {
			onDead ();
		}else if (moving) {
			float step = moving_rate * Time.deltaTime;
			Vector3 target = new Vector3 (pos_x, role_height, pos_y);
			Vector3 targetDir = target - gameObject.transform.position;
			Vector3 newDir = Vector3.RotateTowards (gameObject.transform.forward, targetDir, step*10, 0.0f);
			newDir.y = 0;
			gameObject.transform.rotation = Quaternion.LookRotation (newDir);
			gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, target,step);
			if (Vector3.Distance (gameObject.transform.position, target) ==0) {
				ani.SetBool("Walk",false);
				moving = false;
			}
		}else if (attacking) {
			attack_timer += Time.deltaTime;
			if (attack_timer>attack_time)
				attacking = false;
		}else if (casting) {
			cast_timer += Time.deltaTime;
			if (cast_timer>cast_time) 
				casting = false;
		}

	}

	public void initialize(){
		this.battle_life=this.role_info.basic_life;
		this.battle_damage=this.role_info.basic_damage;
		this.battle_move_range=this.role_info.basic_move_range;
		this.battle_attack_range=this.role_info.basic_attack_range;
		this.battle_cd = skill_info.basic_cd;
		this.ani = gameObject.GetComponentInChildren<Animator> ();
		this.moving = false;
	}

	public void changePos(int x,int y){
		pos_x = x;
		pos_y = y;
		Vector3 pos = new Vector3 (x,role_height,y);
		gameObject.transform.position = pos;
	}

	public void onDead(){
		ani.SetTrigger ("toDead");
	}

	public void onDamage(float damage){
		battle_life -= (int)damage;
		if (battle_life < 0)
			battle_life = 0;
		ani.SetTrigger ("toDamage");
	}

	public void onAttack(Role role){
		float damage = battle_damage;
		Element e=Battle.map.floors[pos_x,pos_y].GetComponent<FloorInfo>().element;
		if (role_info.element == e) {
			damage *= 2;
		}else if (e == Element.Zero) {
			damage = battle_damage;
		}else if(e== RoleInfo.findAgainstElement(role_info.element)){
			damage *= 0.5f;
		}
		attacking = true;
		attack_timer = 0;
		Vector3 targetDir = new Vector3(role.pos_x-pos_x,0,role.pos_y-pos_y);
		gameObject.transform.rotation = Quaternion.LookRotation (targetDir);
		role.onDamage (battle_damage);
		ani.SetTrigger ("toAttack");
	}

	public void onMove(int x,int y){
		print (gameObject+" move to"+ x +"," +y);
		ani.SetBool("Walk",true);
		moving = true;
		Battle.location_roles [pos_x, pos_y] = null;
		Battle.location_roles[x,y]=gameObject;
		pos_x = x;
		pos_y = y;

	}

	public bool canUseSkill (){
		//check cd and range
		Element e=Battle.map.floors[pos_x,pos_y].GetComponent<FloorInfo>().element;
		if(battle_cd ==0 && role_info.element==e || role_info.element==Element.Zero)
			return true;
		//print ("can't use skill,floor is "+floor_name);
		return false;
	}
		
	public void onSkill(){
		ani.SetTrigger ("toSkill");
		casting = true;
		cast_timer = 0;
		foreach (GameObject ob in GameObject.Find("script").GetComponent<Battle>().skill_mask) {
			int x = (int)ob.transform.position.x;
			int y = (int)ob.transform.position.z;
			GameObject target = Battle.location_roles [x, y];
			create_skill_particle (x, y);
			if (target != null && target.tag=="enemy") {
				Role role = target.GetComponent<Role> ();
				role.onDamage (this.battle_damage*skill_info.basic_damage);
			}
		}
		battle_cd = skill_info.basic_cd;
	}

	public void onSkill(Role target){
		ani.SetTrigger ("toSkill");
		casting = true;
		cast_timer = 0;
		create_skill_particle (target.pos_x, target.pos_y);
		target.onDamage (this.battle_damage*skill_info.basic_damage);
		battle_cd = skill_info.basic_cd;
	}

	void create_skill_particle(int x,int y){
		Vector3 pos = new Vector3 (x, role_height, y);
		Quaternion rot = new Quaternion (0, 0, 0, 0);
		GameObject p = (GameObject)Instantiate (skill_particle, pos, rot);
		Destroy (p, 5.0f);
	}
}
