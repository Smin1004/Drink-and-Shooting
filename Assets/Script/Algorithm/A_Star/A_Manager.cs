using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class A_Manager : MonoBehaviour
{
    private static A_Manager _instance = null;
    public static A_Manager Instance => _instance;

    Queue<A_Requset> requsetQueue = new Queue<A_Requset>();
    A_Requset curRequset;
    A_Check a_Check;

    bool isProcessing;

    public void _Instance() {
        _instance = this;
        a_Check = GetComponent<A_Check>();
    }

    public static void Requset(Vector3 _start, Vector3 _end, UnityAction<List<A_Node>, bool> _callbeck){
        A_Requset newRequset = new A_Requset(_start, _end, _callbeck);
        Instance.requsetQueue.Enqueue(newRequset);
        Instance.TryProcessingNext();
    }

    void TryProcessingNext(){
        if(!isProcessing && requsetQueue.Count > 0){
            curRequset = requsetQueue.Dequeue();
            isProcessing = true;
            a_Check.Find(curRequset.start, curRequset.end);
        }
    }

    public void Finished(List<A_Node> path, bool success){
        curRequset.callback(path, success);
        isProcessing = false;
        TryProcessingNext();
    }
}

struct A_Requset
{
    public Vector3 start;
    public Vector3 end;
    public UnityAction<List<A_Node>, bool> callback;

    public A_Requset(Vector3 _start, Vector3 _end, UnityAction<List<A_Node>, bool> _callbeck)
    {
        start = _start;
        end = _end;
        callback = _callbeck;
    }
}
