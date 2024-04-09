using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Gun_Base : MonoBehaviour
{
    [Header("Gun_Base")]
    [Header("Obj")]
    public Transform me;
    public Sprite ui;
    public GameObject bullet_ui;

    [SerializeField] protected Transform startpos;
    [SerializeField] protected Bullet_Base bullet;

    protected Transform left;
    protected Transform right;
    protected Magazine _magazine;
    protected Animator anim;
    protected Vector3 target;
    Player p;
    Text ammo_text;
    Camera main_camera;
    MainCanvas canvas;



    [Header("Status")]
    [Header("Delay")]
    [SerializeField] protected float damage;
    [SerializeField] protected float max_bullet_delay;
    [SerializeField] float max_rerode_delay;
    [SerializeField] float minus_rerode_delay;
    [SerializeField] float camMove_waitTime;
    protected float cur_bullet_delay;
    protected float cur_rerode_delay;

    [Header("Ammo")]
    public int magazine;
    public int cur_ammo;
    public int remain_ammo;
    [SerializeField] int max_ammo;

    [Header("Min : Max")]
    [SerializeField] protected int dir_ran_min;
    [SerializeField] protected int dir_ran_max;

    bool isFest;
    bool isCilck;
    bool isRerode;
    public bool isActive = true;
    protected bool isRight;
    [SerializeField] protected bool isInfinite;

    [Header("Sound")]
    [SerializeField] protected AudioClip _fire;
    [SerializeField] private AudioClip relode;

    [Header("Pos")]
    public Vector3 _startpos;
    protected Vector3 dir_gun;
    protected float rot;

    protected virtual void Start()
    {
        anim = me.gameObject.GetComponent<Animator>();
        p = Player.Instance;
        canvas = MainCanvas.Instance;

        ammo_text = canvas.curAmmo;
        _magazine = canvas.magazine;
        left = p.left_Hend;
        right = p.right_Hend;
        main_camera = canvas.main_camera;

        canvas.rerode.SetFill(0);
    }

    protected virtual void Update()
    {
        target = p.target.position;

        if (!isActive) return;

        Spin();
        if (isInfinite) ammo_text.text = new string("Infinite");
        else ammo_text.text = new string(remain_ammo + " / " + max_ammo);

        if (isRerode) Reload();
        if (cur_bullet_delay < max_bullet_delay) cur_bullet_delay += Time.deltaTime;
    }

    public void Change()
    {
        isRerode = false;
        MainCanvas.Instance.rerode.SetFill(0);
    }

    void Reload()
    {
        cur_rerode_delay += Time.deltaTime;
        canvas.rerode.SetFill(1 - cur_rerode_delay / max_rerode_delay);
        if (cur_rerode_delay > max_rerode_delay && isRerode)
        {
            isRerode = false;
            if (remain_ammo > magazine)
                cur_ammo = magazine;
            else
                cur_ammo = remain_ammo;

            if (isFest)
            {
                max_rerode_delay += minus_rerode_delay;
                isFest = false;
            }
            _magazine.CurMagazin();
        }
    }

    public void fire()
    {
        if (!isActive) return;

        if (!isRerode)
        {
            if ((cur_ammo == 0 && Input.GetMouseButtonDown(0)))
            {
                isRerode = true;
                SoundManager.Instance.Sound(relode, false, 1);
                cur_rerode_delay = 0;
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                isRerode = true;
                isFest = true;
                max_rerode_delay -= minus_rerode_delay;
                SoundManager.Instance.Sound(relode, false, 1);
                cur_rerode_delay = 0;
            }
        }
        else return;

        if (Input.GetMouseButtonUp(0) && !isCilck)
        {
            isCilck = true;
            Click();
        }
        if (!Input.GetMouseButton(0)) return;
        if (cur_bullet_delay < max_bullet_delay) return;
        Shot();
        isCilck = false;
    }

    protected abstract void Spin();

    protected abstract void Click();

    protected abstract void Shot();
}
