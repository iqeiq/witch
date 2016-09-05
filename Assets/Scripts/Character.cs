using UnityEngine;
using System.Collections;


public abstract class Character : MonoBehaviour {

    public float speed = 5;
    
    protected Vector2 size;

    Vector2 min;
    Vector2 max;


    // Use this for initialization
    protected void Start ()
    {
        var sr = GetComponent<SpriteRenderer>();
        size = sr.bounds.size;

        min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        min += size / 2;
        max -= size / 2;
    }

    // Update is called once per frame
    protected void Update ()
    {
        

	}

    protected void SetPosition(Vector2 pos)
    {
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }
    
    protected void Move(Vector2 dir)
    {
        Vector2 pos = transform.position;

        pos += dir * speed * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y, max.y);

        transform.position = pos;
    }

}
