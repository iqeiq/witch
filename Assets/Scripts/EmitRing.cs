using UnityEngine;
using System;
using System.Collections;


public class EmitRing : MonoBehaviour
{

    public RingObject ring;
    Coroutine coro;

	// Use this for initialization
	void Awake ()
    {
        if(ring == null) ring = GetComponent<RingObject>();
        coro = null;
	}

    public void Emit(Color from, Color to, float interval)
    {
        if (coro != null) StopCoroutine(coro);
        
        ring.color = from;
        coro = StartCoroutine(Util.FrameTimer(interval, (t) => {
            ring.color.r = Mathf.Lerp(from.r, to.r, t);
            ring.color.g = Mathf.Lerp(from.g, to.g, t);
            ring.color.b = Mathf.Lerp(from.b, to.b, t);
            ring.color.a = Mathf.Lerp(from.a, to.a, t);
        }, () => {
            ring.color = to;
        }));
    }

}
