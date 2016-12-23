using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI : MonoBehaviour {
	bool canAttack;
	bool[,] move_range;
	public bool acted;

	// Use this for initialization
	void Start () {
		print ("map lenth"+Battle.map.lenth);
		move_range = new bool[Battle.map.lenth, Battle.map.lenth];
		for (int i=0; i < Battle.map.lenth; i++) {
			for (int j = 0; j < Battle.map.lenth; j++) {
				move_range [i, j] = false;
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void aiAction(){
		Role role = gameObject.GetComponent<Role> ();
		Vector3 player_pos = searchPlayerInAttackRange ();
		if (player_pos.x!=role.pos_x || player_pos.z!=role.pos_y) {
			//有攻擊對象
			print("AI attack");
			Role target_player=Battle.location_roles [(int)player_pos.x, (int)player_pos.z].GetComponent<Role>();
			role.onAttack (target_player);
			return;
		} else {
			//move
			print("AI move:");
			createMoveRange ();
			Vector3 target_pos = searchClosestPlayerOnMap ();
			print ("closest player:" + target_pos);
			ArrayList move_pos = new ArrayList ();
			for (int i = 0; i < Battle.map.lenth; i++) {
				for (int j = 0; j < Battle.map.lenth; j++) {
					if (move_range [i, j]) {
						move_pos.Add (new Vector2 (i, j));
						print ("move range:"+i+","+j);
					}
				}
			}
			Vector2[] sorted_move_pos = sortByDistance (move_pos, new Vector2 (target_pos.x, target_pos.z));
			for (int i = 0; i < sorted_move_pos.Length; i++) {
				Vector2 moveTo = sorted_move_pos [i];
				if (Battle.location_roles [(int)moveTo.x, (int)moveTo.y] == null) {
					role.onMove ((int)moveTo.x, (int)moveTo.y);
					break;
				}
			}
			//再檢查一次攻擊對象
			player_pos=searchPlayerInAttackRange ();

			if (player_pos.x!=gameObject.transform.position.x && player_pos.z!=gameObject.transform.position.z) {
				Role target_player=Battle.location_roles [(int)player_pos.x, (int)player_pos.z].GetComponent<Role>();
				role.onAttack (target_player);
				return;
			}
		}
		for (int i=0; i < Battle.map.lenth; i++) {
			for (int j = 0; j < Battle.map.lenth; j++) {
				move_range [i, j] = false;
			}
		}
	}
		

	Vector2[] sortByDistance(ArrayList arr,Vector2 target){
		int len = arr.Count;
		Vector2[] result=new Vector2[len];
		for (int i = 0; i < len; i++) {
			Vector2 closePoint=new Vector2( gameObject.transform.position.x,gameObject.transform.position.z);
			float minDis = Battle.map.lenth*Battle.map.lenth;
			int index = 0;
			for (int j = 0; j < arr.Count; j++) {
				Vector2 cur = (Vector2)arr [j];
				float dis = (target - cur).magnitude;
				if (dis < minDis) {
					closePoint = cur;
					minDis = dis;
					index = j;
				}
			}
			result [i] = closePoint;
			arr.RemoveAt(index);
		}
		print ("sorted move:");
		for (int i = 0; i < result.Length; i++) {
			print (result[i]);
		}
		return result;
	}

	void createMoveRange(){
		Role role = gameObject.GetComponent<Role> ();
		int role_posx = (int)role.transform.position.x;
		int role_posy = (int)role.transform.position.z;
		int range = role.battle_move_range;
		Vector3 pos = new Vector3 (role_posx, 0, role_posy);
		for (int i = 1; i <= range; i++) {
			Vector3 pos1 = new Vector3 (role_posx+i, 0, role_posy);
			Vector3 pos2 = new Vector3 (role_posx-i, 0, role_posy);
			Vector3 pos3 = new Vector3 (role_posx, 0, role_posy+i);
			Vector3 pos4 = new Vector3 (role_posx, 0, role_posy-i);
			if (Battle.isOnMap (pos1)) {
				move_range[(int)pos1.x,(int)pos1.z]=true;
			}
			if (Battle.isOnMap (pos2)) {
				move_range[(int)pos2.x,(int)pos2.z]=true;
			}
			if (Battle.isOnMap (pos3)) {
				move_range[(int)pos3.x,(int)pos3.z]=true;
			}
			if (Battle.isOnMap (pos4)) {
				move_range[(int)pos4.x,(int)pos4.z]=true;
			}
			for (int j = 1; j <= range - i; j++) {
				Vector3 pos5 = new Vector3 (role_posx+i, 0, role_posy+j);
				Vector3 pos6 = new Vector3 (role_posx+i, 0, role_posy-j);
				Vector3 pos7 = new Vector3 (role_posx-i, 0, role_posy+j);
				Vector3 pos8 = new Vector3 (role_posx-i, 0, role_posy-j);
				if (Battle.isOnMap (pos5)) {
					move_range[(int)pos5.x,(int)pos5.z]=true;
				}
				if (Battle.isOnMap (pos6)) {
					move_range[(int)pos6.x,(int)pos6.z]=true;
				}
				if (Battle.isOnMap (pos7)) {
					move_range[(int)pos7.x,(int)pos7.z]=true;
				}
				if (Battle.isOnMap (pos8)) {
					move_range[(int)pos8.x,(int)pos8.z]=true;
				}
			}
		}
	}

	Vector3 searchPlayerInAttackRange(){
		Role role = gameObject.GetComponent<Role> ();
		GameObject[,] location_roles = Battle.location_roles;
		int role_posx = role.pos_x;
		int role_posy = role.pos_y;
		int range = role.battle_attack_range;
		Vector3 pos = new Vector3 (role_posx, 0, role_posy);
		for (int i = 1; i <= range; i++) {
			Vector3 pos1 = new Vector3 (role_posx+i, 0, role_posy);
			Vector3 pos2 = new Vector3 (role_posx-i, 0, role_posy);
			Vector3 pos3 = new Vector3 (role_posx, 0, role_posy+i);
			Vector3 pos4 = new Vector3 (role_posx, 0, role_posy-i);
			if (Battle.isOnMap (pos1) && location_roles[(int)pos1.x,(int)pos1.z]!=null && location_roles[(int)pos1.x,(int)pos1.z].tag=="Player") {
				print ("pos1 true");
				return pos1;
			}
			if (Battle.isOnMap (pos2) && location_roles[(int)pos2.x,(int)pos2.z]!=null && location_roles[(int)pos2.x,(int)pos2.z].tag=="Player") {
				print ("pos2 true");
				return pos2;
			}
			if (Battle.isOnMap (pos3) && location_roles[(int)pos3.x,(int)pos3.z]!=null && location_roles[(int)pos3.x,(int)pos3.z].tag=="Player") {
				print ("pos3 true");
				return pos3;
			}
			if (Battle.isOnMap (pos4) && location_roles[(int)pos4.x,(int)pos4.z]!=null && location_roles[(int)pos4.x,(int)pos4.z].tag=="Player") {
				print ("pos4 true");
				return pos4;
			}
			for (int j = 1; j <= range - i; j++) {
				Vector3 pos5 = new Vector3 (role_posx+i, 0, role_posy+j);
				Vector3 pos6 = new Vector3 (role_posx+i, 0, role_posy-j);
				Vector3 pos7 = new Vector3 (role_posx-i, 0, role_posy+j);
				Vector3 pos8 = new Vector3 (role_posx-i, 0, role_posy-j);
				if (Battle.isOnMap (pos5) && location_roles[(int)pos5.x,(int)pos5.z]!=null && location_roles[(int)pos5.x,(int)pos5.z].tag=="Player") {
					print ("pos5 true");
					return pos5;
				}
				if (Battle.isOnMap (pos6) && location_roles[(int)pos6.x,(int)pos6.z]!=null && location_roles[(int)pos6.x,(int)pos6.z].tag=="Player") {
					print ("pos6 true");
					return pos6;
				}
				if (Battle.isOnMap (pos7) && location_roles[(int)pos7.x,(int)pos7.z]!=null && location_roles[(int)pos7.x,(int)pos7.z].tag=="Player") {
					print ("pos7 true");
					return pos7;
				}
				if (Battle.isOnMap (pos8) && location_roles[(int)pos8.x,(int)pos8.z]!=null && location_roles[(int)pos8.x,(int)pos8.z].tag=="Player") {
					print ("pos8 true");
					return pos8;
				}
			}
		}
		print ("can't attack player");
		return pos;
	}

	Vector3 searchClosestPlayerOnMap(){
		Role role = gameObject.GetComponent<Role> ();
		GameObject[] player_roles = GameObject.Find("script").GetComponent<Battle>().player_roles;
		int role_posx = (int)role.transform.position.x;
		int role_posy = (int)role.transform.position.z;
		Vector2 pos = new Vector2 (role_posx, role_posy);

		float minDis = Battle.map.lenth*Battle.map.lenth;
		int index = 0;
		for (int i = 0; i < player_roles.Length; i++) {
			Vector2 player_pos =new Vector2( player_roles [i].transform.position.x,player_roles [i].transform.position.z);
			float dis = (player_pos - pos).magnitude;
			if (dis < minDis) {
				minDis = dis;
				index = i;
			}	
		}
		return player_roles [index].transform.position;
	}


}
