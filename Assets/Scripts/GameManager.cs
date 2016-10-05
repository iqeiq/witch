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
        timeLimit = 120f;
        waveText = waveTextRef.GetComponent<Text>();
        waveText.enabled = false;
        CountdownText.enabled = false;
        StartCoroutine(Updater());

        this.FadeIn(500, Init);
    }

    void Init()
    {
        this.InputAsObservable("Menu").Where(_ => !isClear).Take(1).Subscribe(_ => {
            StartFadeOut();
        });

        this.UpdateAsObservable().Select(_ => (int)Mathf.Ceil(timeLimit))
            .DistinctUntilChanged()
            .Where(t => 0 <= t && t < 4)
            .Subscribe(t => { StartCoroutine(Countdown(t)); });

        StartCoroutine(TimeUpdater());
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

    public void StartFadeOut()
    {
        this.FadeOut(500, ()=> {
            SceneManager.LoadScene("Title");
        });
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
            
            yield return new WaitForSeconds(0.7f);

            waveText.enabled = false;

            while (!wave.isFinish() && !isClear)
            {
                yield return new WaitForSeconds(0.2f);
            }
            DestroyImmediate(wave.gameObject);
            wave = null;

            // timeover
            if(isClear)
            {
                //Debug.Log("TIMEOVER");
                break;
            }
            // TODO: 遷移アニメーション
        }
        AddScore(100 * (int)Mathf.Ceil(timeLimit));
        isClear = true;
        //Debug.Log("CLEAR!");
        yield return new WaitForSeconds(0.5f);
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
            timeText.text = string.Format("{0:D2}", (int)Mathf.Ceil(timeLimit));
            yield return new WaitForSeconds(0.3f);
        }
        
    }

}
