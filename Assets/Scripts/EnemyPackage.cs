using UnityEngine;
using System.Collections;

public class EnemyPackage : MonoBehaviour {

    private Enemy enemy = null;
    private RingObject healthbar = null;


	// Use this for initialization
	void Start () {
        enemy = GetComponentInChildren<Enemy>();
        healthbar = GetComponentInChildren<RingObject>();
        Debug.Assert(enemy != null);
        Debug.Assert(healthbar != null);
    }

    void UpdateHealthbar()
    {
        float ratio = enemy.hp / enemy.GetMaxHP();
        healthbar.fanAngle = Mathf.Lerp(0, 2 * Mathf.PI, ratio);
    }

    // Update is called once per frame
    void Update () {
        healthbar.transform.position = enemy.transform.position;
        UpdateHealthbar();
    }
}
