using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tilte_Size : MonoBehaviour
{
    [SerializeField] float sizeUpSpeed;
    [SerializeField] bool isSizeUp;

    void Update()
    {
        if(transform.localScale.x >= 1)
            isSizeUp = false;
        else if(transform.localScale.x <= 0.6f)
            isSizeUp = true;

        Size();
    }

    void Size()
    {
        if(isSizeUp){
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.1f, 1.1f), Time.deltaTime * sizeUpSpeed);
            
        }
        else{
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.5f, 0.5f), Time.deltaTime * sizeUpSpeed);
            
        }
    }
}
