using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map_Generator : MonoBehaviour
{
    [Header("TileMap")]
    private Tilemap wallBase;
    private Tilemap tileBase;
    //private Tilemap testBase;
    private Tilemap mini_WallBase;
    private Tilemap mini_TileBase;
    //private Tile testTile;
    private Tile mini_Wall;
    private Tile mini_Tile;

    [Header("Obj")]
    [SerializeField] private Enter_Room enterRoom;
    [SerializeField] private ExitStage exitStage;
    [SerializeField] private GameObject doorWidth;
    [SerializeField] private GameObject doorLength;
    [SerializeField] private GameObject blind;
    [SerializeField] private Transform spawn;
    [SerializeField] private Transform door;
    private List<Door> doorList = new List<Door>();

    [Header("Map")]
    [SerializeField] private Stage curStage;
    [SerializeField] private int stageIndex;

    [Header("Start : 0  Boss : 1  Exit : 2  Shop : 3  Chest : 4")]
    [SerializeField] private List<Map> specialMap = new List<Map>();
    [SerializeField] private List<Map> battleMap = new List<Map>();
    //[SerializeField] private List<Map> eventMap = new List<Map>();
    [SerializeField] private List<Map> hallWay = new List<Map>();
    [SerializeField] private List<Map> bossMap = new List<Map>();

    [SerializeField] private List<Map> testMap = new List<Map>();

    [Header("SpecialMap Count")]
    [SerializeField] private int shopCount;
    [SerializeField] private int chest01Count;
    [SerializeField] private int chest02Count;
    [SerializeField] private int bossRoomCount;

    [Header("Map Count")]
    [SerializeField] private int maxBattleRoomCount;
    [SerializeField] private int curBattleRoomCount;

    [SerializeField] private Vector3Int xy;
    private bool isExit;
    private bool isNotRandom;

    public void StartCreateMap()
    {
        Initialization();
        MapTree startTree = new MapTree(xy, Vector3Int.zero, specialMap[0], null, Vector3Int.zero);
        CreateMap(startTree);
        CreateDoor(startTree);
        GameManager.Instance.doorList = doorList;
        GameManager.Instance.DoorOnOff(false);
        startTree.ForOfWar(startTree, false);
    }

    void Initialization()
    {
        MainCanvas canvas = MainCanvas.Instance;
        wallBase = canvas.wallBase;
        tileBase = canvas.tileBase;
        //testBase = canvas.testBase;
        mini_WallBase = canvas.mini_WallBase;
        mini_TileBase = canvas.mini_TileBase;
        //testTile = canvas.testTile;
        mini_Wall = canvas.mini_Wall;
        mini_Tile = canvas.mini_Tile;

        stageIndex = Data.Instance.stageIndex;
        curStage = GameManager.Instance.stageList[stageIndex];


        specialMap = curStage.specialMap.ToList();
        battleMap = curStage.battleMap.ToList();
        hallWay = curStage.hallWay.ToList();
        bossMap = curStage.bossMap.ToList();

        shopCount = curStage.shopCount;
        chest01Count = curStage.chest01Count;
        chest02Count = curStage.chest02Count;
        bossRoomCount = curStage.bossRoomCount;
        isNotRandom = curStage.isNotRandom;

        if (isNotRandom) maxBattleRoomCount = curStage.battleMap.Count;
        else maxBattleRoomCount = curStage.maxBattleRoomCount;
        curBattleRoomCount = 0;


        foreach (var i in specialMap) for (int j = 0; j < i.isOpen.Count; j++) i.isOpen[j] = false;
        foreach (var i in battleMap) for (int j = 0; j < i.isOpen.Count; j++) i.isOpen[j] = false;
        foreach (var i in hallWay) for (int j = 0; j < i.isOpen.Count; j++) i.isOpen[j] = false;
        foreach (var i in bossMap) for (int j = 0; j < i.isOpen.Count; j++) i.isOpen[j] = false;
        for (int i = 0; i < testMap[0].isOpen.Count; i++) testMap[0].isOpen[i] = false;
    }

    void CreateMap(MapTree mapTree)
    {
        if (curBattleRoomCount == 0)
        {
            for (int i = mapTree.map.leftDown.x; i <= mapTree.map.rightUp.x; i++)
            {
                for (int j = mapTree.map.leftDown.y; j <= mapTree.map.rightUp.y; j++)
                {
                    if (mapTree.map.wall.HasTile(new Vector3Int(i, j)))
                    {
                        wallBase.SetTile(new Vector3Int(i + xy.x, j + xy.y), mapTree.map.wall.GetTile(new Vector3Int(i, j)));
                        mini_WallBase.SetTile(new Vector3Int(i + xy.x, j + xy.y), mini_Wall);
                    }

                    if (mapTree.map.tile.HasTile(new Vector3Int(i, j)))
                    {
                        tileBase.SetTile(new Vector3Int(i + xy.x, j + xy.y), mapTree.map.tile.GetTile(new Vector3Int(i, j)));
                        mini_TileBase.SetTile(new Vector3Int(i + xy.x, j + xy.y), mini_Tile);
                    }
                }
            }

            if (isNotRandom)
            {
                mapTree = CreateNotRandomMap(mapTree, battleMap);
                CreateMap(mapTree);
            }
            else
            {
                mapTree = CreateRandomMap(mapTree, battleMap, false);
                CreateMap(mapTree);
            }
        }
        else
        {
            //Debug.Log($"curRoomCount : {curRoomCount} xy : {xy}");
            if (curBattleRoomCount >= maxBattleRoomCount) return;

            if (curBattleRoomCount == bossRoomCount) CreateForkedRoad(mapTree, 1);
            if (curBattleRoomCount == shopCount) CreateForkedRoad(mapTree, 3);
            if (curBattleRoomCount == chest01Count) CreateForkedRoad(mapTree, 4);
            if (curBattleRoomCount == chest02Count) CreateForkedRoad(mapTree, 4);
            if (curBattleRoomCount >= maxBattleRoomCount) return;

            if (isNotRandom)
            {
                mapTree = CreateNotRandomMap(mapTree, battleMap);
                CreateMap(mapTree);
            }
            else
            {
                mapTree = CreateRandomMap(mapTree, battleMap, false);
                CreateMap(mapTree);
            }

            CreateMap(mapTree);
        }
    }

    MapTree CreateNotRandomMap(MapTree curTree, List<Map> mapList)
    {
        Vector3Int curDoor = new Vector3Int(curTree.mapPos.x, curTree.mapPos.y) + FindNextDoorPos(curTree.mapPos, curTree.mapPos, curTree.map);
        while (true)
        {
            if (mapList.Count == 1) isExit = true;

            int ranHallWay = Random.Range(0, hallWay.Count);
            Map curHallWay = hallWay[ranHallWay];
            for (int i = 0; i < curHallWay.isOpen.Count; i++) curHallWay.isOpen[i] = false;
            Vector3Int curHallWayDoor = CheckDoor(curDoor, curHallWay);

            if (curHallWayDoor == Vector3Int.zero) continue;
            Vector3Int nextHallWayDoor = FindNextDoorPos(curDoor, curHallWayDoor, curHallWay);

            Map curMap = mapList[0];
            Vector3Int curMapDoor = CheckDoor(nextHallWayDoor, curMap);


            Vector3Int hallWayXY = new Vector3Int(curDoor.x + -curHallWayDoor.x, curDoor.y + -curHallWayDoor.y, curHallWayDoor.z);
            xy = new Vector3Int(nextHallWayDoor.x + -curMapDoor.x, nextHallWayDoor.y + -curMapDoor.y, curMapDoor.z);
            mapList.Remove(mapList[0]);
            curBattleRoomCount++;


            curTree.downTree = new MapTree(xy, hallWayXY, curMap, curHallWay, curMapDoor) { parentTree = curTree };
            curTree.outputDoor.Add(curDoor - new Vector3Int(curTree.mapPos.x, curTree.mapPos.y));

            DrawMap(curDoor, curHallWayDoor, curTree.downTree, true);
            DrawMap(nextHallWayDoor, curMapDoor, curTree.downTree, false);

            return curTree.downTree;
        }
    }

    MapTree CreateRandomMap(MapTree curTree, List<Map> mapList, bool isForkRoad)
    {
        int loopCount = 0;
        Vector3Int curDoor = new Vector3Int(curTree.mapPos.x, curTree.mapPos.y) + FindNextDoorPos(curTree.mapPos, curTree.mapPos, curTree.map);
        while (true)
        {
            loopCount++;
            int ranHallWay = Random.Range(0, hallWay.Count);
            Map curHallWay = hallWay[ranHallWay];
            for (int i = 0; i < curHallWay.isOpen.Count; i++) curHallWay.isOpen[i] = false;
            Vector3Int curHallWayDoor = CheckDoor(curDoor, curHallWay);

            if (curHallWayDoor == Vector3Int.zero) continue;

            if (CheckMap(curDoor, curHallWayDoor, curHallWay, true))
            {
                if (loopCount > 50)
                    return CreateRandomMap(curTree.parentTree, mapList, true);

                curDoor = new Vector3Int(curTree.mapPos.x, curTree.mapPos.y) + FindNextDoorPos(curTree.mapPos, curTree.mapPos, curTree.map);
                continue;
            }

            Vector3Int nextHallWayDoor = FindNextDoorPos(curDoor, curHallWayDoor, curHallWay);
            if (nextHallWayDoor == Vector3Int.zero)
                return CreateRandomMap(curTree.parentTree, mapList, true);

            int ranMap = Random.Range(0, mapList.Count);
            Map curMap = mapList[ranMap];

            Vector3Int curMapDoor = CheckDoor(nextHallWayDoor, curMap);

            if (CheckMap(nextHallWayDoor, curMapDoor, curMap, false))
            {
                curDoor = new Vector3Int(curTree.mapPos.x, curTree.mapPos.y) + FindNextDoorPos(curTree.mapPos, curTree.mapPos, curTree.map);
                continue;
            }


            Vector3Int hallWayXY = new Vector3Int(curDoor.x + -curHallWayDoor.x, curDoor.y + -curHallWayDoor.y, curHallWayDoor.z);
            xy = new Vector3Int(nextHallWayDoor.x + -curMapDoor.x, nextHallWayDoor.y + -curMapDoor.y, curMapDoor.z);

            mapList.Remove(mapList[ranMap]);
            curBattleRoomCount++;

            MapTree nextTree;
            if (isForkRoad)
            {
                if (curTree.leftTree != null)
                {
                    curTree.rightTree = new MapTree(xy, hallWayXY, curMap, curHallWay, curMapDoor) { parentTree = curTree };
                    curTree.outputDoor.Add(curDoor - new Vector3Int(curTree.mapPos.x, curTree.mapPos.y));
                    nextTree = curTree.rightTree;
                }
                else
                {
                    curTree.leftTree = new MapTree(xy, hallWayXY, curMap, curHallWay, curMapDoor) { parentTree = curTree };
                    curTree.outputDoor.Add(curDoor - new Vector3Int(curTree.mapPos.x, curTree.mapPos.y));
                    nextTree = curTree.leftTree;
                }
            }
            else
            {
                curTree.downTree = new MapTree(xy, hallWayXY, curMap, curHallWay, curMapDoor) { parentTree = curTree };
                curTree.outputDoor.Add(curDoor - new Vector3Int(curTree.mapPos.x, curTree.mapPos.y));
                nextTree = curTree.downTree;
            }

            DrawMap(curDoor, curHallWayDoor, nextTree, true);
            DrawMap(nextHallWayDoor, curMapDoor, nextTree, false);

            return nextTree;
        }
    }

    void CreateForkedRoad(MapTree mapTree, int specialMapIndex)
    {
        Vector3Int orignalMapPos = mapTree.mapPos;
        int ranCount = Random.Range(1, 3);

        for (int i = 0; i < ranCount; i++)
        {
            mapTree = CreateRandomMap(mapTree, battleMap, true);
            if (curBattleRoomCount == bossRoomCount) CreateForkedRoad(mapTree, 1);
            if (curBattleRoomCount == shopCount) CreateForkedRoad(mapTree, 3);
            if (curBattleRoomCount == chest01Count) CreateForkedRoad(mapTree, 4);
            if (curBattleRoomCount == chest02Count) CreateForkedRoad(mapTree, 4);
        }

        NowCreateSpecailMap(mapTree, orignalMapPos, specialMapIndex);
    }

    void NowCreateSpecailMap(MapTree curTree, Vector3Int originalMapPos, int specialMapIndex)
    {
        int loopCount = 0;
        Vector3Int curDoor = new Vector3Int(curTree.mapPos.x, curTree.mapPos.y) + FindNextDoorPos(curTree.mapPos, curTree.mapPos, curTree.map);
        while (true)
        {
            loopCount++;
            int ranHallWay = Random.Range(0, hallWay.Count);
            Map curHallWay = hallWay[ranHallWay];
            for (int i = 0; i < curHallWay.isOpen.Count; i++) curHallWay.isOpen[i] = false;
            Vector3Int curHallWayDoor = CheckDoor(curDoor, curHallWay);

            if (curHallWayDoor == Vector3Int.zero) continue;

            if (CheckMap(curDoor, curHallWayDoor, curHallWay, true))
            {
                if (loopCount > 50)
                {
                    NowCreateSpecailMap(curTree.parentTree, originalMapPos, specialMapIndex);
                    return;
                }

                curDoor = new Vector3Int(curTree.mapPos.x, curTree.mapPos.y) + FindNextDoorPos(curTree.mapPos, curTree.mapPos, curTree.map);
                continue;
            }

            Vector3Int nextHallWayDoor = FindNextDoorPos(curDoor, curHallWayDoor, curHallWay);

            Map curMap = specialMap[specialMapIndex];
            for (int i = 0; i < curMap.isOpen.Count; i++) curMap.isOpen[i] = false;
            Vector3Int curMapDoor = CheckDoor(nextHallWayDoor, curMap);

            if (CheckMap(nextHallWayDoor, curMapDoor, curMap, false))
            {
                if (loopCount > 50)
                {
                    NowCreateSpecailMap(curTree.parentTree, originalMapPos, specialMapIndex);
                    return;
                }

                curDoor = new Vector3Int(curTree.mapPos.x, curTree.mapPos.y) + FindNextDoorPos(xy, curTree.mapPos, curTree.map);
                continue;
            }

            Vector3Int hallWayXY = new Vector3Int(curDoor.x + -curHallWayDoor.x, curDoor.y + -curHallWayDoor.y, curHallWayDoor.z);
            xy = new Vector3Int(nextHallWayDoor.x + -curMapDoor.x, nextHallWayDoor.y + -curMapDoor.y, curMapDoor.z);

            curTree.downTree = new MapTree(xy, hallWayXY, curMap, curHallWay, curMapDoor) { parentTree = curTree };
            curTree.outputDoor.Add(curDoor - new Vector3Int(curTree.mapPos.x, curTree.mapPos.y));

            DrawMap(curDoor, curHallWayDoor, curTree.downTree, true);
            DrawMap(nextHallWayDoor, curMapDoor, curTree.downTree, false);

            if (specialMapIndex == 1)
            {
                MapTree downTree = CreateRandomMap(curTree.downTree, bossMap, false);
                curBattleRoomCount--;
                isExit = true;
                NowCreateSpecailMap(downTree, xy, 2);
                isExit = false;
            }

            xy = originalMapPos;

            return;
        }
    }

    Vector3Int CheckDoor(Vector3Int curXY, Map map)
    {
        for (int i = 0; i < map.doorPos.Count; i++)
        {
            if (Mathf.Abs(curXY.z - map.doorPos[i].z) == 180)
            {
                map.isOpen[i] = true;
                return map.doorPos[i];
            }
        }
        return Vector3Int.zero;
    }

    Vector3Int FindNextDoorPos(Vector3Int plusXY, Vector3Int curDoor, Map map)
    {
        int count = 0;
        while (true)
        {
            count++;
            if (count > 30)
                return Vector3Int.zero;

            int ranPos = Random.Range(0, map.doorPos.Count);
            if (map.isOpen[ranPos] == true) continue;

            Vector3Int nextDoor = new Vector3Int(plusXY.x + -curDoor.x, plusXY.y + -curDoor.y) + map.doorPos[ranPos];
            if (nextDoor.z >= 360) nextDoor.z -= 360;
            if (nextDoor.z < 0) nextDoor.z = Mathf.Abs(nextDoor.z);
            map.isOpen[ranPos] = true;

            return nextDoor;
        }
    }

    void CreateDoor(MapTree mapTree)
    {
        Vector3 Check(Vector3 door)
        {
            switch (door.z)
            {
                case 0: door.y += 1; break;
                case 90: door.x += 1; break;
                case 180: door.y += -1; break;
                case 270: door.x += -1; break;
            }

            return door;
        }

        if (mapTree.parentTree != null)
        {
            Vector3 curPos = mapTree.inputDoor + new Vector3Int(mapTree.mapPos.x, mapTree.mapPos.y);
            Door curDoor;
            if (curPos.z == 0 || curPos.z == 180) curDoor = Instantiate(doorWidth, Check(curPos), Quaternion.identity, door).GetComponent<Door>();
            else curDoor = Instantiate(doorLength, Check(curPos), Quaternion.identity, door).GetComponent<Door>();

            if (curDoor != null) doorList.Add(curDoor);
        }

        if (mapTree.outputDoor.Count != 0)
            for (int i = 0; i < mapTree.outputDoor.Count; i++)
            {
                Vector3 curPos = mapTree.outputDoor[i] + new Vector3Int(mapTree.mapPos.x, mapTree.mapPos.y);
                Door curDoor;
                if (curPos.z == 0 || curPos.z == 180) curDoor = Instantiate(doorWidth, Check(curPos), Quaternion.identity, door).GetComponent<Door>();
                else curDoor = Instantiate(doorLength, Check(curPos), Quaternion.identity, door).GetComponent<Door>();

                if (curDoor != null) doorList.Add(curDoor);
            }

        if (mapTree.downTree != null) CreateDoor(mapTree.downTree);
        if (mapTree.leftTree != null) CreateDoor(mapTree.leftTree);
        if (mapTree.rightTree != null) CreateDoor(mapTree.rightTree);
    }

    void DrawMap(Vector3Int curDoor, Vector3Int pos, MapTree mapTree, bool isHallWay)
    {
        Map map;
        if (isHallWay) map = mapTree.hallWay;
        else map = mapTree.map;

        Tilemap wall = map.wall;
        Tilemap tile = map.tile;

        Vector2Int leftDown = map.leftDown;
        Vector2Int rightUp = map.rightUp;
        Vector2Int plusXY = Vector2Int.zero;

        switch (curDoor.z)
        {
            case 0:
                plusXY.x = curDoor.x + -pos.x;
                plusXY.y = curDoor.y + Mathf.Abs(pos.y);
                break;

            case 90:
                plusXY.x = curDoor.x + Mathf.Abs(pos.x);
                plusXY.y = curDoor.y + -pos.y;
                break;

            case 180:
                plusXY.x = curDoor.x + -pos.x;
                plusXY.y = curDoor.y - Mathf.Abs(pos.y);
                break;

            case 270:
                plusXY.x = curDoor.x - Mathf.Abs(pos.x);
                plusXY.y = curDoor.y + -pos.y;
                break;
        }


        for (int i = leftDown.x; i <= rightUp.x; i++)
        {
            for (int j = leftDown.y; j <= rightUp.y; j++)
            {
                int x = i + plusXY.x;
                int y = j + plusXY.y;

                if (isHallWay && wallBase.HasTile(new Vector3Int(x, y)))
                    if (tile.HasTile(new Vector3Int(i, j)) || wall.HasTile(new Vector3Int(i, j)))
                        wallBase.SetTile(new Vector3Int(x, y), null);

                if (!isHallWay && (wallBase.HasTile(new Vector3Int(x, y)) || tileBase.HasTile(new Vector3Int(x, y))))
                    continue;

                if (wall.HasTile(new Vector3Int(i, j)))
                    wallBase.SetTile(new Vector3Int(x, y), wall.GetTile(new Vector3Int(i, j)));

                if (tile.HasTile(new Vector3Int(i, j)))
                    tileBase.SetTile(new Vector3Int(x, y), tile.GetTile(new Vector3Int(i, j)));
            }
        }

        Vector3 colliderPos = new(leftDown.x + plusXY.x + wall.size.x / 2, leftDown.y + plusXY.y + wall.size.y / 2);

        if (Mathf.Abs(leftDown.x) != Mathf.Abs(rightUp.x)) colliderPos.x += -0.5f;
        if (Mathf.Abs(leftDown.y) != Mathf.Abs(rightUp.y)) colliderPos.y += -0.5f;

        Vector2 size = (Vector2Int)tile.size;

        var temp = Instantiate(enterRoom, colliderPos, Quaternion.identity, spawn).GetComponent<Enter_Room>();
        if (isHallWay && curDoor.z % 180 == 0)
            size = new Vector2(size.x, size.y - 4);
        else if (isHallWay && curDoor.z % 180 == 90)
            size = new Vector2(size.x - 4, size.y);

        if (!isHallWay && isExit) Instantiate(exitStage, colliderPos, Quaternion.identity, temp.transform);

        var temp2 = Instantiate(blind, colliderPos, Quaternion.identity, temp.transform);
        if (!isHallWay) temp2.transform.localScale = size + new Vector2(4, 4);
        else
        {
            Vector2 plusSize;
            if (curDoor.z % 180 == 0) plusSize = new Vector2(2, 0);
            else plusSize = new Vector2(0, 2);

            temp2.transform.localScale = size + plusSize;
        }

        temp.Setting(size, pos, mapTree, temp2, isHallWay);
    }

    bool CheckMap(Vector3Int curDoor, Vector3Int pos, Map map, bool isHallWay)
    {
        Vector2Int leftDown = map.leftDown;
        Vector2Int rightUp = map.rightUp;
        Vector2Int plusXY = Vector2Int.zero;

        switch (curDoor.z)
        {
            case 0:
                plusXY.x = curDoor.x + -pos.x;
                plusXY.y = curDoor.y + Mathf.Abs(pos.y);
                break;

            case 90:
                plusXY.x = curDoor.x + Mathf.Abs(pos.x);
                plusXY.y = curDoor.y + -pos.y;
                break;

            case 180:
                plusXY.x = curDoor.x + -pos.x;
                plusXY.y = curDoor.y - Mathf.Abs(pos.y);
                break;

            case 270:
                plusXY.x = curDoor.x - Mathf.Abs(pos.x);
                plusXY.y = curDoor.y + -pos.y;
                break;
        }

        if (isHallWay)
            switch (curDoor.z)
            {
                case 0: leftDown.y += 1; break;
                case 90: leftDown.x += 1; break;
                case 180: rightUp.y += -1; break;
                case 270: rightUp.x += -1; break;
            }
        else
        {
            leftDown += new Vector2Int(-1, -1);
            rightUp += new Vector2Int(1, 1);
        }

        for (int i = leftDown.x; i <= rightUp.x; i++)
        {
            for (int j = leftDown.y; j <= rightUp.y; j++)
            {
                int x = i + plusXY.x;
                int y = j + plusXY.y;

                if (wallBase.HasTile(new Vector3Int(x, y)) || tileBase.HasTile(new Vector3Int(x, y)))
                    return true;

                //testBase.SetTile(new Vector3Int(x, y), testTile);
            }
        }
        return false;
    }
}
