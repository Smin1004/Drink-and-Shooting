using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Node
{
    public bool isWall;
    public Vector3Int pos;

    public int gCost;
    public int hCost;
    public A_Node parentNode;

    public A_Node(bool _isWall, Vector3Int _pos)
    {
        isWall = _isWall;
        pos = _pos;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }
}
