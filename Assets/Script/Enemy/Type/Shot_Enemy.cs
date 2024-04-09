using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot_Enemy : A_Unit
{
    [Header("Shot_Enemy")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private int dir_ran_min;
    [SerializeField] private int dir_ran_max;
    [SerializeField] private float fire_delay;
    [SerializeField] private float bullet_speed;
    private float cur_delay;
    private bool isfire;

    protected override void Update()
    {
        base.Update();
        cur_delay += Time.deltaTime;

        if (cur_delay >= fire_delay && isPlayerCheak && !isfire) {
            isfire = true;
            StartCoroutine(fire());
        }
    }

    IEnumerator fire()
    {
        anim.SetBool("isAttack", true);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("isAttack", false);
        int dir_ran = Random.Range(dir_ran_min, dir_ran_max + 1);
        float z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, z - 90f + dir_ran);
        var temp = Instantiate(bullet, transform.position, rot).GetComponent<Bullet_Base>();
        temp.Init(10, bullet_speed);

        cur_delay = 0;
        isfire = false;
    }

    protected override void DieDestroy()
    {
        Instantiate(die_effect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
