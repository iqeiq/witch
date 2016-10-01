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
    private RingObject damagebar = null;

    private float damageStart = 0f;
    private float damageEnd = 0f;
    private float damageTimer = 1f;

    void Awake()
    {
        enemy = Util.CreateAndGetComponent<Enemy>(enemyPrefab, transform);
        healthbar = Util.CreateAndGetComponent<RingObject>(healthbarPrefab, transform);
        damagebar = Util.CreateAndGetComponent<RingObject>(damagePrefab, transform);
        Debug.Assert(enemy != null);
        Debug.Assert(healthbar != null);
        Debug.Assert(damagebar != null);

        damagebar.fanAngle = 0f;
        damagebar.enabled = false;
        damageStart = enemy.GetMaxHP();
        damageEnd = enemy.GetMaxHP();

        var hpChange = this.UpdateAsObservable()
            .Select(_ => enemy.hp)
            .DistinctUntilChanged()
            .Pairwise()
            .Select(x => new Pair<float>(Mathf.Max(0f, x.Previous), Mathf.Max(0f, x.Current)));

        hpChange.Subscribe(x =>{
            //Debug.Log(string.Format("{0} -> {1}", x.Previous, x.Current));
            damageStart = Mathf.Max(damageStart - (x.Previous - x.Current), 0f);
            damagebar.enabled = true;
            damageTimer = 1f;
            UpdateDamagebar();
        }).AddTo(damagebar);

        hpChange.Delay(TimeSpan.FromMilliseconds(500)).Subscribe(x => {
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

        damagebar.color.a = Mathf.Lerp(0, 1f, damageTimer);
        damageTimer = Mathf.Max(0f, damageTimer - 0.05f);
    }

    void UpdateDamagebar()
    {
        //damagebar.color.a = 0.25f;

        float ratio = (damageEnd - damageStart) / enemy.GetMaxHP();
        damagebar.fanAngle = Mathf.Lerp(0, 2 * Mathf.PI, ratio);

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
