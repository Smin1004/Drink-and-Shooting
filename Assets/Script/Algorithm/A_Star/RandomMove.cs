using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RandomMove : Enemy_Base
{
    [Header("RandomMove")]
    public Vector2Int room_LeftDown;
    public Vector2Int room_RightUp;
    [SerializeField] int maxCheckSize;
    [SerializeField] int minCheckSize;

    protected void RandomPosCheak()
    {
        A_Node curNode = A_Map.GetNodeTransfrom(target.position);
        List<A_Node> posList = new List<A_Node>();

        foreach (A_Node n in A_Map.find_Pos(curNode, maxCheckSize, minCheckSize, room_LeftDown, room_RightUp))
        {
            posList.Add(n);
        }

        int ranPos = Random.Range(0, posList.Count);

        transform.position = posList[ranPos].pos;
    }
}
