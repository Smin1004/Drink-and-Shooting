using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce_Bullet : Bounce
{
    [Header("Bounce")]
    [SerializeField] float checkRadius;
    [SerializeField] float spinSpeed;

    protected override void Update()
    {
        base.Update();
        if(!isStop) transform.Translate(Vector3.up * Time.deltaTime * moveSpeed, Space.Self);
    }

    protected override void Hit_Event() {
        Destroy(gameObject);
    }

    protected override void Hit_Wall(Collision2D hit)
    {
        _Bounce(hit);
        bounce_count--;
        if (bounce_count < 0) Hit_Event();
    }
}
