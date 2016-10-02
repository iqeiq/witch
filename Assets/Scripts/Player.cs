using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using System;


public class Player : Character {

    enum MagicRune { A = 0x01, B = 0x02, C = 0x04, END }

    public GameObject magic;

    [SerializeField]
    private float interval = 300f;
    
    Dictionary<MagicRune, EmitRing> rings;

    override protected void Init()
    {
        SetPosition(0.25f, 0.5f);

        var dic = new Dictionary<MagicRune, string> {
            { MagicRune.A, "A" },
            { MagicRune.B, "B" },
            { MagicRune.C, "C" },
        };

        rings = new Dictionary<MagicRune, EmitRing>();

        foreach (var d in dic)
        {
            rings.Add(
                d.Key,
                GameObject.Find("ring" + d.Value).GetComponent<EmitRing>()
            );
        }

        
        var merged = dic.Select(
            d => this.InputAsObservable("chant " + d.Value).Select(_ => d.Key)
        ).Merge();

        var ringColor = new Dictionary<MagicRune, Color> {
            { MagicRune.A, new Color(1f, 0.25f, 0.25f, 1f) },
            { MagicRune.B, new Color(0.25f, 1f, 0.25f, 1f) },
            { MagicRune.C, new Color(0.25f, 0.25f, 1f, 1f) },
        };

        merged.Subscribe(x => {
            rings[x].Emit(ringColor[x], new Color(0.25f, 0.25f, 0.25f, 1f), interval);
        });
        
        merged.Buffer(
            merged.Throttle(TimeSpan.FromMilliseconds(interval)).Merge(
                this.InputAsObservable("Fire").Select(_ => MagicRune.END)
            )
        ).Where(
            x => x.Count > 0
        ).Subscribe(x => {
            //Debug.Log("" + string.Join(", ", x.Select(n => dic[n]).ToArray()));
            var pos = transform.position + new Vector3(size.x / 2, 0);
            var m = Util.CreateAndGetComponent<Magic>(magic, pos);
            SetMagic(m, x.ToArray());
        });

    }

    void SetMagic(Magic m, MagicRune[] runes)
    {
        Magic.Arche ar = Magic.Arche.VOID;
        Magic.Type ty = Magic.Type.NORMAL;
        var sp = 5f;
        var dmg = 2f;
        if (runes.Length == 1)
        {
            ar = (Magic.Arche)runes[0];
        }
        else if(runes.Length == 2)
        {
            ar = (Magic.Arche)(runes[0] | runes[1]);
            dmg = 3f;
            sp = 8f;
        }
        else if(runes.Length == 3)
        {

        }
        if (ar == Magic.Arche.HOLY) ty = Magic.Type.PENETRATE;
        m.Set(ar, ty, sp, dmg);
    }
    
    // Update is called once per frame
    new void Update ()
    {
        base.Update();
        Move(new Vector2(0, Input.GetAxis("Vertical")));
    }

}
