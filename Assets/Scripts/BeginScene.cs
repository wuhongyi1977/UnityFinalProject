using UnityEngine;
using System.Collections;

public class BeginScene : MonoBehaviour {

	Canvas canvas;

	// Use this for initialization
	void Start () {
		canvas = GameObject.Find ("ExitCanvas").GetComponent<Canvas> ();
		canvas.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Exit(){
		canvas.enabled = true;
	}
	public void CertainExit(){
		Application.Quit();
	}
	public void DontExit(){
		canvas.enabled = false;
	}
	public void StartNewGame(){
		//UnityEngine.SceneManagement.SceneManager.LoadScene("Create", UnityEngine.SceneManagement.LoadSceneMode.Single);
	}
}
