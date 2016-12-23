using UnityEngine;
using System.Collections;

public class map_maker : MonoBehaviour {

	public GameObject[] floor_Prefabs;
	public GameObject frame_Prefab;
	public GameObject empty_map;
	public int lenth ;
	public int width ;
	public GameObject[,] floors;
	public GameObject[,] frames;
	public GameObject map;

	// Use this for initialization
	void Start () {
		floors =new GameObject[lenth,lenth];
		frames =new GameObject[lenth,lenth];
		Quaternion rot = new Quaternion (0, 0, 0, 0);
		Vector3 pos = new Vector3 (0, 0, 0);
		map=(GameObject)Instantiate (empty_map, pos, rot);
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < lenth; j++) {
				int k = Random.Range (0, floor_Prefabs.Length);
				Vector3 pos1 = new Vector3 (i, 0, j);
				Vector3 pos2 = new Vector3 (i, 0.01f, j);
				floors[i,j] = (GameObject)Instantiate (floor_Prefabs[k], pos1, rot);
				floors[i,j].transform.parent = map.transform;
				frames[i,j] = (GameObject)Instantiate (frame_Prefab, pos2, rot);
				frames[i,j].transform.parent = map.transform;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
