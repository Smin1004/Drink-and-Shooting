using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class B_TreeMaker : MonoBehaviour
{
    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private int maxNodeCount;

    [SerializeField] private float minBoxSize;
    [SerializeField] private float maxBoxSize;

    [SerializeField] private List<Map> mapData = new List<Map>();
    private List<Vector2Int> RoomPos = new List<Vector2Int>();

    B_CreateMap createMap;

    private void Start() {
        createMap = GetComponent<B_CreateMap>();
        createMap.Setting();
        B_Node startNode = new B_Node(0, 0, mapSize.x, mapSize.y);
        MakeTree(startNode, 0);
        MakeMap(startNode, 0);

        Vector2Int startPos = RoomPos[Random.Range(0, RoomPos.Count)];
        RoomPos.Remove(startPos);
        MakeRoad(startPos);
    }

    private void MakeTree(B_Node treeNode, int count)
    {
        if (count < maxNodeCount)
        {
            RectInt size = treeNode.treeSize;

            int length = size.width >= size.height ? size.width : size.height;
            int split = Mathf.RoundToInt(Random.Range(length * minBoxSize, length * maxBoxSize));
            if (size.width >= size.height)
            {
                treeNode.leftTree = new B_Node(size.x, size.y, split, size.height);
                treeNode.rightTree = new B_Node(size.x + split, size.y, size.width - split, size.height);
                //createMap.OnDrawLine(new Vector2Int(size.x + split, size.y), new Vector2Int(size.x + split, size.y + size.height));
            }
            else
            {
                treeNode.leftTree = new B_Node(size.x, size.y, size.width, split);
                treeNode.rightTree = new B_Node(size.x, size.y + split, size.width, size.height - split);
                //createMap.OnDrawLine(new Vector2Int(size.x, size.y + split), new Vector2Int(size.x + size.width, size.y + split));
            }
            
            treeNode.leftTree.parentTree = treeNode;
            treeNode.rightTree.parentTree = treeNode;

            MakeTree(treeNode.leftTree, count + 1);
            MakeTree(treeNode.rightTree, count + 1);
        }
    }

    private void MakeMap(B_Node treeNode, int count)
    {
        if (count == maxNodeCount)
        {
            int ranMap = Random.Range(0, mapData.Count);
            RectInt curtree = treeNode.treeSize;
            createMap.CreateMap(treeNode, curtree.x + curtree.width / 2, curtree.y + curtree.height / 2,
                                mapData[ranMap].tile, mapData[ranMap].wall);
            RoomPos.Add(new Vector2Int(curtree.x + curtree.width / 2, curtree.y + curtree.height / 2));
            mapData.Remove(mapData[ranMap]);
            return;
        }
        MakeMap(treeNode.leftTree, count + 1);
        MakeMap(treeNode.rightTree, count + 1);
    }

    void MakeRoad(Vector2Int from){
        if(RoomPos.Count == 0) return;

        Vector2Int to = FindCloseRoom(from, RoomPos);
        RoomPos.Remove(to);
        createMap.CreateRoad(from, to);

        MakeRoad(to);
    }

    private Vector2Int FindCloseRoom(Vector2Int curRoom, List<Vector2Int> RoomList){
        Vector2Int closeRoom = Vector2Int.zero;
        float distance = float.MaxValue;

        foreach(var pos in RoomList){
            float curdistance = Vector2.Distance(pos, curRoom);
            if(curdistance < distance){
                distance = curdistance;
                closeRoom = pos;
            }
        }
        return closeRoom;
    }
}
