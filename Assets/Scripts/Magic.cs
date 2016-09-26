using UnityEngine;


public class Magic : MonoBehaviour {

    public float speed = 5.0f;
    public float damage = 2.0f;

    ParticleSystem ps = null;
    Rigidbody2D rig = null;
    CircleCollider2D coll = null;

    float max_x;

    // Use this for initialization
    void Start () {
        ps = GetComponent<ParticleSystem>();
        rig = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
        max_x = Camera.main.ViewportToWorldPoint(new Vector2(1.0f, 0.5f)).x;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Assert(other.tag == "Enemy");
        //Debug.LogFormat("hit {0}", other);
        Stop();
    }

    void Stop()
    {
        coll.enabled = false;
        speed = 0;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        ps.Stop();
    }

    // Update is called once per frame
    void FixedUpdate () {

        var dir = new Vector2(1f , 0f);
        rig.AddForce(dir * speed);

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
