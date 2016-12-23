using UnityEngine;
using System.Collections;


public class Map {
	public GameObject[,] floors;
	public int lenth;
	public Map(){
		GameObject[] childen = GameObject.FindGameObjectsWithTag ("floor");
		lenth = (int)Mathf.Sqrt (childen.Length);

		floors=new GameObject[lenth,lenth];
		foreach (GameObject floor in childen) {
			int pos_x = (int)floor.transform.position.x;
			int pos_y = (int)floor.transform.position.z;
			floors [pos_x, pos_y] = floor;
		}
	}
}
