using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion_Bullet : Bullet_Base
{
    [Header("Explosion")]
    [SerializeField] GameObject boom;

    protected override void Update()
    {
        base.Update();
        if(!isStop) transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);
    }

    void Explosion(){
        var temp = Instantiate(boom, transform.position, Quaternion.identity);
    }

    protected override void Hit_Event()
    {
        Explosion();
        Destroy(gameObject);
    }

    protected override void Hit_Wall(Collision2D hit){
        Hit_Event();
    }
}
