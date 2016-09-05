using UnityEngine;
using System.Collections;


public class Player : Character {

    // Use this for initialization
    new void Start ()
    {
        base.Start();

        Init();
    }

    void Init()
    {
        speed = 5;
        SetPosition(new Vector2(0.1f, 0.5f));
    }

    // Update is called once per frame
    new void Update ()
    {
        base.Update();

        var vy = Input.GetAxis("Vertical");
        Move(new Vector2(0, vy));
    }

}
