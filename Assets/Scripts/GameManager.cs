using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


public class GameManager : MonoBehaviour {

    [SerializeField]
    public int score { get; private set; }

    const int scoreLimit = 99999999;

    public int startStage = 1;
    public int maxStage = 1;

    public float[] buff = { 1f, 1f, 1f };

    [SerializeField]
    private Text[] buffText;

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

    Coroutine[] coro = { null, null, null };

    [SerializeField]
    float timeLimit_;
    float timeLimit;

    // Use this for initialization
    void Start () {
        score = 0;
        timeLimit = timeLimit_;
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

    public void UpdateBuff()
    {
        buff.Select((b, i) => 
            new Tuple<string, int>(string.Format("x{0:F2}", b), i)
        ).ToList().ForEach(p => {
            buffText[p.Item2].text = p.Item1;

            if (coro[p.Item2] != null) StopCoroutine(coro[p.Item2]);
            coro[p.Item2] = StartCoroutine(Util.FrameTimer(10000, (t) => {
                buffText[p.Item2].color = new Color(1, t * 0.5f + 0.5f,  t, 0.75f);
            }, () => {
                buffText[p.Item2].color = new Color(1, 1, 1, 0.75f);
                buff[p.Item2] = 1f;
                buffText[p.Item2].text = string.Format("x{0:F2}", buff[p.Item2]);
                coro[p.Item2] = null;
            }));

        });
    }

    IEnumerator TutorialText(Text text, string mes, Func<bool> trigger)
    {
        text.text = mes;
        text.enabled = true;
        yield return this.UpdateAsObservable().Where(_ => trigger()).Take(1).StartAsCoroutine();
        text.enabled = false;
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator Tutorial()
    {
        var ttext = GameObject.Find("TutorialText").GetComponent<Text>();
        var tcur = GameObject.Find("TutorialCursor").GetComponentInChildren<Text>();
        tcur.enabled = false;
        var player = GameObject.Find("player").GetComponent<Player>();

        var filename = "tutorial";
        var textAsset = Resources.Load(filename) as TextAsset;
        Debug.AssertFormat(textAsset != null, "{0} is not found.", filename);
        JsonNode json = JsonNode.Parse(textAsset.text);

        var messages = json["message"].Select(e => new Tuple<string, string, string>(
            e["text"].Get<string>(),
            e["triggerType"].Get<string>(),
            e["trigger"].Get<string>()
        ));

        waveText.text = "TUTORIAL";
        waveText.enabled = true;
        yield return new WaitForSeconds(2f);
        waveText.enabled = false;

        GameObject.Find("TutorialCursor").GetComponent<Image>().enabled = true;

        foreach (var mes in messages)
        {
            player.lastRunes = "";
            var key = mes.Item3;
            var type = mes.Item2;
            Func<bool> trigger = () => true;
            if (type == "button")
            {
                tcur.enabled = true;
                trigger = () => Input.GetButtonDown(key);
            }
            else if (type == "player") trigger = () => {
                var triggerMap = "ABC".ToCharArray().ToDictionary(c => c, _ => 0);
                key.ToCharArray().ToList().ForEach(c => triggerMap[c]++);
                player.lastRunes.ToCharArray().ToList().ForEach(c => triggerMap[c]--);
                return triggerMap.All(c => c.Value == 0);
            };    
            yield return StartCoroutine(TutorialText(ttext, mes.Item1, trigger));
            tcur.enabled = false;
        }

        GameObject.Find("TutorialCursor").GetComponent<Image>().enabled = false;

        yield return new WaitForSeconds(1f);
    }

    IEnumerator Updater()
    {
        isClear = false;
        for(var stage = startStage; stage <= maxStage; ++stage)
        {
            var g = Instantiate(wavePrefab) as GameObject;
            
            if (stage > 0)
            {
                wave = g.GetComponent<Wave>();
                wave.Load(stage);
                
                waveText.text = "WAVE " + stage;
                waveText.enabled = true;

                yield return new WaitForSeconds(0.7f);

                waveText.enabled = false;
            }
            else
            {
                yield return StartCoroutine(Tutorial());
                wave = g.GetComponent<Wave>();
                wave.Load(stage);
            }

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
            timeText.text = string.Format("{0:D3}", (int)Mathf.Ceil(timeLimit));
            yield return new WaitForSeconds(0.3f);
        }
        
    }

}
