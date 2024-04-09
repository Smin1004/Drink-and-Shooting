using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float size;
    float cur_timer;
    [SerializeField] float max_timer;

    private void Start()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, size);
        foreach (var item in hits)
        {
            if (item.TryGetComponent<Enemy_Base>(out var e_hit))
            {
                e_hit.Enemy_Damage(10);
            }
            else if (item.TryGetComponent<Player>(out var p_hit))
            {
                //플레이어 데미지 1 ro 2
            }
        }
    }

    void Update()
    {
        if (cur_timer >= max_timer)
        {
            Destroy(gameObject);
        }

        cur_timer += Time.deltaTime;
    }
}
