using UnityEngine;
using UnityEngine.UI;
using UniRx;


public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private GameObject scoreTextRef;

    [SerializeField]
    private GameObject rankTextRef;

    [SerializeField]
    private Button button;


    string GetRank(int score)
    {
        if(score > 1000)
        {
            return "A";
        }
        return "B";
    }

    void Awake ()
    {
        var gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        scoreTextRef.GetComponent<Text>().text = string.Format("{0:D8}", gm.score);
        rankTextRef.GetComponent<Text>().text = GetRank(gm.score);

        button.onClick.AsObservable().Subscribe(_ => {
            gm.StartFadeOut();
        });
        
    }
	
}
