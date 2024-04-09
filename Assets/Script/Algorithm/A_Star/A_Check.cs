using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class A_Check : MonoBehaviour
{
    A_Manager a_Manager;

    private void Awake()
    {
        a_Manager = GetComponent<A_Manager>();
    }

    public void Find(Vector3 startPos, Vector3 endPos)
    {
        A_Node startNode = A_Map.GetNodeTransfrom(startPos);
        A_Node endNode = A_Map.GetNodeTransfrom(endPos);
        bool isSuccess = false;

        List<A_Node> openNode = new List<A_Node>();
        HashSet<A_Node> closeNode = new HashSet<A_Node>();
        openNode.Add(startNode);

        List<A_Node> end = new List<A_Node>();

        while (openNode.Count > 0)
        {

            openNode.Sort(delegate (A_Node a, A_Node b)
            {
                if (a.fCost < b.fCost) return -1;
                else return 1;
            });

            A_Node curNode = openNode[0];

            openNode.Remove(curNode);
            closeNode.Add(curNode);

            if (curNode == endNode)
            {
                isSuccess = true;
                break;
            }
            
            foreach (A_Node n in A_Map.find_Node(curNode))
            {

                if (n.isWall || closeNode.Contains(n)) continue;

                int newCurNodeGCost = curNode.gCost + NodeCost(curNode, n);
                if (newCurNodeGCost < n.gCost || !openNode.Contains(n))
                {
                    n.gCost = newCurNodeGCost;
                    n.hCost = NodeCost(n, endNode);
                    n.parentNode = curNode;

                    if (!openNode.Contains(n)) openNode.Add(n);
                }
            }
        }
        if(isSuccess){
           end = ResultNode(startNode, endNode);
        }
        a_Manager.Finished(end, isSuccess);
    }

    List<A_Node> ResultNode(A_Node startNode, A_Node endNode)
    {
        List<A_Node> result = new List<A_Node>();
        A_Node curNode = endNode;

        while (curNode != startNode)
        {
            result.Add(curNode);
            curNode = curNode.parentNode;
        }
        result.Reverse();
        return result;
    }

    int NodeCost(A_Node nodeA, A_Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.pos.x - nodeB.pos.x);
        int distY = Mathf.Abs(nodeA.pos.y - nodeB.pos.y);

        if (distX > distY) return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }
}
