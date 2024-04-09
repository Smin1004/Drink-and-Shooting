using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public void OpenDoor(){
        gameObject.SetActive(true);
    }

    public void CloseDoor(){
        gameObject.SetActive(false);
    }
}
