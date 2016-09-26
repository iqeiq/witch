using UnityEngine;


public class Bat : Enemy
{

    public const int maxhp = 2;
    float th1, th2;

    protected override void InitEnemy()
    {
        hp = maxhp;
        th1 = th2 = 0f;
    }


    protected override void ActionEnemy()
    {
        th1 = (th1 > 360 ? th1 - 360 : th1) + 5;
        th2 = (th2 > 360 ? th2 - 360 : th2) + 6.11f;

        var y = 0.2f * Mathf.Sin(th1 * Mathf.Deg2Rad)
            + 0.18f * Mathf.Sin(th2 * Mathf.Deg2Rad)
            + base_y;

        Move(new Vector2(0, y - prev_y));
    }
}
