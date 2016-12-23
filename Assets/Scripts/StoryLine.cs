using UnityEngine;
using System.Collections;

public class StoryLine : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PressGoBack()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("mainScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
