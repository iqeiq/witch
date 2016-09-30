using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using System;
using System.Collections;


public class TitleManager : MonoBehaviour
{

    [SerializeField]
    private Button start;

    [SerializeField]
    private Button end;

    // Use this for initialization
    void Start () {
        StartCoroutine("FadeIn");
    }

    void Init()
    {
        start.onClick.AsObservable().Take(1).Subscribe(_ => {
            StartCoroutine("FadeOut");
        });

        end.onClick.AsObservable()
            .Merge(this.InputAsObservable("Menu")).Take(1).Subscribe(_ => {
                Application.Quit();
            });
    }

    IEnumerator FadeIn()
    {
        var fi = GetComponentInChildren<FadeImage>();
        var sec = 0.5f;
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
        Debug.Log("Title -> Main");
        var fi = GetComponentInChildren<FadeImage>();
        var sec = 0.2f;
        var wait = 8 / 1000f;
        var div = sec / wait;
        for (var i = 0; i < div; ++i)
        {
            fi.Range = i / div;
            yield return new WaitForSeconds(wait);
        }
        fi.Range = 1f;
        SceneManager.LoadScene("Main");
    }

}
