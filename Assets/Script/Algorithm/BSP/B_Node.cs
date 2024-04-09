using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Node
{
    public B_Node leftTree;
    public B_Node rightTree;
    public B_Node parentTree;
    public RectInt treeSize;
    public HashSet<Vector2Int> doorPos;

    public B_Node(int x, int y, int width, int height)
    {
        treeSize.x = x;
        treeSize.y = y;
        treeSize.width = width;
        treeSize.height = height;
    }
}
