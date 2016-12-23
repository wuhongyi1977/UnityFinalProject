using UnityEngine;
using System.Collections;

public class ButtonBehavior : MonoBehaviour {

    public AudioClip CardSE;
    public AudioSource Audioplayer;
    Canvas canvas;
    Canvas bookCanvas;

	// Use this for initialization
	void Start () {
        canvas = GameObject.Find("ExitCanvas").GetComponent<Canvas>();
        canvas.enabled = false;
        bookCanvas = GameObject.Find("BookCanvas").GetComponent<Canvas>();
        bookCanvas.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PressCardCollection()
    {
        Audioplayer.PlayOneShot(CardSE);
        bookCanvas.enabled = true;

    }

    public void PressExitButton(){
        canvas.enabled = true;
    }
    public void NotToExit()
    {
        canvas.enabled = false;
    }
    public void CertainExit()
    {
        Application.Quit();
    }
    public void PressStoryLine()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StoryLine", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
