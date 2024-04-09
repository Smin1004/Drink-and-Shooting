using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class MainCanvas : MonoBehaviour
{
    private static MainCanvas _instance = null;
    public static MainCanvas Instance => _instance;

    [Header("Instance")]
    public SoundManager soundManager;
    public GameManager gameManager;
    public Player player;
    public Map_Generator map_Generator;
    public A_Manager a_Manager;
    public A_Map a_Map;

    [Header("TileMap")]
    public Tilemap wallBase;
    public Tilemap tileBase;
    public Tilemap testBase;
    public Tilemap mini_WallBase;
    public Tilemap mini_TileBase;
    public Tile testTile;
    public Tile mini_Wall;
    public Tile mini_Tile;

    [Header("obj")]
    public Camera main_camera;
    public Camera miniMap_camera;
    public List<Image> Hp;
    public GameObject map;
    public RawImage miniMap;
    public Image shadow_ui;
    public Image damage_ui;
    public Gun_UI gun_ui;
    public Magazine magazine;
    public GameObject win;
    public GameObject die;
    public GameObject home;
    public P_UI pui;
    public Rerode rerode;
    public Text curAmmo;

    [Header("sound")]
    [SerializeField] AudioClip bgm;

    public void Awake() {
        Setting();
    }

    void Setting(){
        _instance = this;
        soundManager._Instance();
        player._Instance();
        gameManager._Instance();
        map_Generator.StartCreateMap();
        a_Map._Instance();
        a_Manager._Instance();
        SoundManager.Instance.Sound(bgm, true, 0.115f);
    }

    public void homeButton(){
        Time.timeScale = 1;
        Cursor.visible = true;
        SceneManager.LoadScene("Title");
    }
}
