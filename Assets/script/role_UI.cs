using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class role_UI : MonoBehaviour {
	public GameObject HP_bar_pref;
	public GameObject HP_text_pref;
	public GameObject Pass_text_pref;
	public Canvas UI;
	public Slider HP_bar;
	public Text HP_text;
	public Text Pass_text;
	public Role role;
	bool isDead;

	// Use this for initialization
	void Start () {
		isDead = false;
		role = gameObject.GetComponent<Role> ();
		UI=GameObject.Find("role_UI").GetComponent<Canvas>();
		HP_bar=Instantiate (HP_bar_pref).GetComponent<Slider>();
		HP_bar.GetComponent<RectTransform> ().SetParent (UI.GetComponent<RectTransform>());
		HP_bar.maxValue=role.role_info.basic_life;
		HP_text=Instantiate (HP_text_pref).GetComponent<Text>();
		HP_text.GetComponent<RectTransform> ().SetParent (UI.GetComponent<RectTransform>());
		HP_text.text=role.role_info.basic_life+"";
		Pass_text=Instantiate (Pass_text_pref).GetComponent<Text>();
		Pass_text.GetComponent<RectTransform> ().SetParent (UI.GetComponent<RectTransform>());
		Pass_text.gameObject.SetActive (false);

	}
	
	// Update is called once per frame
	void Update () {
		if (role.battle_life == 0 && !isDead) {
			Destroy (HP_bar.gameObject);
			Destroy (HP_text.gameObject);
			isDead = true;
		}
		if (!isDead) {
			Vector2 pos1 = Camera.main.WorldToScreenPoint (gameObject.transform.position );
			HP_bar.GetComponent<RectTransform> ().position = pos1 + Vector2.up * 30;
			HP_bar.value = role.battle_life;
			Vector2 pos2 = Camera.main.WorldToScreenPoint (gameObject.transform.position);
			HP_text.GetComponent<RectTransform> ().position = pos2 + Vector2.up * 45;
			HP_text.text = role.battle_life + "";
			Vector2 pos3 = Camera.main.WorldToScreenPoint (gameObject.transform.position);
			Pass_text.GetComponent<RectTransform> ().position = pos3 + Vector2.up ;
		}
	}


		
}
