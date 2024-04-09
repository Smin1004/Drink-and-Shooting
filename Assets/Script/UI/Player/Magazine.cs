using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magazine : MonoBehaviour
{
    [SerializeField] GameObject down;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject up;
    [SerializeField] Transform startPos;
    Gun_Base now;

    [SerializeField]List<GameObject> magazineObj;
    [SerializeField]List<Vector3> magazinePos;

    public void WeaponChange(){
        if(magazineObj != null){
            for(int i = 0; i < magazineObj.Count; i++){
                Destroy(magazineObj[i]);
            }
        }
        now = Player.Instance.weaponList[Player.Instance.weaponIndex];
        bullet = now.bullet_ui;
        magazinePos = new List<Vector3>();
        magazinePos.Add(startPos.position);
        for(int i = 1; i < now.magazine + 2; i++){
            magazinePos.Add(startPos.position + new Vector3(0, i * bullet.transform.localScale.y * 2));
        }
        SettingMagazine();
        CurMagazin();
    }

    void SettingMagazine(){
        magazineObj = new List<GameObject>();
        
        for(int i = 0; i < magazinePos.Count; i++){
            if(i == 0){
                var temp = Instantiate(down, magazinePos[i], Quaternion.identity, startPos);
                magazineObj.Add(temp);
            }
            else if(i == magazinePos.Count - 1){
                var temp = Instantiate(up, magazinePos[i], Quaternion.identity, startPos);
                magazineObj.Add(temp);
            }
            else{
                var temp = Instantiate(bullet, magazinePos[i], Quaternion.identity, startPos);
                magazineObj.Add(temp);
            }
        }
    }

    public void CurMagazin(){
        for(int i = 1; i < magazinePos.Count - 1; i++){
            if(now.cur_ammo < i){
                magazineObj[i].GetComponent<SpriteRenderer>().color = new Color(0,0,0);
            }else{
                magazineObj[i].GetComponent<SpriteRenderer>().color = new Color(255,255,255);
            }
        }
    }
}
