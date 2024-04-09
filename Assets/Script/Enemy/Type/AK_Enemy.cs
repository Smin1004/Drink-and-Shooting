using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK_Enemy : A_Unit
{
    [Header("AK_Enemy")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private int dir_ran_min;
    [SerializeField] private int dir_ran_max;
    [SerializeField] private float fire_delay;
    [SerializeField] private float bullet_delay;
    [SerializeField] private float bullet_speed;
    [SerializeField] private int bullet_count;
    private float cur_delay;
    bool isFire;

    protected override void Update()
    {
        base.Update();
        cur_delay += Time.deltaTime;

        if (cur_delay >= fire_delay && isPlayerCheak && !isFire)
        {
            isFire = true;
            StartCoroutine(fire());
        }
    }

    IEnumerator fire()
    {
        anim.SetBool("isAttack", true);
        yield return new WaitForSeconds(1);
        anim.SetBool("isAttack", false);

        for (int i = 0; i < bullet_count; i++)
        {
            if(!isPlayerCheak) break;
            
            int dir_ran = Random.Range(dir_ran_min, dir_ran_max + 1);
            float z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0, 0, z - 90f + dir_ran);
            var temp = Instantiate(bullet, transform.position, rot).GetComponent<Bullet_Base>();
            temp.Init(10, bullet_speed);
            yield return new WaitForSeconds(bullet_delay);
        }

        cur_delay = 0;
        isFire = false;
    }

    protected override void DieDestroy()
    {
        Instantiate(die_effect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
