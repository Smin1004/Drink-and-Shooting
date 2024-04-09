using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class A_Unit : Enemy_Base
{
    [Header("A_STAR")]
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected bool isMove;
    [SerializeField] float playerCheakRange;

    List<A_Node> path;

    bool isEnd = true;

    int targetIndex;

    protected override void Start() {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (isEnd && isMove)
            A_Manager.Requset(transform.position, target.position, Found);
    }

    void Found(List<A_Node> newPath, bool success)
    {
        isEnd = false;
        path = newPath;
        //StopAllCoroutines();
        StartCoroutine(Follow());
    }

    IEnumerator Follow()
    {
        Vector3 dir = Player.Instance.transform.position - transform.position;

        var hits = Physics2D.RaycastAll(transform.position, dir, playerCheakRange);
        foreach (var item in hits)
        {
            if(item.collider.CompareTag("Wall"))
                break;
            
            else if (item.collider.CompareTag("Player"))
            {
                isEnd = true;
                anim.SetBool("isMove", false);
                yield break;
            }
        }

        A_Node curWayPoint = path[0];
        targetIndex = 0;

        if (transform.position == curWayPoint.pos)
        {
            targetIndex++;
            if (targetIndex >= path.Count - 1)
            {
                yield break;
            }
            curWayPoint = path[targetIndex];
        }
        anim.SetBool("isMove", true);
        yield return StartCoroutine(MoveTo(curWayPoint.pos, 0.5f));
        isEnd = true;
        yield break;
    }

    private IEnumerator MoveTo(Vector3 target, float sec)
    {
        float timer = 0f;
        Vector3 start = transform.position;

        while (timer <= sec)
        {
            if(!isMove) yield break;

            transform.position = Vector3.Lerp(start, target, timer / sec);
            timer += Time.deltaTime * moveSpeed;

            yield return null;
        }

        yield break;
    }
}
