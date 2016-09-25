using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;


public class GameManager : MonoBehaviour {

    public int score = 0;
    public int startStage = 1;
    public int maxStage = 1;
    public GameObject wavePrefab;
    public GameObject waveTextRef;
    Text waveText;
    
    Wave wave = null;
    bool isClear = false;
    bool isFadeout = false;

	// Use this for initialization
	void Start () {
        score = 0;
        waveText = waveTextRef.GetComponent<Text>();
        waveText.enabled = false;
        isFadeout = false;
        StartCoroutine("Updater");
        StartCoroutine("FadeIn");
    }

    void Init()
    {

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
        isFadeout = true;
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
        if (!isFadeout) StartCoroutine("FadeOut");
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
	
	// Update is called once per frame
	void Update () {

        // TODO:
        if (Input.GetButtonDown("Menu") && !isClear)
        {
            StartFadeOut();
        }

    }
}
