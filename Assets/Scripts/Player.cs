using UnityEngine;
using System.Collections;


public class Player : Character {


    override protected void Init()
    {
        SetPosition(0.25f, 0.5f);
    }

    // Update is called once per frame
    new void Update ()
    {
        base.Update();

        var vy = Input.GetAxis("Vertical");
        Move(new Vector2(0, vy));

        //var fire = Input.GetButtonDown("Fire");
        //var c1 = Input.GetButtonDown("chant A");
        //var c2 = Input.GetButtonDown("chant B");
        //var c3 = Input.GetButtonDown("chant C");

    }

}
