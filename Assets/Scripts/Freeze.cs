using UnityEngine;
using System.Collections;

public class Freeze : MonoBehaviour {

    private Enemy e;

	void Start () {
        e = transform.parent.gameObject.GetComponent<Enemy>();
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Assert(other.tag == "Magic");
        var m = other.GetComponent<Magic>();
        e.Damage(m);
    }
}
