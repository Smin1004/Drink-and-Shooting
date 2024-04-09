using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] float moveSpeed;

    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);
    }
}
