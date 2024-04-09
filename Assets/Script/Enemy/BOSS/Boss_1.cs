using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_1 : Enemy_Base
{
    [Header("BOSS")]
    [Header("OBJ")]
    [SerializeField] private GameObject line;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject spin_bullet;
    [SerializeField] private GameObject bezier_bullet;
    [SerializeField] private Transform StartPos;
    [SerializeField] private AudioClip bossBGM;
    [SerializeField] private AudioClip bgm;

    [Header("Stats")]
    [SerializeField] private int pattern = 99;
    [SerializeField] private float fire_delay;
    float cur_delay;

    public bool isintro;
    bool isfire;

    [Header("Spin_Shot")]
    [SerializeField] private int startRadius;
    [SerializeField] private int spin_ShotCount;
    [SerializeField] private int spin_bulletCount;
    [SerializeField] private float spin_spinSpeed;
    [SerializeField] private float spin_sizeUpSpeed;
    [SerializeField] private float spin_shotDelay;

    [Header("Bezier_Shot")]
    [SerializeField] private Transform bezier_Pos;
    [SerializeField] private int bezier_bulletCount;
    [SerializeField] private float bezier_moveSpeed;
    [SerializeField] private float bezier_minRadius;
    [SerializeField] private float bezier_maxRadius;
    [SerializeField] private float bezier_maxMoveValue;

    [Header("Speed")]
    [SerializeField] private int speed_shotCount;
    [SerializeField] private int speed_bulletCount;
    [SerializeField] private int speed_angle;
    [SerializeField] private float speed_shotWaitTime;
    [SerializeField] private float speed_doubleShotWait;
    [SerializeField] private float speed_waitTime;
    [SerializeField] private float speed_moveSpeed;

    [Header("AllDir")]
    [SerializeField] private int allDir_shotCount;
    [SerializeField] private int allDir_spinCount;
    [SerializeField] private int allDir_bulletCount;
    [SerializeField] private int allDir_moveSpeed;
    [SerializeField] private int allDir_plusAngle;
    [SerializeField] private float allDir_waitTime;

    protected override void Start()
    {
        base.Start();
        SoundManager.Instance.Sound(bossBGM, true, 0.115f);
    }

    protected override void Update()
    {
        if (isintro) return;
        base.Update();

        if (!isfire) cur_delay += Time.deltaTime;

        if (cur_delay >= fire_delay && !isfire)
        {
            isfire = true;
            while(true){
                var ranPattern = Random.Range(0, 4);
                if(pattern != ranPattern) {
                    pattern = ranPattern;
                    break;
                }
            }

            StartCoroutine(shot_pattern(pattern));
        }
    }

    protected override void DieDestroy()
    {
        Instantiate(die_effect, transform.position, Quaternion.identity);
        SoundManager.Instance.Sound(bgm, true, 0.115f);
        Destroy(gameObject);
    }

    IEnumerator shot_pattern(int pattern)
    {
        anim.SetBool("isAttack", true);
        switch (pattern)
        {
            case 0:
                yield return StartCoroutine(Spin());
                break;

            case 1:
                yield return StartCoroutine(Bezier());
                break;

            case 2:
                yield return StartCoroutine(Speed());
                break;

            case 3:
                yield return StartCoroutine(AllDir());
                break;
        }
        cur_delay = 0;
        isfire = false;
        anim.SetBool("isAttack", false);
    }

    IEnumerator Spin()
    {
        List<List<Spin>> CicleList = new List<List<Spin>>();
        for (int j = 1; j <= spin_ShotCount; j++)
        {
            List<Spin> Cicle = new List<Spin>();
            for (int i = 0; i < 360; i += 360 / spin_bulletCount)
            {
                Vector3 pos = transform.position + new Vector3(Mathf.Cos(i * Mathf.Deg2Rad) * startRadius, Mathf.Sign(i * Mathf.Rad2Deg) * startRadius);
                Vector2 nor = (target.position - transform.position).normalized;

                var temp = Instantiate(spin_bullet, pos, Quaternion.identity).GetComponent<Spin>();
                temp.Stop(true);
                if (j % 2 == 0) temp.InitSpin(transform.position, nor, spin_spinSpeed, i, startRadius);
                else temp.InitSpin(transform.position, nor, -spin_spinSpeed, i, startRadius);

                Cicle.Add(temp);
            }
            CicleList.Add(Cicle);
        }

        for (int i = 0; i < CicleList.Count; i++)
        {
            yield return new WaitForSeconds(spin_shotDelay);
            List<Spin> curCicle = CicleList[i];
            for (int j = 0; j < curCicle.Count; j++)
            {
                curCicle[j].InitSizeUP(spin_sizeUpSpeed);
            }
        }
    }

    IEnumerator Bezier()
    {
        List<Bezier_Bullet> bulletList = new List<Bezier_Bullet>();
        for (int i = 0; i < 360; i += 360 / bezier_bulletCount)
        {
            var temp = Instantiate(bezier_bullet, StartPos.position, Quaternion.identity).GetComponent<Bezier_Bullet>();
            temp.Init(99, bezier_moveSpeed);
            temp.InitBezier(StartPos.position, bezier_Pos.position, bezier_minRadius, bezier_maxRadius, bezier_maxMoveValue);
            temp.Stop(true);
            bulletList.Add(temp);
            yield return new WaitForSeconds(0.025f);
        }

        for (int i = 0; i < bulletList.Count; i++)
        {
            bulletList[i].InitBezier(bezier_Pos.position, target.position, bezier_minRadius, bezier_maxRadius, bezier_maxMoveValue);
            bulletList[i].Stop(false);
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator Speed()
    {
        for (int j = 0; j < speed_shotCount; j++)
        {
            List<Quaternion> bulletRot = new List<Quaternion>();
            List<Quaternion> bulletList = new List<Quaternion>();
            List<GameObject> lineList = new List<GameObject>();

            float z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            for (int i = speed_angle / 2 * -1; i < speed_angle / 2; i += speed_angle / (speed_bulletCount * 3))
            {
                Quaternion rot = Quaternion.Euler(0, 0, z - 90f + i);
                bulletRot.Add(rot);
            }

            for (int i = 0; i < speed_bulletCount; i++)
            {
                int ranRot = Random.Range(0, bulletRot.Count);
                if (i == 0) ranRot = 0;
                else if (i == speed_bulletCount - 1) ranRot = bulletRot.Count - 1;

                Quaternion rot = bulletRot[ranRot];
                bulletRot.Remove(bulletRot[ranRot]);
                bulletList.Add(rot);

                var temp = Instantiate(line, StartPos.position, rot);
                lineList.Add(temp);
            }

            yield return new WaitForSeconds(speed_shotWaitTime);
            for (int i = 0; i < lineList.Count; i++)
            {
                Destroy(lineList[i].gameObject);
            }
            lineList.Clear();

            for (int i = 0; i < bulletList.Count; i++)
            {
                var temp = Instantiate(bullet, StartPos.position, bulletList[i]).GetComponent<Bullet_Base>();
                temp.Init(5, speed_moveSpeed);
            }

            yield return new WaitForSeconds(speed_doubleShotWait);

            for (int i = 0; i < bulletList.Count; i++)
            {
                var temp = Instantiate(bullet, StartPos.position, bulletList[i]).GetComponent<Bullet_Base>();
                temp.Init(5, speed_moveSpeed);
            }

            yield return new WaitForSeconds(speed_waitTime);
        }
    }

    IEnumerator AllDir()
    {
        bool isReverse = false;
        for (int i = 0; i < allDir_shotCount; i++)
        {
            int plusZ = 0;

            if(i % (allDir_shotCount / allDir_spinCount) == 0 && isReverse) isReverse = false;
            else if(i % (allDir_shotCount / allDir_spinCount) == 0 && !isReverse) isReverse = true;

            if(!isReverse) plusZ = allDir_plusAngle * i;
            else plusZ = -allDir_plusAngle * i;

            for (int j = 0; j < 360; j += 360 / allDir_bulletCount)
            {
                var temp = Instantiate(bullet, StartPos.position, Quaternion.Euler(0, 0, j + plusZ)).GetComponent<Bullet_Base>();
                temp.Init(10, allDir_moveSpeed);
            }

            yield return new WaitForSeconds(allDir_waitTime);
        }
    }
}
