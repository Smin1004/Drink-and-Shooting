using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Double_Shotgun_Enemy : A_Unit
{
    [Header("Double_Shotgun_Enemy")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private int dir_ran_min;
    [SerializeField] private int dir_ran_max;
    [SerializeField] private float fire_delay;
    [SerializeField] private float bullet_speed;
    [SerializeField] private int one_bullet_count;
    [SerializeField] private int two_bullet_count;
    [SerializeField] private float bullet_central;
    [SerializeField] private float bullet_delay;
    private float cur_delay;
    bool isfire;

    protected override void Update()
    {
        base.Update();
        cur_delay += Time.deltaTime;

        if (cur_delay >= fire_delay && isPlayerCheak && !isfire)
        {
            isfire = true;
            StartCoroutine(fire());
        }
    }

    IEnumerator fire()
    {
        anim.SetBool("isAttack", true);
        yield return new WaitForSeconds(1);
        anim.SetBool("isAttack", false);
        Vector2 nor = direction.normalized;
        int dir_ran = Random.Range(dir_ran_min, dir_ran_max + 1);
        float tarZ = Mathf.Atan2(nor.y, nor.x) * Mathf.Rad2Deg;

        float amount = bullet_central / (one_bullet_count - 1);
        float z = bullet_central / -2f + (int)tarZ;

        for (int i = 0; i < one_bullet_count; i++)
        {
            Quaternion rot = Quaternion.Euler(0, 0, z - 90f + dir_ran);
            var temp = Instantiate(bullet, transform.position, rot).GetComponent<Bullet_Base>();
            temp.Init(10, bullet_speed);
            z += amount;
        }

        yield return new WaitForSeconds(bullet_delay);

        float _bullet_central = bullet_central -= 30;
        amount = _bullet_central / (two_bullet_count - 1);
        z = _bullet_central / -2f + (int)tarZ;

        for (int i = 0; i < two_bullet_count; i++)
        {
            Quaternion rot = Quaternion.Euler(0, 0, z - 90f + dir_ran);
            Instantiate(bullet, transform.position, rot);
            z += amount;
        }

        cur_delay = 0;
        isfire = false;
    }

    protected override void DieDestroy()
    {
        Instantiate(die_effect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
