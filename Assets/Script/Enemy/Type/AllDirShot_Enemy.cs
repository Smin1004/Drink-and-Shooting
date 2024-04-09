using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllDirShot_Enemy : A_Unit
{
    [Header("Clrcleshot")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private float fire_delay;
    [SerializeField] private float bullet_speed;
    [SerializeField] private int bullet_count;
    float cur_delay;
    bool isFire;

    protected override void Start()
    {
        base.Start();
        moveSpeed /= 10;
    }

    protected override void Update()
    {
        base.Update();
        cur_delay += Time.deltaTime;

        if (cur_delay >= fire_delay && isPlayerCheak && !isFire) {
            isFire = true;
            StartCoroutine(fire());
        }
    }

    IEnumerator fire()
    {
        anim.SetBool("isAttack", true);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("isAttack", false);
        
        for (int i = 0; i < 360; i += 360 / bullet_count)
        {
            var temp = Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, i)).GetComponent<Bullet_Base>();
            temp.Init(10, bullet_speed);
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
