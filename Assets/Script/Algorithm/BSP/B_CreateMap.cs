using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class B_CreateMap : MonoBehaviour
{
    private Tilemap tileBase;
    private Tilemap wallBase;
    [SerializeField] Tile test;

    int map_x;
    int map_y;

    int plus_x;
    int plus_y;

    public void Setting()
    {
        tileBase = MainCanvas.Instance.tileBase;
        wallBase = MainCanvas.Instance.wallBase;
    }

    public void CreateMap(B_Node treeNode, int _x, int _y, Tilemap tile, Tilemap wall)
    {
        plus_x = _x;
        plus_y = _y;
        map_x = Mathf.RoundToInt(tile.size.x / 2);
        map_y = Mathf.RoundToInt(tile.size.y / 2);

        for (int i = -map_x - 1; i < map_x; i++)
        {
            for (int j = -map_y - 1; j < map_y + 1; j++)
            {
                int x = i + plus_x;
                int y = j + plus_y;

                if (wall.HasTile(new Vector3Int(i, j)))
                    wallBase.BoxFill(new Vector3Int(x, y), wall.GetTile(new Vector3Int(i, j)), x, y, x, y);

                if (tile.HasTile(new Vector3Int(i, j)))
                    tileBase.BoxFill(new Vector3Int(x, y), tile.GetTile(new Vector3Int(i, j)), x, y, x, y);

            }
        }
    }

    public void CreateRoad(Vector2Int from, Vector2Int to)
    {
        Vector2Int start = Vector2Int.zero;
        Vector2Int end = Vector2Int.zero;

        start.x = from.x <= to.x ? from.x : to.x;
        end.x = from.x <= to.x ? to.x : from.x;
        start.y = from.y <= to.y ? from.y : to.y;
        end.y = from.y <= to.y ? to.y : from.y;

        for (int x = start.x - 1; x <= end.x + 1; x++)
        {
            for (int y = from.y - 1; y <= from.y + 1; y++)
            {
                if (!tileBase.HasTile(new Vector3Int(x, y)))
                    tileBase.SetTile(new Vector3Int(x, y), test);
                
            }
        }

        for (int y = start.y - 1; y <= end.y + 1; y++)
        {
            for (int x = to.x - 1; x <= to.x + 1; x++)
            {
                if (!tileBase.HasTile(new Vector3Int(x, y)))
                    tileBase.SetTile(new Vector3Int(x, y), test);

            }
        }
    }

    public void OnDrawLine(Vector2Int from, Vector2Int to)
    {
        wallBase.SetTile(new Vector3Int(from.x, from.y), null);
        wallBase.BoxFill(new Vector3Int(from.x, from.y), test, from.x, from.y, to.x, to.y);
    }
}
