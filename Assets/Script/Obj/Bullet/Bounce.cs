using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Bounce : Bullet_Base
{
    [Header("Bounce")]
    [SerializeField] protected int bounce_count;
    
    protected void _Bounce(Collision2D other)
    {
        if (other.collider != null && other.collider.CompareTag("Wall"))
        {
            Vector3 income = transform.TransformDirection(Vector3.up).normalized;
            Vector3 normal = other.GetContact(0).normal;
            var dir = Vector3.Reflect(income, normal).normalized;

            var rotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0,0,rotation-90);
        }
    }
}
