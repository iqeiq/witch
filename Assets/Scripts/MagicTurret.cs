using UnityEngine;
using System.Collections;
using UniRx;
using System;
using System.Linq;


public class MagicTurret : MonoBehaviour
{

    [SerializeField]
    private GameObject magicPrefab;

    [SerializeField]
    private float interval = 2f;

    [SerializeField]
    private int count = 5;


    void Shot()
    {
        var pos = transform.position;
        var sp = 5f;
        var dmg = 1f;
        Magic.Arche ar = Magic.Arche.AQUA;
        Magic.Type ty = Magic.Type.NORMAL;

        var m = Util.CreateAndGetComponent<Magic>(magicPrefab, pos);
        m.Set(ar, ty, sp, dmg);
    }

    void Disappear()
    {
        var rings = GetComponentsInChildren<RingObject>();
        var dic = rings.ToDictionary(r => r.transform.localScale.x);
        
        StartCoroutine(Util.FrameTimer(1000, (t) => {
            dic.ToList().ForEach(p => {
                var s = (1 - t) * p.Key;
                p.Value.transform.localScale = new Vector3(s, s, s);
            });
            
        }, () => {
            Destroy(gameObject);
        }));
    }


	// Use this for initialization
	void Start ()
    {
        Observable
            .Interval(TimeSpan.FromSeconds(interval))
            .Take(count)
            .Subscribe(x => {
                Shot();
            }, () => {
                Disappear();
            });
    }
	
}
