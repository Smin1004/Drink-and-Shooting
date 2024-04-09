using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGun : TwoHand
{
    [Header("SpawnGun")]
    [SerializeField] float lifeTime;
    [SerializeField] float moveSpeed;

    protected override void Shot()
    {
        if(cur_ammo == 0 || remain_ammo == 0) return;

        int dir_ran = Random.Range(dir_ran_min, dir_ran_max + 1);
        var temp = Instantiate(bullet, startpos.transform.position, Quaternion.Euler(0, 0, rot + dir_ran)).GetComponent<Bullet_Base>();
        temp.Init(lifeTime, moveSpeed, damage);
        cur_ammo--;
        remain_ammo--;
        _magazine.CurMagazin();
        cur_bullet_delay = 0;
    }

    protected override void Click()
    {
        
    }
}
