using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using System;


public class Player : Character {

    enum MagicRune { A = 0x01, B = 0x02, C = 0x04, END }

    [SerializeField]
    private GameObject[] magics;

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
            { MagicRune.A, new Color(1.7f, 0.16f, 0.16f, 1f) },
            { MagicRune.B, new Color(0.16f, 1.7f, 0.16f, 1f) },
            { MagicRune.C, new Color(0.16f, 0.16f, 1.7f, 1f) },
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
            //Debug.Log(x.Count);
            //Debug.Log("" + string.Join(", ", x.Select(n => dic[n]).ToArray()));
            SetMagic(x.ToArray());
        });

    }

    void SetMagic(MagicRune[] runes)
    {
        var pos = transform.position + new Vector3(size.x / 2, 0);
        
        Magic.Arche ar = Magic.Arche.VOID;
        Magic.Type ty = Magic.Type.NORMAL;
        var sp = 5f;
        var dmg = 2f;
        var idx = 0;

        if (runes.Length == 1)
        {
            ar = (Magic.Arche)runes[0];
        }
        else if (runes.Length == 2)
        {
            ar = (Magic.Arche)(runes[0] | runes[1]);
            sp = 8f;
            idx = (int)ar;

            if (ar == Magic.Arche.HOLY)
            {
                ty = Magic.Type.PENETRATE;
                sp = 6f;
            }
            else if (ar == Magic.Arche.FROST)
            {
                dmg = 1f;
                sp = 8f;
            }
            else if (ar == Magic.Arche.DARK)
            {
                dmg = 0.8f;
                sp = 6f;
                ty = Magic.Type.RATIO;
            }
            else if (ar == Magic.Arche.WIND)
            {
                dmg = 4f;
                sp = 10f;
            }
            else if (ar == Magic.Arche.FLAME)
            {
                sp = 6f;
                ty = Magic.Type.DIFFUSE;
            }
            else if (ar == Magic.Arche.AQUA)
            {
                var g = Instantiate(magics[idx]) as GameObject;
                g.transform.position = pos;
                return;
            }

        }
        else if (runes.Length == 3)
        {

            return;
        }
        else
        {
            dmg = 1;
            sp = 3f;
        }
        var m = Util.CreateAndGetComponent<Magic>(magics[idx], pos);
        m.Set(ar, ty, sp, dmg);
    }
    
    // Update is called once per frame
    new void FixedUpdate()
    {
        base.FixedUpdate();
        Move(new Vector2(0, Input.GetAxis("Vertical")));
    }

}
