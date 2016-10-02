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
        this.FadeIn(500, Init);
    }

    void Init()
    {
        start.onClick.AsObservable().Take(1).Subscribe(_ => {
            this.FadeOut(500, () => {
                SceneManager.LoadScene("Main");
            });
        });

        end.onClick.AsObservable()
            .Merge(this.InputAsObservable("Menu")).Take(1).Subscribe(_ => {
                Application.Quit();
            });
    }
    
}
