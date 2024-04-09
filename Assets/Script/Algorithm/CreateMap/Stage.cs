using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Stage", menuName = "Game/Stage", order = int.MinValue), Serializable]
public class Stage : ScriptableObject
{
    [Header("Start : 0  Boss : 1  Exit : 2  Shop : 3  Chest : 4")]
    public List<Map> specialMap = new List<Map>();
    public List<Map> battleMap = new List<Map>();
    //public List<Map> eventMap = new List<Map>();
    public List<Map> hallWay = new List<Map>();
    public List<Map> bossMap = new List<Map>();

    [Header("SpecialMap Count")]
    public int shopCount;
    public int chest01Count;
    public int chest02Count;
    public int bossRoomCount;

    [Header("Map Count")]
    public int maxBattleRoomCount;
    public bool isNotRandom;
}
