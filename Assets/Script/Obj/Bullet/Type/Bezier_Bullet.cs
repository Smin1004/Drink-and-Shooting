using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier_Bullet : Bullet_Base
{
    [SerializeField] private float move_Value;
    [SerializeField] private float maxStopValue;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private List<Vector3> plusPos;

    public void InitBezier(Vector3 _startPos, Vector3 _endPos, float minRadius, float maxRadius, float _maxStopValue){
        startPos = _startPos;
        plusPos.Add(GetCirclePos(startPos, Random.Range(minRadius, maxRadius + 1)));
        endPos = _endPos;
        maxStopValue = _maxStopValue;
        move_Value = 0;
    }

    protected override void Update()
    {
        //base.Update();
        if(isStop && move_Value > maxStopValue){
        
        }else{
            transform.position = Bezier(startPos, endPos, plusPos, move_Value);
            move_Value += Time.deltaTime;
        }
    }


    Vector3 GetCirclePos(Vector3 center, float radius){
        int angle = Random.Range(0, 360);
        
        if(angle % 2 == 0) return center + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sign(angle * Mathf.Rad2Deg) * radius);
        else return center - new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sign(angle * Mathf.Rad2Deg) * radius);
    }

    Vector3 Bezier(Vector3 startPos, Vector3 endPos, List<Vector3> plusPos, float value)
    {
        List<Vector3> posList = new List<Vector3>();

        posList.Add(startPos);
        foreach (var n in plusPos)
        {
            posList.Add(n);
        }
        posList.Add(endPos);

        while (posList.Count > 1)
        {
            List<Vector3> curPos = new List<Vector3>();
            for (int i = 0; i < posList.Count - 1; i++)
            {
                curPos.Add(Vector3.LerpUnclamped(posList[i], posList[i + 1], value));
            }
            posList = curPos;
        }

        return posList[0];
    }

    protected override void Hit_Event()
    {
        Destroy(gameObject);
    }

    protected override void Hit_Wall(Collision2D hit)
    {
        Hit_Event();
    }
}
