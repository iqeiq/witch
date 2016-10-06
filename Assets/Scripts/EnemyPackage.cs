using UnityEngine;
using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;


public class EnemyPackage : MonoBehaviour
{

    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private GameObject healthbarPrefab;

    [SerializeField]
    private GameObject damagePrefab;

    private Enemy enemy = null;
    private RingObject healthbar = null;
    private EmitRing damagebar = null;

    private float damageStart = 0f;
    private float damageEnd = 0f;


    void Awake()
    {
        enemy = Util.CreateAndGetComponent<Enemy>(enemyPrefab, transform);
        healthbar = Util.CreateAndGetComponent<RingObject>(healthbarPrefab, transform);
        damagebar = Util.CreateAndGetComponent<EmitRing>(damagePrefab, transform);
        Debug.Assert(enemy != null);
        Debug.Assert(healthbar != null);
        Debug.Assert(damagebar != null);

        damagebar.ring.fanAngle = 0f;
        damagebar.ring.enabled = false;
        damageStart = enemy.GetMaxHP();
        damageEnd = enemy.GetMaxHP();
    }

    void Start()
    {
        var hpChange = this.ToObservable(() => enemy.hp)
            .DistinctUntilChanged()
            .Pairwise()
            .Select(x => new Pair<float>(x.Previous, Mathf.Max(0f, x.Current)));

        hpChange.Subscribe(x =>{
            //Debug.Log(string.Format("{0} -> {1}", x.Previous, x.Current));
            damageStart = Mathf.Max(damageStart - (x.Previous - x.Current), 0f);
            damagebar.ring.enabled = true;
            UpdateDamagebar();
            var c = damagebar.ring.color;
            var from = new Color(c.r, c.g, c.b, 1f);
            var to = new Color(c.r, c.g, c.b, 0f);
            damagebar.Emit(from, to, 300);
        }).AddTo(damagebar);

        hpChange.Delay(
            TimeSpan.FromMilliseconds(500)
        ).Subscribe(x => {
            damageEnd = x.Current;
            UpdateDamagebar();
            if (damageEnd == damageStart) damagebar.enabled = false;
        }).AddTo(damagebar);


    }

    void UpdateHealthbar()
    {
        healthbar.enabled = (enemy.state == Enemy.State.ALIVE);

        float ratio = enemy.hp / enemy.GetMaxHP();
        healthbar.fanAngle = Mathf.Lerp(0, 2 * Mathf.PI, ratio);
        
    }

    void UpdateDamagebar()
    {
        float ratio = (damageEnd - damageStart) / enemy.GetMaxHP();
        damagebar.ring.fanAngle = Mathf.Lerp(0, 2 * Mathf.PI, ratio);

        var ang = Mathf.Lerp(0, 360, damageStart / enemy.GetMaxHP());
        var rot = damagebar.transform.rotation.eulerAngles;
        rot.z = 90f + ang;
        damagebar.transform.rotation = Quaternion.Euler(rot);

    }


    // Update is called once per frame
    void Update ()
    {
        healthbar.transform.position = enemy.transform.position;
        damagebar.transform.position = healthbar.transform.position;
        UpdateHealthbar();
    }
}
