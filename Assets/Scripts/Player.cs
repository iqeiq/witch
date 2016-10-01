using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using System;


public class Player : Character {

    enum MagicRune { A, B, C, END }

    public GameObject magic;

    [SerializeField]
    private float interval = 300f;
    
    Dictionary<MagicRune, PlayerRing> rings;

    override protected void Init()
    {
        SetPosition(0.25f, 0.5f);

        var dic = new Dictionary<MagicRune, string> {
            { MagicRune.A, "A" },
            { MagicRune.B, "B" },
            { MagicRune.C, "C" },
        };

        rings = new Dictionary<MagicRune, PlayerRing>();

        foreach (var d in dic)
        {
            rings.Add(
                d.Key,
                GameObject.Find("ring" + d.Value).GetComponent<PlayerRing>()
            );
        }

        
        var merged = dic.Select(
            d => this.InputAsObservable("chant " + d.Value).Select(_ => d.Key)
        ).Merge();

        merged.Subscribe(x => {
            var from = new Color(0.25f, 1f, 0.25f, 1f);
            var to = new Color(0.25f, 0.25f, 0.25f, 1f);
            rings[x].Emit(from, to, interval - 100);
        });
        
        var trigger = merged.Throttle(TimeSpan.FromMilliseconds(interval)).Merge(
             this.InputAsObservable("Fire").Select(_ => MagicRune.END)
        );

        merged.Buffer(trigger).Where(x => x.Count > 0).Subscribe(x => {
            Debug.Log("" + string.Join(", ", x.Select(m => dic[m]).ToArray()));
            var pos = transform.position + new Vector3(size.x / 2, 0);
            Util.CreateAndGetComponent<Magic>(magic, pos);
        });

    }
    
    // Update is called once per frame
    new void Update ()
    {
        base.Update();
        Move(new Vector2(0, Input.GetAxis("Vertical")));
    }

}
