using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using System;


public class Player : Character {

    public GameObject magic;

    enum MagicRune { A, B, C, END }

    override protected void Init()
    {
        SetPosition(0.25f, 0.5f);

        var dic = new Dictionary<MagicRune, string> {
            { MagicRune.A, "chant A" },
            { MagicRune.B, "chant B" },
            { MagicRune.C, "chant C" },
        };

        var merged = dic.Select(
            d => this.InputAsObservable(d.Value).Select(_ => d.Key)
        ).Merge();

        var trigger = merged.Throttle(TimeSpan.FromMilliseconds(300)).Merge(
             this.InputAsObservable("Fire").Select(_ => MagicRune.END)
        );

        merged.Buffer(trigger).Where(x => x.Count > 0).Subscribe(x => {
            Debug.Log("" + string.Join(", ", x.Select(m => dic[m]).ToArray()));
            var pos = transform.position + new Vector3(size.x / 2, 0);
            Instantiate(magic, pos, Quaternion.identity);
        });

    }

    // Update is called once per frame
    new void Update ()
    {
        base.Update();
        Move(new Vector2(0, Input.GetAxis("Vertical")));
    }

}
