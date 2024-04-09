using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitStage : MonoBehaviour
{
    [SerializeField] private float waitTime;
    private float curTime;

    private void OnTriggerStay2D(Collider2D other) {
        if(other.CompareTag("Player")){
            curTime+=Time.deltaTime;
            if(curTime >= waitTime){
                GameManager.Instance.NextStage();
                Destroy(GetComponent<CircleCollider2D>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player"))
            curTime = 0;
    }
}
