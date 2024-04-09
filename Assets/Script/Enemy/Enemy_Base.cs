using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy_Base : MonoBehaviour
{
    [Header("Enemy_Base")]
    protected Transform target;
    [SerializeField] protected GameObject die_effect;

    [SerializeField] float HP;
    [SerializeField] float maxHP;
    [SerializeField] protected float fireRange;

    protected Vector2 direction;
    [SerializeField] protected bool isPlayerCheak;
    bool isDie;

    public Action dieAction;
    protected Animator anim;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        HP = maxHP;
        dieAction += DieDestroy;
    }

    protected virtual void Update()
    {
        target = Player.Instance.transform;
        direction = Player.Instance.transform.position - transform.position;

        var hits = Physics2D.RaycastAll(transform.position, direction, fireRange);
        foreach (var item in hits)
        {
            if(item.collider.CompareTag("Wall")){
                isPlayerCheak = false;
                break;
            }
            else if (item.collider.CompareTag("Player"))
            {
                isPlayerCheak = true;
                if(!gameObject.TryGetComponent<IItme>(out var isItem)) GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
                break;
            }
        }
    }

    public virtual void Enemy_Damage(float Damage)
    {
        HP -= Damage;

        if (HP <= 0)
        {
            if(isDie) return;
            isDie = true;

            dieAction?.Invoke();
        }
    }

    protected abstract void DieDestroy();
}
