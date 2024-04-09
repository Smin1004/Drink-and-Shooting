using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class A_Map : MonoBehaviour
{
    private static A_Map _instance = null;
    public static A_Map Instance => _instance;

    //[SerializeField] Tilemap testM;
    //[SerializeField] Tile test;

    [SerializeField] Tilemap wall;
    [SerializeField] Tilemap tile;
    [SerializeField] Tile invisibleTile;
    A_Node[,] a_Node;

    Vector3Int center;
    Vector3Int plusXY;
    int map_x;
    int map_y;

    public void _Instance()
    {
        _instance = this;
        wall.CompressBounds();
        
        if(Mathf.RoundToInt(wall.size.x) % 2 != 0) wall.SetTile(new Vector3Int(wall.cellBounds.xMax, 0), invisibleTile);
        if(Mathf.RoundToInt(wall.size.y) % 2 != 0) wall.SetTile(new Vector3Int(0, wall.cellBounds.yMax), invisibleTile);

        map_x = Mathf.RoundToInt(wall.size.x / 2);
        map_y = Mathf.RoundToInt(wall.size.y / 2);
        CheckALLTile();
    }

    void CheckALLTile()
    {
        center = new Vector3Int((int)wall.cellBounds.center.x, (int)wall.cellBounds.center.y);
        a_Node = new A_Node[wall.size.x, wall.size.y];

        for (int i = 0; i < wall.size.x; i++)
        {
            for (int j = 0; j < wall.size.y; j++)
            {
                int x = i + center.x - map_x;
                int y = j + center.y - map_y;
                if (tile.HasTile(new Vector3Int(i + wall.cellBounds.xMin, j + wall.cellBounds.yMin))){
                    a_Node[i, j] = new A_Node(false, new Vector3Int(x, y));
                    //testM.SetTile(new Vector3Int(x,y), test);
                }
                else{
                    a_Node[i, j] = new A_Node(true, new Vector3Int(x, y));
                    //testM.SetTile(new Vector3Int(x,y), invisibleTile);
                }
            }
        }

        plusXY = -a_Node[0,0].pos;
    }

    public static A_Node GetNodeTransfrom(Vector3 nodePos)
    {
        Vector3Int pos = Instance.wall.WorldToCell(nodePos);

        return Instance.a_Node[pos.x + Instance.plusXY.x, pos.y + Instance.plusXY.y];
    }

    public static List<A_Node> find_Node(A_Node node)
    {
        List<A_Node> node_List = new List<A_Node>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                int x = node.pos.x + i;
                int y = node.pos.y + j;
                
                node_List.Add(Instance.a_Node[x + Instance.plusXY.x, y + Instance.plusXY.y]);
            }
        }
        return node_List;
    }

    public static List<A_Node> find_Pos(A_Node node, int maxSize, int minSize, Vector2Int leftDown, Vector2Int rightUp)
    {
        List<A_Node> node_List = new List<A_Node>();
        for (int i = -maxSize; i <= maxSize; i++)
        {
            for (int j = -maxSize; j <= maxSize; j++)
            {
                int x = node.pos.x + i;
                int y = node.pos.y + j;
                
                try{
                    A_Node curNode = Instance.a_Node[x + Instance.plusXY.x, y + Instance.plusXY.y];

                    if ((Mathf.Abs(i) <= minSize && Mathf.Abs(j) <= minSize) || curNode.isWall ||
                    leftDown.x > curNode.pos.x || leftDown.y > curNode.pos.y ||
                    rightUp.x < curNode.pos.x || rightUp.y < curNode.pos.y) continue;
                
                    //Instance.testM.SetTile(curNode.pos, Instance.test);
                    node_List.Add(curNode);
                }catch{
                    continue;
                }
            }
        }
        return node_List;
    }
}
