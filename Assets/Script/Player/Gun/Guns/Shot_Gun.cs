using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot_Gun : TwoHand
{
    [Header("Shot_Gun")]
    [SerializeField] int radius;
    [SerializeField] int bullet_count;
    [SerializeField] float lifeTime;
    [SerializeField] int moveSpeedMin;
    [SerializeField] int moveSpeedMax;

    protected override void Shot()
    {
        if(cur_ammo == 0 || remain_ammo == 0) return;

        int dir_ran = Random.Range(dir_ran_min, dir_ran_max + 1);
        anim.Play("Shot_Gun_Fire");
        SoundManager.Instance.Sound(_fire, false, 1);
        for (int i = radius / 2 * -1 ; i < radius / 2; i += radius / bullet_count)
        {
            var temp = Instantiate(bullet, startpos.transform.position, Quaternion.Euler(0, 0, i + rot + dir_ran)).GetComponent<Bullet_Base>();
            temp.Init(lifeTime, Random.Range(moveSpeedMin, moveSpeedMax + 1), damage);
        }
        cur_ammo--;
        remain_ammo--;
        cur_bullet_delay = 0;
        _magazine.CurMagazin();
    }

    protected override void Click()
    {
        cur_bullet_delay += 0.1f;
    }
}
