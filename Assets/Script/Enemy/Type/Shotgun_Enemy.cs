using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun_Enemy : A_Unit
{
    [Header("Shotgun_Enemy")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private int dir_ran_min;
    [SerializeField] private int dir_ran_max;
    [SerializeField] private float fire_delay;
    [SerializeField] private float bullet_speed;
    [SerializeField] private float bullet_central;
    [SerializeField] private int bullet_count;
    private float cur_delay;
    private bool isfire;

    protected override void Update()
    {
        base.Update();
        cur_delay += Time.deltaTime;

        if (cur_delay >= fire_delay && isPlayerCheak && !isfire) {
            isfire = true;
            StartCoroutine(fire(bullet_count, bullet_central));
        }
    }


    IEnumerator fire(int count, float central)
    {
        anim.SetBool("isAttack", true);
        yield return new WaitForSeconds(1);
        anim.SetBool("isAttack", false);
        Vector2 nor = direction.normalized;
        int dir_ran = Random.Range(dir_ran_min, dir_ran_max + 1);
        float tarZ = Mathf.Atan2(nor.y, nor.x) * Mathf.Rad2Deg;

        float amount = central / (count - 1);
        float z = central / -2f + (int)tarZ;

        for (int i = 0; i < count; i++)
        {
            Quaternion rot = Quaternion.Euler(0, 0, z - 90f + dir_ran);
            var temp = Instantiate(bullet, transform.position, rot).GetComponent<Bullet_Base>();
            temp.Init(10, bullet_speed);
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
