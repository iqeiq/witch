using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;


public class ScoreManager : MonoBehaviour
{
    public GameObject scoreTextRef;
    public GameObject rankTextRef;
    public Button button;


    string GetRank(int score)
    {
        if(score > 1000)
        {
            return "A";
        }
        return "B";
    }

    // Use this for initialization
    void Awake () {
        var gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        var score = gm.score;
        scoreTextRef.GetComponent<Text>().text = System.String.Format("{0:D8}", score);
        rankTextRef.GetComponent<Text>().text = GetRank(score);

        button.onClick.AsObservable().Subscribe(_ => {
            gm.StartFadeOut();
        });
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
