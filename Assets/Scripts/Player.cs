using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using System;
using Util;


public class Player : Character {

    public GameObject magic;

    enum MagicRune { A, B, C }

    override protected void Init()
    {
        SetPosition(0.25f, 0.5f);

        var dic = new Dictionary<MagicRune, string> {
            { MagicRune.A, "chant A" },
            { MagicRune.B, "chant B" },
            { MagicRune.C, "chant C" }
        };

        var merged = dic.Select(
            d => this.InputAsObservable(d.Value).Select(_ => d.Key)
        ).Merge();
        
        merged
            .Buffer(merged.Throttle(TimeSpan.FromMilliseconds(300)))
            .Where(x => x.Count > 0)
            .Subscribe(x => {
                Debug.Log("" + string.Join(", ", x.Select(m => dic[m]).ToArray()));    
            });

    }

    // Update is called once per frame
    new void Update ()
    {
        base.Update();

        var vy = Input.GetAxis("Vertical");
        Move(new Vector2(0, vy));

        var fire = Input.GetButtonDown("Fire");
        //var c1 = Input.GetButtonDown("chant A");
        //var c2 = Input.GetButtonDown("chant B");
        //var c3 = Input.GetButtonDown("chant C");

        if (fire)
        {
            var pos = transform.position + new Vector3(size.x / 2, 0);
            Instantiate(magic, pos, Quaternion.identity);
        }

    }

}
