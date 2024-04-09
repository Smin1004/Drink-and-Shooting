using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_UI : MonoBehaviour
{
    Camera main_camera;
    Camera miniMap_camera;

    List<Image> hp;
    GameObject map;
    RawImage mini_Map;
    Image shadow_ui;
    Image damage_ui;
    Player p;
    MainCanvas canvas;

    int curMapSize;

    bool isCamMove;
    public bool isActive = true;

    Vector3 target_pos;
    Vector3 player_pos;
    Vector3 original_campos;

    private void Start()
    {
        p = Player.Instance;
        canvas = MainCanvas.Instance;

        main_camera = canvas.main_camera;
        miniMap_camera = canvas.miniMap_camera;
        curMapSize = 40;
        hp = canvas.Hp;
        map = canvas.map;
        mini_Map = canvas.miniMap;
        shadow_ui = canvas.shadow_ui;
        damage_ui = canvas.damage_ui;
    }

    private void Update()
    {
        if (!isActive) return;

        target_pos = p.target.position;
        player_pos = p.transform.position;

        main_camera.transform.position =
        new Vector3(Mathf.Clamp(Vector3.Lerp(player_pos, target_pos, 0.3f).x, player_pos.x + -5, player_pos.x + 5),
        Mathf.Clamp(Vector3.Lerp(player_pos, target_pos, 0.3f).y, player_pos.y + -4.5f, player_pos.y + 4.5f), -10);
    }

    public void Slow(bool onoff)
    {
        if (onoff)
        {
            shadow_ui.gameObject.SetActive(true);
            Time.timeScale = 0.1f;
        }
        else
        {
            shadow_ui.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void Map(bool onoff)
    {
        if (onoff)
        {
            mini_Map.gameObject.SetActive(false);
            miniMap_camera.orthographicSize = curMapSize;
            map.SetActive(true);
        }
        else
        {
            mini_Map.gameObject.SetActive(true);
            curMapSize = (int)miniMap_camera.orthographicSize;
            miniMap_camera.orthographicSize = 18;
            map.SetActive(false);
        }
    }

    public void MapCam(Vector2 scroll)
    {
        if (scroll.y > 0)
        {
            if (miniMap_camera.orthographicSize > 30)
                miniMap_camera.orthographicSize--;
        }
        else if (scroll.y < 0)
        {
            if (miniMap_camera.orthographicSize < 108)
                miniMap_camera.orthographicSize++;
        }
    }

    public void SettingHP(int Hp)
    {
        int curHp = 0;
        for (int i = 0; i < hp.Count; i++)
        {
            hp[i].fillAmount = 0;
            if (Hp > curHp)
            {
                hp[i].fillAmount += 0.5f;
                curHp++;
            }
            if (Hp > curHp)
            {
                hp[i].fillAmount += 0.5f;
                curHp++;
            }
        }
    }

    public void Damage()
    {
        StopCoroutine(alpha(damage_ui, 1));
        StartCoroutine(alpha(damage_ui, 1));
    }

    IEnumerator alpha(Image image, int sec)
    {
        float timer = 0f;
        while (timer <= sec)
        {
            image.color = new Color(1, 1, 1, 1 - timer / sec);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
