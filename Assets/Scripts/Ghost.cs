using UnityEngine;
using System.Collections;


public class Ghost : Enemy
{

    public override float GetMaxHP() { return 3; }

    protected override void InitEnemy()
    {
        coll.enabled = true;
    }

    IEnumerator DisappearMotion()
    {
        coll.enabled = false;

        var c = render.color;
        yield return StartCoroutine(Util.FrameTimer(1000f, (t) =>
        {
            render.color = new Color(c.r, c.g, c.b, (1f - t) * 0.5f + 0.25f); // 0.75 -> 0.25
        }, ()=> {
            render.color = new Color(c.r, c.g, c.b, 0.25f);
        }));
        
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(Util.FrameTimer(1000f, (t) =>
        {
            render.color = new Color(c.r, c.g, c.b, t * 0.5f + 0.25f);  // 0.25 -> 0.75
        }, () => {
            render.color = new Color(c.r, c.g, c.b, 0.75f);
        }));

        coll.enabled = true;

        yield return null;
    }

    IEnumerator AppearMotion()
    {
        var c = render.color;
        yield return StartCoroutine(Util.FrameTimer(1000f, (t) =>
        {
            render.color = new Color(c.r, c.g, c.b, (1f + t) * 0.25f + 0.5f); // 0.75 -> 1
        }, () => {
            render.color = new Color(c.r, c.g, c.b, 1f);
        }));
        
        yield return new WaitForSeconds(1f);

        if (isFreeze)
        {
            yield break;
        }

        yield return StartCoroutine(Util.FrameTimer(1000f, (t) =>
        {
            render.color = new Color(c.r, c.g, c.b, (2f - t) * 0.25f + 0.5f); // 1 -> 0.75
        }, () => {
            render.color = new Color(c.r, c.g, c.b, 0.75f);
        }));

        yield return null;
    }

    IEnumerator ToggleDisappear()
    {
        while (true)
        {
            if (isFreeze)
            {
                yield return new WaitForEndOfFrame();
                continue;
            }
            yield return StartCoroutine(DisappearMotion());
            yield return StartCoroutine(AppearMotion());
        }
    }
    
    protected override void OnAppear()
    {
        StartCoroutine(ToggleDisappear());   
    }
}
