using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardInfo : MonoBehaviour {
	static public float card_y=1.0f;
	static public float card_z=-1.6f;
	static public float card_x_rate=1.5f;
	static public float card_x_offset=1.5f;

	public GameObject floor_pref;
	public GameObject floor_particle_pref;
	public GameObject mask_pref;
	public GameObject empty;
	public Element element;
	public int range;
	public int level;
	public string element_word;
	public string explain;

	GameObject all_mask;
	List<GameObject> floor_mask = new List<GameObject> ();
	Camera camera; 
	float card_height=0.0f;
	float floating_height=2.0f;
	float floating_rate=10.0f;
	bool floating;
	bool put_floor;
	static public bool using_card=false;

	// Use this for initialization
	void Start () {
		camera = Camera.main;
		put_floor = false;
		element_word = elementToWord (element);
		explain="在場上創造出"+range+"*"+range+"的等級"+level+","+element_word+"屬性魔方";
	}
	
	// Update is called once per frame
	void Update () {
		float step = floating_rate * Time.deltaTime;
		if (floating || put_floor && !using_card) {
			Vector3 pos = gameObject.transform.position;
			Vector3 target = new Vector3 (pos.x, floating_height, pos.z);
			gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, target, step);
		} else {
			Vector3 pos = gameObject.transform.position;
			Vector3 target = new Vector3 (pos.x, card_height, pos.z);
			gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, target, step);
		}
			
		if (put_floor) {
			if (Input.GetMouseButtonDown (1) || Battle.player_stat!=Player_state.Play_card) {
				put_floor = false;
				using_card = false;
				Destroy (all_mask);
				floor_mask.Clear ();
			} else {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit = new RaycastHit ();
				if (Physics.Raycast (ray, out hit)) {
					GameObject touch = hit.collider.gameObject;
					print ("touch:" + touch);
					update_mask ((int)touch.transform.position.x, (int)touch.transform.position.z);
					if (Input.GetMouseButtonDown (0) && Battle.isOnMap (touch.transform.position)) {
						print ("put floor");
						placeFloor ();
						Destroy (all_mask);
						floor_mask.Clear ();
						Vector3 pos = gameObject.transform.position;
						Quaternion rot = new Quaternion (0, 0, 0, 0);
						GameObject particle = (GameObject)Instantiate (floor_particle_pref, pos, rot);
						Destroy (gameObject, 1.0f);
						Destroy (particle, 1.0f);
						put_floor = false;
						using_card = false;
					}
				}
			}
		}
	}

	public void pointEnter(){
		floating=true;
	}

	public void pointExit(){
		floating=false;
	}

	public void pointClick(){
		if(using_card==false && Battle.player_stat==Player_state.Play_card){
			if (put_floor == false) {
				put_floor = true;
				create_mask ();
				using_card = true;
			} 
		}
	}

	void create_mask(){
		Vector3 pos = new Vector3 (0, Battle.mask_height,0);
		Quaternion rot = new Quaternion (0, 0, 0, 0);
		all_mask=(GameObject)Instantiate (empty, pos, rot);
		for (int i = 0; i < range; i++) {
			for (int j = 0; j < range; j++) {
				Vector3 pos1=new Vector3 (i-range/2, Battle.mask_height,j-range/2);
				GameObject ob = (GameObject)Instantiate (mask_pref, pos1, rot);
				ob.transform.parent = all_mask.transform;
				floor_mask.Add (ob);
			}
		}
	}
		
	void destroy_mask(){
		foreach (GameObject ob in floor_mask) {
			Destroy (ob);
		}
	}

	void enable_mask(bool active){
		foreach (GameObject ob in floor_mask) {
			ob.SetActive (active);
		}
	}

	void update_mask (int x,int y){
		all_mask.transform.position = new Vector3 (x,Battle.mask_height,y);
		foreach (GameObject ob in floor_mask) {
			if (Battle.isOnMap (ob.transform.position)) {
				ob.SetActive (true);
			} else {
				ob.SetActive (false);
			}
		}
	}

	void placeFloor(){
		foreach (GameObject ob in floor_mask) {
			if (ob.activeSelf) {
				int x = (int)ob.transform.position.x;
				int y = (int)ob.transform.position.z;
				print ("put on:"+x+","+y);
				Vector3 pos = new Vector3 (x, 0,y);
				Vector3 pos1 = new Vector3 (x, 0.5f,y);
				Quaternion rot = new Quaternion (0, 0, 0, 0);
				GameObject particle=(GameObject)Instantiate (floor_particle_pref, pos1, rot);
				Destroy (particle, 2.0f);
				Destroy (Battle.map.floors [x, y]);
				Battle.map.floors [x, y]=(GameObject)Instantiate (floor_pref, pos, rot);
			}
		}
	}

	public static string elementToWord(Element e){
		if (e == Element.Earth) {
			return "地";
		}else if(e == Element.Fire) {
			return "火";
		}else if(e == Element.Thunder) {
			return "雷";
		}else if(e == Element.Water) {
			return "水";
		}else if(e == Element.Wind) {
			return "風";
		}else {
			return "無";
		}
	}
}
