using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using System;


public class GameManager : MonoBehaviour {

    [SerializeField]
    public int score { get; private set; }

    const int scoreLimit = 99999999;

    public int startStage = 1;
    public int maxStage = 1;

    [SerializeField]
    private GameObject wavePrefab;

    [SerializeField]
    private GameObject waveTextRef;

    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private Text timeText;

    [SerializeField]
    private Text CountdownText;

    Text waveText;
    
    Wave wave = null;
    bool isClear = false;

    [SerializeField]
    float timeLimit;
    
	// Use this for initialization
	void Start () {
        score = 0;
        timeLimit = 90f;
        waveText = waveTextRef.GetComponent<Text>();
        waveText.enabled = false;
        CountdownText.enabled = false;
        StartCoroutine("Updater");
        StartCoroutine("FadeIn");
    }

    void Init()
    {
        this.InputAsObservable("Menu").Where(_ => !isClear).Take(1).Subscribe(_ => {
            StartFadeOut();
        });

        this.UpdateAsObservable().Select(_ => (int)Mathf.Floor(timeLimit))
            .DistinctUntilChanged()
            .Where(t => 0 <= t && t < 4)
            .Subscribe(t => { StartCoroutine(Countdown(t)); });

        StartCoroutine("TimeUpdater");
    }

    public void AddScore(int s)
    {
        if (isClear) return;
        score += s;
        score = Math.Min(scoreLimit, score);
        scoreText.text = string.Format("{0:D8}", score);
    }

    IEnumerator Countdown(int time)
    {
        CountdownText.text = string.Format("{0}", time);
        CountdownText.enabled = true;
        yield return new WaitForSeconds(0.7f);
        CountdownText.enabled = false;
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

            while (!wave.isFinish() && !isClear)
            {
                yield return new WaitForSeconds(0.5f);
            }
            DestroyImmediate(wave.gameObject);
            wave = null;

            // timeover
            if(isClear)
            {
                Debug.Log("TIMEOVER");
                break;
            }
            // TODO: 遷移アニメーション
        }
        isClear = true;
        Debug.Log("CLEAR!");
        AddScore(100 * (int)Mathf.Floor(timeLimit));
        SceneManager.LoadScene("Score", LoadSceneMode.Additive);
    }

    IEnumerator TimeUpdater()
    {
        var prev = DateTime.UtcNow;
        while (!isClear)
        {
            if (timeLimit > 0)
            {
                var now = DateTime.UtcNow;
                timeLimit -= (float)(now - prev).TotalSeconds;
                prev = now;
            }
            if(timeLimit <= 0f)
            {
                timeLimit = 0f;
                isClear = true;            
            }
            timeText.text = string.Format("{0:D2}", (int)Mathf.Floor(timeLimit));
            yield return new WaitForSeconds(0.3f);
        }
        
    }

}
