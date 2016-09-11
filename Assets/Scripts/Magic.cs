using UnityEngine;
using System.Collections;

public class Magic : MonoBehaviour {

    float theta = 0.0f;
    public float speed = 5.0f;
    float dist = 1.0f;

	// Use this for initialization
	void Start () {

        var pos = new Vector2(0.5f, 0.5f);
        transform.position = Camera.main.ViewportToWorldPoint(pos);

        var max = Camera.main.ViewportToWorldPoint(new Vector2(0.2f, 0.5f));
        dist = max.x - transform.position.x;

    }
	
	// Update is called once per frame
	void Update () {

        theta = (theta > 360 ? theta - 360 : theta) + speed;

        var pos = transform.position;
        pos.x = dist * Mathf.Sin(theta * Mathf.Deg2Rad);
        transform.position = pos;

	}
}
