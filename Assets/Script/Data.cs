using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Game/Data", order = int.MinValue), Serializable]
public class Data : ScriptableObject
{
    private static Data _instance = null;
    public static Data Instance => _instance;

    public int stageIndex;

    [Header("Player")]
    public int HP;
    public int weaponIndex;
    public List<Gun_Base> weaponList = new List<Gun_Base>();
    public List<gunData> gunDataList = new List<gunData>(); 
    [SerializeField] Gun_Base startGun;

    public void Reset(){
        stageIndex = 0;
        HP = 6;
        weaponIndex = 0;
        weaponList.Clear();
        weaponList.Add(startGun);
        gunDataList.Clear();
        gunDataList.Add(new gunData(startGun.cur_ammo, startGun.remain_ammo));
    }

    public void InitData()
    {
        _instance = this;
    }
}

[Serializable]
public class gunData
{
    public int cur_ammo;
    public int remain_ammo;

    public gunData(int _curAmmo, int _remainAmmo)
    {
        cur_ammo = _curAmmo;
        remain_ammo = _remainAmmo;
    }
}
