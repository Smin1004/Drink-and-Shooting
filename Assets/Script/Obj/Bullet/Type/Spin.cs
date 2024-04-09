using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : Bullet_Base
{
    Vector3 center;
    Vector2 dir;
    float rotateSpeed;
    float angle;
    float radius;
    bool isSizeUP;
    float sizeUpSpeed;

    public void InitSpin(Vector3 _center, Vector2 _dir, float _rotateSpeed, float _angle, float _radius){
        center = _center;
        dir = _dir;
        rotateSpeed = _rotateSpeed;
        angle = _angle;
        radius = _radius;
    }

    public void InitSizeUP(float _sizeUpSpeed){
        isSizeUP = true;
        sizeUpSpeed = _sizeUpSpeed;
    }

    protected override void Update()
    {
        base.Update();
        if(!isStop) center += (Vector3)dir * Time.deltaTime * moveSpeed;
        angle += rotateSpeed * Time.deltaTime;

        Vector3 pos = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sin(angle * Mathf.Deg2Rad) * radius);
        pos += center;
        if(isSizeUP) radius += Time.deltaTime * sizeUpSpeed;

        transform.position = pos;
    }

    protected override void Hit_Event()
    {
        Destroy(gameObject);
    }

    protected override void Hit_Wall(Collision2D hit){
        Hit_Event();
    }
}
