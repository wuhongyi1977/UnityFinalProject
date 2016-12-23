using UnityEngine;
using System.Collections;

public class CreateCharacter : MonoBehaviour {

	public UnityEngine.UI.Text Choose;
	public UnityEngine.UI.Text playerName;
	public UnityEngine.UI.Button ConfirmName;
	public UnityEngine.UI.InputField YourName;
	public UnityEngine.UI.Button Man;

	public Canvas canvas_Gender;
    public Canvas canvas_College;
    public Canvas canvas_End;

	int stage = 1;
	string name;
	bool gender = true;
    int college = 1;

	// Use this for initialization
	void Start () {
		canvas_Gender = GameObject.Find ("Canvas_Gender").GetComponent<Canvas> ();
		canvas_Gender.enabled = false;
        canvas_College = GameObject.Find("Canvas_College").GetComponent<Canvas>();
        canvas_College.enabled = false;
        canvas_End = GameObject.Find("Canvas_End").GetComponent<Canvas>();
        canvas_End.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public void StageZero(){
	
	}
	public void StageOne(UnityEngine.UI.Text enterText){
		playerName.text = enterText.text;
		name = playerName.text.ToString();
		StageTwo ();
	}
    public void StageTwo(){
		canvas_Gender.enabled = true;
		UnityEngine.UI.Text text = GameObject.Find ("Text_Gender").GetComponent<UnityEngine.UI.Text> ();
		text.text = name + "  你的性別是";
	}
    public void StageThree()
    {
        canvas_College.enabled = true;
    }
    public void SelectGround()
    {
        college = 1;
        canvas_End.enabled = true;
        UnityEngine.UI.Text text = GameObject.Find("Text_End").GetComponent<UnityEngine.UI.Text>();
        text.text = name + "  你決定進入傾嘩魔法學園的牲珂土學院就讀\n利用你所學到的大地魔法\n震撼整個世界吧!!!";
    }
    public void SelectWater()
    {
        college = 2;
        canvas_End.enabled = true;
        UnityEngine.UI.Text text = GameObject.Find("Text_End").GetComponent<UnityEngine.UI.Text>();
        text.text = name + "  你決定進入傾嘩魔法學園的壬舍水學院就讀\n利用你所學到的水系魔法\n在世界裡掀起一陣波瀾吧!!!";
    }
    public void SelectFire()
    {
        college = 3;
        canvas_End.enabled = true;
        UnityEngine.UI.Text text = GameObject.Find("Text_End").GetComponent<UnityEngine.UI.Text>();
        text.text = name + "  你決定進入傾嘩魔法學園的茲甸火學院就讀\n利用你所學到的火系魔法\n將你的敵人燃燒殆盡吧!!!";
    }
    public void SelectThunder()
    {
        college = 4;
        canvas_End.enabled = true;
        UnityEngine.UI.Text text = GameObject.Find("Text_End").GetComponent<UnityEngine.UI.Text>();
        text.text = name + "  你決定進入傾嘩魔法學園的戊鋰雷學院就讀\n利用你所學到的雷電魔法\n閃電~~~~~~~~~~~~~~~~~~~~~!!!";
    }
    public void SelectWind()
    {
        college = 5;
        canvas_End.enabled = true;
        UnityEngine.UI.Text text = GameObject.Find("Text_End").GetComponent<UnityEngine.UI.Text>();
        text.text = name + "  你決定進入傾嘩魔法學園的花薛風學院就讀\n利用你所學到的風系魔法\n排除眼前的一切阻礙吧";
    }
    public void GoTeaching()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("battle", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
