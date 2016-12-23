using UnityEngine;
using System.Collections;

public class UI_controll : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void click_pass(){
		GameObject.Find ("script").GetComponent<Battle> ().button_pass ();	
	}

	public void click_ready(){
		GameObject.Find ("script").GetComponent<Battle> ().button_ready ();
	}

	public void click_skill(){
		GameObject.Find ("script").GetComponent<Battle> ().button_skill ();
	}

	public void click_finish_card(){
		GameObject.Find ("script").GetComponent<Battle> ().button_finish_card();
	}
}
