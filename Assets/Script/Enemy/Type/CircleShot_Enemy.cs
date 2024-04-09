using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleShot_Enemy : RandomMove
{
    [Header("Clrcleshot")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private float fire_delay;
    [SerializeField] private float bullet_speed;
    [SerializeField] private float move_delay;
    [SerializeField] private float radius;
    [SerializeField] private int bullet_count;
    [SerializeField] private int spinSpeed;
    float cur_delay;
    float cur_Move_delay;

    bool isfire;

    protected override void Update()
    {
        base.Update();
        cur_delay += Time.deltaTime;
        if(!isfire) cur_Move_delay += Time.deltaTime;

        if(cur_Move_delay >= move_delay){
            cur_delay = 0;
            cur_Move_delay = 0;
            isfire = false;
            RandomPosCheak();
        }

        if (cur_delay >= fire_delay && isPlayerCheak && !isfire)
        {
            isfire = true;
            anim.SetBool("isAttack", true);
            StartCoroutine(fire());
        }
    }

    IEnumerator fire()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 360; i += 360 / bullet_count)
        {
            Vector3 pos = transform.position + new Vector3(Mathf.Cos(i * Mathf.Deg2Rad) * radius, Mathf.Sign(i * Mathf.Rad2Deg) * radius);
            Vector2 nor = (target.position - transform.position).normalized;

            var temp = Instantiate(bullet, pos, Quaternion.identity).GetComponent<Spin>();
            temp.Init(10, bullet_speed);
            temp.InitSpin(transform.position, nor, spinSpeed, i, radius);
        }

        yield return new WaitForSeconds(0.3f);
        anim.SetBool("isAttack", false);
        yield return new WaitForSeconds(1);
        RandomPosCheak();

        cur_delay = 0;
        cur_Move_delay = 0;
        isfire = false;
    }

    protected override void DieDestroy()
    {
        Instantiate(die_effect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
