using UnityEngine;
using System.Collections;

public class Magic : MonoBehaviour {

    public float speed = 5.0f;
    public float damage = 2.0f;

    ParticleSystem ps = null;

    float max_x;

    // Use this for initialization
    void Start () {

        ps = GetComponent<ParticleSystem>();

        //var pos = new Vector2(0.5f, 0.5f);
        //transform.position = Camera.main.ViewportToWorldPoint(pos);

        max_x = Camera.main.ViewportToWorldPoint(new Vector2(1.0f, 0.5f)).x;

        //StartCoroutine("Logger");

    }

    IEnumerator Logger()
    {
        while (true)
        {
            Debug.LogFormat("particle: {0}", ps.particleCount);
            yield return new WaitForSeconds(0.5f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Enemy")
        {
            Debug.LogAssertion("n?");
            return;
        }
        Debug.LogFormat("hit {0}", other);
    }

    // Update is called once per frame
    void FixedUpdate () {

        var dir = new Vector2(1f , 0f);
        GetComponent<Rigidbody2D>().AddForce(dir * speed);

        if (transform.position.x > max_x && !ps.isStopped)
        {
            ps.Stop();
        }
        if(ps.isStopped && ps.particleCount < 1)
        {
            Destroy(gameObject);
        }

    }
}
