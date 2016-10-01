using UnityEngine;
using System.Collections;


public class PlayerRing : MonoBehaviour
{

    RingObject ring;
    Coroutine coro;

	// Use this for initialization
	void Start ()
    {
        ring = GetComponent<RingObject>();
        coro = null;
	}

    public void Emit(Color from, Color to, float interval)
    {
        if (coro != null) StopCoroutine(coro);
        coro = StartCoroutine(CalmDown(from, to, interval));
    }

    IEnumerator CalmDown(Color from, Color to, float interval)
    {
        var sec = interval / 1000f;
        var wait = 8 / 1000f;
        var div = sec / wait;
        ring.color = from;
        for (var i = 0; i < div; ++i)
        {
            ring.color.r = Mathf.Lerp(from.r, to.r, i / div);
            ring.color.g = Mathf.Lerp(from.g, to.g, i / div);
            ring.color.b = Mathf.Lerp(from.b, to.b, i / div);
            ring.color.a = Mathf.Lerp(from.a, to.a, i / div);
            yield return new WaitForSeconds(wait);
        }
        ring.color = to;
    }
	
	
}
