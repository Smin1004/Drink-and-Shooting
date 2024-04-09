using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charging : TwoHand, ICharging
{
    [Header("Charging")]
    [SerializeField] float Charge_Time;
    float timer;
    [SerializeField] float lifeTime;
    [SerializeField] float moveSpeed;

    bool isMove;

    Vector3 orignal_pos;

    protected override void Start()
    {
        base.Start();
        orignal_pos = me.transform.localPosition;
    }

    protected override void Update()
    {
        base.Update();
        if (cur_bullet_delay >= max_bullet_delay - 0.2f) me.gameObject.SetActive(true);
    }

    protected override void Shot()
    {
        if(cur_ammo == 0 || remain_ammo == 0) return;

        if (timer > Charge_Time)
        {
            me.gameObject.SetActive(false);
            SoundManager.Instance.Sound(_fire, false, 1);
            float dir_ran = Random.Range(dir_ran_min, dir_ran_max + 1);
            var temp = Instantiate(bullet, startpos.transform.position, Quaternion.Euler(0, 0, rot + dir_ran)).GetComponent<Bullet_Base>();
            temp.Init(lifeTime, moveSpeed, damage);
            if(isRight) temp.me.flipY = true;
            else temp.me.flipY = false;

            cur_ammo--;
            remain_ammo--;
            cur_bullet_delay = 0;
            timer = 0;
            _magazine.CurMagazin();
        }
        timer += Time.deltaTime;
        StartCoroutine(Vibration());
    }

    protected override void Click()
    {
        timer = 0;
    }

    public void Setting(){
        isMove = false;
    }

    IEnumerator Vibration(){
        if(isMove) yield break;

        isMove = true;
        float ranx = Random.Range(-1, 2);
        if (ranx == 0) ranx = -1;
        float rany = Random.Range(-1, 2);
        if (rany == 0) rany = 1;

        me.transform.localPosition = me.transform.localPosition + new Vector3(ranx / 20, rany / 20);
        yield return new WaitForSeconds(0.01f);
        me.transform.localPosition = orignal_pos;
        isMove = false;
    }
}
