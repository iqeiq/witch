using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    Rigidbody2D rig;
    Vector2 size;

    // Use this for initialization
    void Start () {
        rig = GetComponent<Rigidbody2D>();
        var sr = GetComponent<SpriteRenderer>();
        size = sr.bounds.size;

    }
	
	// Update is called once per frame
	void Update () {
        Move();
	}

    void Move()
    {
        var vy = Input.GetAxis("Vertical");
        rig.AddForce(new Vector2(0, vy), ForceMode2D.Force);

        Clamp();

    }

    void Clamp()
    {
        // 画面左下,右上のワールド座標をビューポートから取得
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(0, 1));

        Vector2 pos = transform.position;
        min.y += size.y / 2;
        max.y -= size.y / 2;

        var top = pos.y < min.y;
        var bottom = pos.y > max.y;
        
        // Mathf.Clamp(pos.y, min.y, max.y);
        if (top || bottom)
        {
            rig.velocity = new Vector2(0, 0);
            pos.y = top ? min.y : max.y;
            transform.position = pos;
        }
    }

}
