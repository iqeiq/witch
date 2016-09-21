﻿using UnityEngine;
using System.Collections;


public abstract class Enemy : Character
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

    protected int hp;
    protected float prev_y, base_y;
    
    public State state { get; private set; }
    

    protected sealed override void Init()
    {
        transform.localScale = new Vector2(0f, 0f);
        transform.localRotation = Quaternion.Euler(0, 0, 180);

        InitEnemy();

        state = State.READY;
    }

    protected virtual void InitEnemy() { }


    void Action()
    {
        if (state != State.ALIVE) return;
            
        ActionEnemy();

        if (hp < 0)
        {
            StartCoroutine("Disappear");
        }
    }

    protected virtual void ActionEnemy() { }


    protected void alphaUpdate(float a)
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

        base_y = prev_y = transform.position.y;
        state = State.ALIVE;
        OnAppear();

        yield return new WaitForSeconds(UnityEngine.Random.value / 2);
        Shake1();
        //Debug.Log("alive");
    }

    void Shake1()
    {
        float sec = 1.5f;
        iTween.RotateTo(gameObject, iTween.Hash(
            "z", 5,
            "time", sec,
            "islocal", true,
            "easetype", iTween.EaseType.easeInOutQuad,
            "oncomplete", "Shake2"
        ));
    }
    void Shake2()
    {
        float sec = 1.5f;
        iTween.RotateTo(gameObject, iTween.Hash(
            "z", -5,
            "time", sec,
            "islocal", true,
            "easetype", iTween.EaseType.easeInOutQuad,
            "oncomplete", "Shake1"
        ));
    }

    protected virtual void OnAppear() { }


    IEnumerator Disappear()
    {
        if (state != State.ALIVE) yield break;
        state = State.DISAPPEAR;


        var sec = 1.5f;

        iTween.RotateTo(gameObject, iTween.Hash(
            "y", 180,
            "time", sec,
            "islocal", true,
            "easetype", iTween.EaseType.easeOutQuad
        ));

        iTween.ScaleTo(gameObject, iTween.Hash(
            "x", 0f,
            "y", 0f,
            "z", 1f,
            "time", sec,
            "islocal", true,
            "easetype", iTween.EaseType.easeOutQuad
        ));

        iTween.ValueTo(render.gameObject, iTween.Hash(
            "from", 1f,
            "to", 0f,
            "time", sec,
            "islocal", true,
            "onupdate", "alphaUpdate",
            "easetype", iTween.EaseType.easeOutQuad
        ));

        yield return new WaitForSeconds(sec);

        state = State.DEAD;
        SetPosition(1.5f, 1.5f);
        Destroy(gameObject);
        //Debug.Log("dead");

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
