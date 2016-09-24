using UnityEngine;
using System.Collections;


public class Ghost : Enemy
{

    public const int maxhp = 3;
    
    protected override void InitEnemy()
    {
        hp = maxhp;
        coll.enabled = true;
    }


    IEnumerator ShiftDisappear()
    {
        float appear_sec = 1f;
        float anim_sec = 2.0f;

        yield return new WaitForSeconds(appear_sec);
        
        iTween.ValueTo(render.gameObject, iTween.Hash(
            "from", 1f,
            "to", 0.3f,
            "time", anim_sec,
            "islocal", true,
            "onupdate", "alphaUpdate",
            "easetype", iTween.EaseType.easeInOutQuad
        ));
        yield return new WaitForSeconds(anim_sec / 2);
        coll.enabled = false;
        yield return new WaitForSeconds(anim_sec / 2);

        StartCoroutine("ShiftAppear");
    }

    IEnumerator ShiftAppear()
    {
        float disappear_sec = 1f;
        float anim_sec = 2.0f;

        yield return new WaitForSeconds(disappear_sec);

        iTween.ValueTo(render.gameObject, iTween.Hash(
            "from", 0.3f,
            "to", 1f,
            "time", anim_sec,
            "islocal", true,
            "onupdate", "alphaUpdate",
            "easetype", iTween.EaseType.easeInOutQuad
        ));
        yield return new WaitForSeconds(anim_sec / 2);
        coll.enabled = true;
        yield return new WaitForSeconds(anim_sec / 2);

        StartCoroutine("ShiftDisappear");
    }



    protected override void OnAppear()
    {
        StartCoroutine("ShiftDisappear");   
    }
}
