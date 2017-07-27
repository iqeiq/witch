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

    public float damage = 1f;
    public Magic.Arche arche = Magic.Arche.AQUA;
    public Magic.Type type = Magic.Type.NORMAL;
    public int[] buffidxs = { };


    void Shot(GameManager gm)
    {
        var pos = transform.position;
        var sp = 2f;
        var dmg = damage;
        Magic.Arche ar = arche;
        Magic.Type ty = type;

        var dr = buffidxs.Average(i => gm.buff[i]); // TODO;
        dmg *= dr;

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
        var gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        Observable
            .Interval(TimeSpan.FromSeconds(interval))
            .Take(count)
            .Subscribe(x => {
                Shot(gm);
            }, () => {
                Disappear();
            });
    }
	
}
