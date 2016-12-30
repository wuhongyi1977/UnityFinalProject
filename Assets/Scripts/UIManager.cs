using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

    public GameObject startScreen;
    public string outTrigger;
    public List<GameObject> screenHistory;

    void Awake()
    {
        this.screenHistory = new List<GameObject> { this.startScreen };
    }

    public void ToScreen(GameObject target)
    {
        GameObject current = this.screenHistory[this.screenHistory.Count - 1];
        if (target == null || target == current) return;
        this.PlayScreen(current, target, false, this.screenHistory.Count);
        this.screenHistory.Add(target);
    }

    public void GoBack()
    {
        if(this.screenHistory.Count > 1)
        {
            int currentIndex = this.screenHistory.Count - 1;
            this.PlayScreen(this.screenHistory[currentIndex], this.screenHistory[currentIndex - 1], true, currentIndex - 2);
            this.screenHistory.RemoveAt(currentIndex);
        }
    }

    private void PlayScreen(GameObject current, GameObject target, bool isBack, int order)
    {
        current.GetComponent<Animator>().SetTrigger(this.outTrigger);
        if (isBack)
        {
            current.GetComponent<Canvas>().sortingOrder = order;
        }
        else
        {
            current.GetComponent<Canvas>().sortingOrder = order - 1;
            target.GetComponent<Canvas>().sortingOrder = order;
        }
        target.SetActive(true);
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
