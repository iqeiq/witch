using UnityEngine;
using System.Collections;

using System;


public class Enemy : Character
{

    public enum State
    {
        INIT = 0,
        READY,
        APPEAR,
        ALIVE,
        DISAPPEAR,
        DEAD = 0xDEAD
    }

    public const int maxhp = 2;
    int hp;

    public State state { get; private set; }

    float th1 = 0f, th2 = 0f;


    override protected void Init()
    {
        transform.localScale = new Vector2(0f, 0f);
        transform.localRotation = Quaternion.Euler(0, 0, 180);
        hp = maxhp;

        state = State.READY;
    }


    void Action()
    {
        if (state != State.ALIVE) return;

        th1 = (th1 > 360 ? th1 - 360 : th1) + 1;
        th2 = (th2 > 360 ? th2 - 360 : th2) + 1.1f;

        var pos = transform.position;
        pos.y = 0.8f * Mathf.Sin(th1 * Mathf.Deg2Rad) 
            + 0.4f * Mathf.Sin(th2 * 10 * Mathf.Deg2Rad);
        transform.position = pos;


        if (Input.GetButtonDown("Fire"))
            StartCoroutine("Disappear");

    }

    void alphaUpdate(float a)
    {
        var c = render.color;
        render.color = new Color(c.r, c.b, c.g, a);
    }

    IEnumerator Appear()
    {
        if (state != State.READY) yield break;
        state = State.APPEAR;

        var sec = 1f;

        iTween.RotateTo(gameObject, iTween.Hash(
            "z", 0,
            "time", sec,
            "islocal", true,
            "easetype", iTween.EaseType.easeInOutQuad
        ));

        iTween.ScaleTo(gameObject, iTween.Hash(
            "x", 0.75f,
            "y", 0.75f,
            "z", 1f,
            "time", sec,
            "islocal", true,
            "easetype", iTween.EaseType.easeInOutQuad
        ));

        iTween.ValueTo(render.gameObject, iTween.Hash(
            "from", 0f,
            "to", 1f,
            "time", sec,
            "islocal", true,
            "onupdate", "alphaUpdate",
            "easetype", iTween.EaseType.easeInOutQuad
        ));


        yield return new WaitForSeconds(sec);

        state = State.ALIVE;
        Debug.Log("alive");
    }

    IEnumerator Disappear()
    {
        if (state != State.ALIVE) yield break;
        state = State.DISAPPEAR;


        var sec = 1.5f;

        iTween.RotateTo(gameObject, iTween.Hash(
            "z", 180,
            "time", sec,
            "islocal", true,
            "easetype", iTween.EaseType.easeInOutQuad
        ));

        iTween.ScaleTo(gameObject, iTween.Hash(
            "x", 0f,
            "y", 0f,
            "z", 1f,
            "time", sec,
            "islocal", true,
            "easetype", iTween.EaseType.easeInOutQuad
        ));

        iTween.ValueTo(render.gameObject, iTween.Hash(
            "from", 1f,
            "to", 0f,
            "time", sec,
            "islocal", true,
            "onupdate", "alphaUpdate",
            "easetype", iTween.EaseType.easeInOutQuad
        ));

        yield return new WaitForSeconds(sec);

        state = State.DEAD;
        SetPosition(1.5f, -0.5f);
        Debug.Log("dead");
    }


    // Update is called once per frame
    new void Update ()
    {
        base.Update();

        switch (state)
        {
            case State.INIT:
                Init();
                break;
            case State.READY:
                StartCoroutine("Appear");
                break;
            case State.ALIVE:
                Action();
                break;
        }
	}
}
