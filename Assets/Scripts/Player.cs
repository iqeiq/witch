using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using System;


public class Player : Character {

    enum MagicRune { A = 0x01, B = 0x02, C = 0x04, END = 0xFF }

    [SerializeField]
    private GameObject[] magics;

    [SerializeField]
    private float interval = 300f;

    Dictionary<MagicRune, EmitRing> rings;

    public string lastRunes = "";

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

        var gm = GameObject.Find("GameManager").GetComponent<GameManager>();

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
            lastRunes = x.Select(r => dic[r][0]).Aggregate("", (a, b) => a + b);
            //Debug.Log(x.Count);
            //Debug.Log("" + string.Join(", ", x.Select(n => dic[n]).ToArray()));
            SetMagic(x.ToArray(), gm);
        });

    }

    void SetMagic(MagicRune[] runes, GameManager gm)
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
                dmg = 0.5f;
                sp = 8f;
                ty = Magic.Type.PENETRATE;
            }
            else if (ar == Magic.Arche.DARK)
            {
                var g = Instantiate(magics[idx]) as GameObject;
                g.transform.position = pos;
                var mt = g.GetComponent<MagicTurret>();
                mt.damage = 0.8f;
                mt.arche = Magic.Arche.DARK;
                mt.type = Magic.Type.RATIO;
                mt.buffidxs = new int[] { 0, 2 };
                return;
            }
            else if (ar == Magic.Arche.WIND)
            {
                dmg = 4.5f;
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
                var mt = g.GetComponent<MagicTurret>();
                mt.damage = 1;
                mt.arche = Magic.Arche.AQUA;
                mt.type = Magic.Type.NORMAL;
                mt.buffidxs = new int[] { 2 };
                return;
            }

        }
        else if (runes.Length == 3)
        {
            gm.buff[0] = Mathf.Pow(1.2f, runes.Count((v) => v == MagicRune.A));
            gm.buff[1] = Mathf.Pow(1.2f, runes.Count((v) => v == MagicRune.B));
            gm.buff[2] = Mathf.Pow(1.2f, runes.Count((v) => v == MagicRune.C));
            gm.UpdateBuff();
            return;
        }
        else
        {
            dmg = 1;
            sp = 3f;
        }
        var runemap = new Dictionary<MagicRune, int> {
            { MagicRune.A, 0 },
            { MagicRune.B, 1 },
            { MagicRune.C, 2 },
        };
        var dr = runes.Average(r => gm.buff[runemap[r]]);
        dmg *= dr;
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
