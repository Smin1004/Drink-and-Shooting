using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance => _instance;

    public List<Stage> stageList = new List<Stage>();
    public List<Door> doorList = new List<Door>();

    public void _Instance()
    {
        _instance = this;
    }

    public void Die(){
        MainCanvas.Instance.die.SetActive(true);
        MainCanvas.Instance.home.SetActive(true);
    }

    public void NextStage()
    {
        Debug.Log("exit");
        if (Data.Instance.stageIndex >= stageList.Count - 1)
        {
            Player.Instance.AllStop(false);
            MainCanvas.Instance.win.SetActive(true);
            MainCanvas.Instance.home.SetActive(true);
        }
        else
        {
            Data.Instance.HP = Player.Instance.HP;
            Data.Instance.weaponIndex = Player.Instance.weaponIndex;
            Data.Instance.weaponList = Player.Instance.prefabList;
            Data.Instance.gunDataList.Clear();
            foreach(var i in Player.Instance.weaponList){
                Data.Instance.gunDataList.Add(new gunData(i.cur_ammo, i.remain_ammo));
            }
            Data.Instance.stageIndex++;
            SceneManager.LoadScene("InGame");
        }
    }

    public void DoorOnOff(bool isON)
    {
        for (int i = 0; i < doorList.Count; i++)
        {
            if (isON)
            {
                doorList[i].OpenDoor();
                Player.Instance.moveSpeed = 8;
            }
            else
            {
                doorList[i].CloseDoor();
                Player.Instance.moveSpeed = 12;
            }
        }
    }
}
