using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash_obj : MonoBehaviour
{
    void Start()
    {
        Invoke("Destroy", 0.2f);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}