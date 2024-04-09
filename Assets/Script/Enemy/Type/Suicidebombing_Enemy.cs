using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suicidebombing_Enemy : A_Unit
{
    [Header("Suicidebombing")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private int bullet_count;
    [SerializeField] private float boomDelay;
    [SerializeField] private float bullet_speed;
    bool isfire;

    protected override void Update()
    {
        base.Update();
        if(!isPlayerCheak) return;
        anim.SetBool("isAttack", true);
        isMove = false;
        if(!isfire){
            isfire = true;
            StartCoroutine(fire(boomDelay));
        }
    }

    IEnumerator fire(float timer)
    {   
        yield return new WaitForSeconds(timer);

        for (int i = 0; i < 360; i += 360 / bullet_count)
        {
            var temp = Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, i)).GetComponent<Bullet_Base>();
            temp.Init(10, bullet_speed);
        }

        DieDestroy();
    }

    protected override void DieDestroy()
    {
        Instantiate(die_effect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
