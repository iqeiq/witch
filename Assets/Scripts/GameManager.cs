using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using Util;


public class GameManager : MonoBehaviour {

    public int score = 0;
    public int startStage = 1;
    public int maxStage = 1;

    [SerializeField]
    private GameObject wavePrefab;

    [SerializeField]
    private GameObject waveTextRef;

    Text waveText;
    
    Wave wave = null;
    bool isClear = false;
    
	// Use this for initialization
	void Start () {
        score = 0;
        waveText = waveTextRef.GetComponent<Text>();
        waveText.enabled = false;
        StartCoroutine("Updater");
        StartCoroutine("FadeIn");
    }

    void Init()
    {
        this.InputAsObservable("Menu").Where(_ => !isClear).Take(1).Subscribe(_ => {
            StartFadeOut();
        });
    }

    IEnumerator FadeIn()
    {
        var fi = GetComponentInChildren<FadeImage>();
        var sec = 0.2f;
        var wait = 8 / 1000f;
        var div = sec / wait;
        for (var i = 0; i < div; ++i)
        {
            fi.Range = 1f - i / div;
            yield return new WaitForSeconds(wait);
        }
        fi.Range = 0f;
        Init();
    }

    IEnumerator FadeOut()
    {
        var fi = GetComponentInChildren<FadeImage>();
        var sec = 0.5f;
        var wait = 8 / 1000f;
        var div = sec / wait;
        for (var  i = 0; i < div; ++i)
        {
            fi.Range = i / div;
            yield return new WaitForSeconds(wait);
        }
        fi.Range = 1f;
        SceneManager.LoadScene("Title");
    }

    public void StartFadeOut()
    {
        StartCoroutine("FadeOut");
    }

    IEnumerator Updater()
    {
        isClear = false;
        for(var stage = startStage; stage <= maxStage; ++stage)
        {
            var g = Instantiate(wavePrefab) as GameObject;
            wave = g.GetComponent<Wave>();
            wave.Load(stage);

            // TODO:  display 「WAVE 1」
            waveText.text = "WAVE " + stage;
            waveText.enabled = true;
            
            yield return new WaitForSeconds(1f);

            waveText.enabled = false;

            while (!wave.isFinish())
            {
                yield return new WaitForSeconds(0.5f);
            }
            DestroyImmediate(wave.gameObject);
            wave = null;
            // TODO: 遷移アニメーション
        }
        isClear = true;
        Debug.Log("CLEAR!");
        SceneManager.LoadScene("Score", LoadSceneMode.Additive);
    }

}
