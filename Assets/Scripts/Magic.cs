using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class Magic : MonoBehaviour {

    public enum Type
    {
        NORMAL = 0,
        PENETRATE,  // holy
        DIFFUSE,    // explotion
        RATIO
    }

    public enum Arche
    {
        FLAME   = 0x01,
        WIND    = 0x02,
        AQUA    = 0x04,
        HOLY    = FLAME | WIND,
        DARK    = FLAME | AQUA,
        FROST   = WIND  | AQUA,
        VOID    = FLAME | WIND | AQUA
    }

    [SerializeField]
    private GameObject blast;


    private float speed = 5.0f;
    public float damage { get; private set; }
    public Type type { get; private set; }
    public Arche arche { get; private set; }


    ParticleSystem ps = null;
    Rigidbody2D rig = null;
    CircleCollider2D coll = null;

    float max_x;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        rig = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
        max_x = Camera.main.ViewportToWorldPoint(new Vector2(1.0f, 0.5f)).x;

    }

    public void Set(Arche ar = Arche.VOID, Type ty = Type.NORMAL, float sp = 5f, float dmg = 2f)
    {
        arche = ar;
        type = ty;
        speed = sp;
        damage = dmg;
        var r = (arche & Arche.FLAME) > 0 ? 1f : 0f;
        var g = (arche & Arche.WIND) > 0 ? 1f : 0f;
        var b = (arche & Arche.AQUA) > 0 ? 1f : 0f;
        g = Mathf.Min(1f, g + (b > 0.5f ? 0.4f : 0f));
        ps.startColor = new Color(r, g, b, 1f);

        this.OnTriggerEnter2DAsObservable().Take(1).Subscribe(other => {
            if (type != Type.DIFFUSE) return;

            speed = 0;
            var radius = coll.radius;
            var d = damage;

            var bl = Util.CreateAndGetComponent<RingObject>(blast, transform);
            bl.transform.localScale = new Vector3(0, 0, 0);

            StartCoroutine(Util.FrameTimer(500, (t) => {
                damage = d * 0.5f * (2 - t);
                coll.radius = radius + 2.5f * t;
                var blr = coll.radius;
                bl.transform.localScale = new Vector3(blr, blr, blr);
                bl.color.a = 1 - t;
            }, ()=> {
                Stop();
            }));
        });
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Enemy") return;
        
        if (type != Type.PENETRATE && type != Type.DIFFUSE)
            Stop();

    }

    void Stop()
    {
        coll.enabled = false;
        speed = 0;
        rig.velocity = Vector2.zero;
        ps.Stop();
    }

    // Update is called once per frame
    void FixedUpdate () {

        var dir = new Vector2(1f , 0f);
        //rig.AddForce(dir * speed);
        rig.velocity = dir * speed;

        if (transform.position.x > max_x && !ps.isStopped)
        {
            Stop();
        }
        if(ps.isStopped && ps.particleCount < 1)
        {
            Destroy(gameObject);
        }

    }
}
