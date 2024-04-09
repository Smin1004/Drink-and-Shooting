using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet_Base : MonoBehaviour
{
    public SpriteRenderer me;

    float cur_lifeTime;
    [SerializeField] protected float lifeTime;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float damage;

    [SerializeField] protected bool isStop;
    [SerializeField] bool isHit;

    public void Stop(bool _isStop){
        isStop = _isStop;
    }

    public void Init(float _lifeTime, float _moveSpeed, float _damage)
    {
        lifeTime = _lifeTime;
        moveSpeed = _moveSpeed;
        damage = _damage;
    }

    public void Init(float _lifeTime, float _moveSpeed)
    {
        lifeTime = _lifeTime;
        moveSpeed = _moveSpeed;
    }

    protected virtual void Update()
    {
        cur_lifeTime += Time.deltaTime;
        if (cur_lifeTime > lifeTime) Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D hit) {
        if (!isHit && hit.collider.TryGetComponent<Enemy_Base>(out var e_hit))
        {
            e_hit.Enemy_Damage(damage);
            Hit_Event();
        }
        else if (isHit && hit.collider.TryGetComponent<Player>(out var p_hit) && !p_hit.isDash)
        {
            Player.Instance.Damage();
            Hit_Event();
        }
        else if (hit.collider.CompareTag("Wall"))
        {
            Hit_Wall(hit);
        }
    }

    protected abstract void Hit_Event();
    protected abstract void Hit_Wall(Collision2D hit);
}
