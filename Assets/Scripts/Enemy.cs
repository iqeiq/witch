using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;


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

    
    [SerializeField]
    protected bool isFreeze = false;

    [SerializeField]
    private GameObject freezePrefab;

    const float scale = 0.75f;

    protected float prev_y, base_y;
    protected CircleCollider2D coll = null;
    protected Animator anim;

    public float hp { get; protected set; }
    public State state { get; private set; }

    public abstract float GetMaxHP();
    

    
    protected sealed override void Init()
    {
        transform.localScale = new Vector2(0f, 0f);
        transform.localRotation = Quaternion.Euler(0, 0, 180);
        coll = GetComponent<CircleCollider2D>();
        coll.enabled = false;

        anim = GetComponent<Animator>();
        anim.enabled = false;

        state = State.READY;
        isFreeze = false;
        hp = GetMaxHP();


        var fr = Util.CreateAndGetComponent<SpriteRenderer>(freezePrefab, transform);
        fr.enabled = false;
        var fc = fr.GetComponent<BoxCollider2D>();
        fc.enabled = false;

        this.ToObservable(() => isFreeze)
            .DistinctUntilChanged()
            .Subscribe(v => {
                anim.SetBool("isFreeze", v);
                fr.enabled = v;
                fc.enabled = v;
                coll.enabled = !v;
            });
        
        InitEnemy();
    }

    protected virtual void InitEnemy() { }


    void Action()
    {
        Debug.Assert(state == State.ALIVE);
        
        if (!isFreeze)
        {
            ActionEnemy();
        }

        if (!(hp > 0))
        {
            StartCoroutine("Disappear");
        }

        prev_y = transform.position.y;
    }

    protected virtual void ActionEnemy() { }


    protected void alphaUpdate(float a)
    {
        var c = render.color;
        render.color = new Color(c.r, c.b, c.g, a);
    }

    IEnumerator Appear()
    {
        Debug.Assert(state == State.READY);
        state = State.APPEAR;

        var sec = 1f;

        iTween.RotateTo(gameObject, iTween.Hash(
            "z", 0,
            "time", sec,
            "islocal", true,
            "easetype", iTween.EaseType.easeInOutQuad
        ));

        iTween.ScaleTo(gameObject, iTween.Hash(
            "x", scale,
            "y", scale,
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
        coll.enabled = true;
        OnAppear();

        yield return new WaitForSeconds(UnityEngine.Random.value / 2);
        anim.enabled = true;
        //Debug.Log("alive");
    }

    protected virtual void OnAppear() { }


    IEnumerator Disappear()
    {
        Debug.Assert(state == State.ALIVE);
        state = State.DISAPPEAR;
        coll.enabled = false;
        isFreeze = false;

        GameObject.Find("GameManager").GetComponent<GameManager>().AddScore(100);
        
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
        
        DestroyImmediate(transform.parent.gameObject);
        //Debug.Log("dead");

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Magic") return;

        var m = other.GetComponent<Magic>();
        Damage(m);
    }

    public void Damage(Magic m)
    {
        hp -= m.damage;
        if(m.arche == Magic.Arche.FROST) isFreeze = true;
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
