using UnityEngine;
using System.Collections;

public class EnterName : MonoBehaviour {
	public UnityEngine.UI.Text playerName;////(1)
	public void EnterPlayerName(UnityEngine.UI.Text enterText){////(2)
		playerName.text = enterText.text;////(3)
	}
}
