using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "MapData", menuName = "Game/MapData", order = int.MinValue), Serializable]
public class Map : ScriptableObject
{
    public Tilemap wall;
    public Tilemap tile;

    [Header("Up : 0 Right : 90 Down : 180 Left 270")]
    public List<Vector3Int> doorPos = new List<Vector3Int>();
    public List<bool> isOpen = new List<bool>();

    public Vector2Int leftDown;
    public Vector2Int rightUp;

    public List<EnemyList> enemyList = new List<EnemyList>();
}

[Serializable]
public class EnemyList
{
    public Enemy_Base spawnEnemy;
    public Vector2Int spawnPos;

    public int wavecount;
    public float waitTime;
}

[Serializable]
public class MapTree
{
    public Map map;
    public Map hallWay;
    public Vector3Int mapPos;
    public Vector3Int hallWayPos;
    public Vector3Int inputDoor;
    public List<Vector3Int> outputDoor = new List<Vector3Int>();

    public MapTree parentTree;
    public MapTree leftTree;
    public MapTree rightTree;
    public MapTree downTree;

    private SpriteRenderer fade;
    private Tilemap wallBase;
    private Tilemap tileBase;
    //private Tilemap testBase;
    private Tilemap mini_WallBase;
    private Tilemap mini_TileBase;
    //private Tile testTile;
    private Tile mini_Wall;
    private Tile mini_Tile;

    public MapTree(Vector3Int _mapPos, Vector3Int _hallWayPos, Map _map, Map _hallWay, Vector3Int _inputDoor)
    {
        map = _map;
        mapPos = _mapPos;
        hallWay = _hallWay;
        hallWayPos = _hallWayPos;
        inputDoor = _inputDoor;

        MainCanvas canvas = MainCanvas.Instance;
        tileBase = canvas.tileBase;
        mini_WallBase = canvas.mini_WallBase;
        mini_TileBase = canvas.mini_TileBase;
        mini_Wall = canvas.mini_Wall;
        mini_Tile = canvas.mini_Tile;
    }

    public void ForOfWar(MapTree mapTree, bool isHallWay)
    {
        Map map;
        Vector3Int plusXY;

        if (isHallWay)
        {
            map = mapTree.hallWay;
            plusXY = mapTree.hallWayPos;
        }
        else
        {
            map = mapTree.map;
            plusXY = mapTree.mapPos;
        }

        for (int i = map.leftDown.x; i <= map.rightUp.x; i++)
        {
            for (int j = map.leftDown.y; j <= map.rightUp.y; j++)
            {
                int x = i + plusXY.x;
                int y = j + plusXY.y;

                if (map.wall.HasTile(new Vector3Int(i, j)))
                    if (isHallWay)
                        mini_WallBase.SetTile(new Vector3Int(x, y), mini_Wall);
                    else
                    {
                        if (tileBase.HasTile(new Vector3Int(x, y)))
                            mini_TileBase.SetTile(new Vector3Int(x, y), mini_Tile);
                        else
                            mini_WallBase.SetTile(new Vector3Int(x, y), mini_Wall);
                    }

                if (map.tile.HasTile(new Vector3Int(i, j)))
                    mini_TileBase.SetTile(new Vector3Int(x, y), mini_Tile);
            }
        }
    }
}