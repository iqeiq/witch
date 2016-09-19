using UnityEngine;
using System.Collections;


public abstract class Character : MonoBehaviour {

    public float speed = 5;
    
    protected Vector2 size;
    protected SpriteRenderer render;

    Vector2 min;
    Vector2 max;


    // Use this for initialization
    void Start ()
    {
        render = GetComponent<SpriteRenderer>();
        size = render.bounds.size;
        
        min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0.1f));
        max = Camera.main.ViewportToWorldPoint(new Vector2(1, 0.9f));
        min += size / 2;
        max -= size / 2;

        Init();
    }

    abstract protected void Init();

	
    // Update is called once per frame
    protected void Update ()
    {
        

	}

    public void SetPosition(Vector2 pos)
    {
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    public void SetPosition(float rx, float ry)
    {
        SetPosition(new Vector2(rx, ry));
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
