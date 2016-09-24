using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;


public class GameManager : MonoBehaviour {

    public int score = 0;
    public int startStage = 1;
    public int maxStage = 1;
    public GameObject wavePrefab;

    Wave wave = null;
    bool isClear = false;

	// Use this for initialization
	void Start () {
        score = 0;
        StartCoroutine("Updater");
    }

    IEnumerator Updater()
    {
        isClear = false;
        for(var stage = startStage; stage <= maxStage; ++stage)
        {
            var g = Instantiate(wavePrefab) as GameObject;
            wave = g.GetComponent<Wave>();
            wave.Load(stage);
            yield return new WaitForSeconds(1f);
            // TODO:  display 「WAVE 1」
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
    }
	
	// Update is called once per frame
	void Update () {

        // TODO:
        if (Input.GetButtonDown("Menu"))
        {
            SceneManager.LoadScene("Title");
        }

        if(isClear)
        {
            // TODO: SCORE
        }
    }
}
