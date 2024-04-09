using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private static Player _instance = null;
    public static Player Instance => _instance;

    [Header("Objects")]
    public Transform left_Hend;
    public Transform right_Hend;
    public Transform target;
    [SerializeField] GameObject dash_obj;
    P_UI pui;
    MainCanvas canvas;
    Magazine magazine;
    Rigidbody2D rigid;
    Animator anim;
    Vector3 pos;

    [Header("Status")]
    public int HP;
    public float moveSpeed;
    [SerializeField] float dashSpeed;
    [SerializeField] float invincible_Time;

    [Header("Weapon")]
    public Transform gunPos;
    public Gun_Base nowWeapon;
    public int weaponIndex;
    public List<Gun_Base> weaponList = new List<Gun_Base>();
    public List<Gun_Base> prefabList = new List<Gun_Base>();
    public int mainBookmarkIndex;
    public int subBookmarkIndex;

    [Header("Values")]
    [SerializeField] Vector2 checkbox_Size;
    [SerializeField] float cur_ui_delay;
    float x, y;

    bool isDashNow;
    bool isChange;
    bool isDamage;
    bool isSlow;
    bool isMap;
    public bool isDash;
    public bool isActive = false;

    [Header("Pos")]
    [SerializeField] Vector3 localPosition;
    [SerializeField] Vector2 dir;

    [Header("Sound")]
    [SerializeField] private AudioClip roll;
    [SerializeField] private AudioClip hit;

    public void _Instance()
    {
        _instance = this;
    }

    private void Start()
    {
        canvas = MainCanvas.Instance;
        pui = canvas.pui;
        magazine = canvas.magazine;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        HP = Data.Instance.HP;
        for(int i = 0; i < Data.Instance.weaponList.Count; i++){
            prefabList.Add(Data.Instance.weaponList[i]);
            var temp = Instantiate(Data.Instance.weaponList[i], gunPos).GetComponent<Gun_Base>();
            temp.isActive = true;
            temp.cur_ammo = Data.Instance.gunDataList[i].cur_ammo;
            temp.remain_ammo = Data.Instance.gunDataList[i].remain_ammo;
            temp.gameObject.SetActive(false);
            weaponList.Add(temp);
            canvas.gun_ui.gun_sprite.Add(temp.ui);
        }

        nowWeapon = weaponList[weaponIndex];
        nowWeapon.gameObject.SetActive(true);
        canvas.gun_ui.InputSprite();
        
        magazine.WeaponChange();
    }

    void Update()
    {
        pos = transform.position;
        if (!isActive) return;

        Move();
        Gun_UI();
        Map_UI();
        nowWeapon.fire();
        ItemCheck();
        if(Input.GetKeyDown(KeyCode.T)){
            HP = 6;
            pui.SettingHP(HP);
        }
    }

    void Move()
    {
        if (isSlow || isDash) return;

        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");

        if(x == 0 && y == 0) anim.SetBool("IsWalk", false);
        else anim.SetBool("IsWalk", true);

        dir = target.position - pos;
        float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    
        if(z > 50 && z < 135) anim.SetInteger("NEWS", 0);
        else if(z > -50 && z < 50) anim.SetInteger("NEWS", 90);
        else if(z > -130 && z < -50) anim.SetInteger("NEWS", 180);
        else anim.SetInteger("NEWS", 270);

        if ((x != 0 || y != 0) && Input.GetMouseButtonDown(1))
        {
            rigid.velocity = Vector2.zero;
            StartCoroutine("Dash");
            return;
        }

        Vector3 nor = new Vector3(x, y, 0f).normalized;
        //if(!isDashNow) transform.Translate(nor * moveSpeed * Time.deltaTime);
        if (!isDashNow) rigid.velocity = new Vector2(nor.x * moveSpeed, nor.y * moveSpeed);

    }

    IEnumerator Dash()
    {
        if (isDashNow) yield break;
        isDashNow = true;
        isDash = true;

        Vector3 curXY = new Vector3((int)x, (int)y).normalized;
        anim.SetBool("IsRoll", true);
        SoundManager.Instance.Sound(roll, false, 1);
        yield return StartCoroutine(DashMove(transform.position + new Vector3(curXY.x * 5, curXY.y * 5), dashSpeed));

        anim.SetBool("IsRoll", false);
        isDashNow = false;
    }
    
    public void Damage()
    {
        if (isDash) return;

        StartCoroutine(damage());

        IEnumerator damage()
        {

            if (isDamage) yield break;

            isDamage = true;

            if(isActive) pui.Damage();
            SoundManager.Instance.Sound(hit, false, 0.5f);

            HP--;
            pui.SettingHP(HP);
            if (HP < 1) AllStop(true);
            yield return new WaitForSeconds(invincible_Time);
            isDamage = false;
        }
    }

    public void AllStop(bool isDie)
    {
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        anim.SetBool("IsWalk", false);
        anim.SetBool("IsRoll", false);
        if(isDie) anim.SetBool("IsDie", true);
        isActive = false;
        nowWeapon.isActive = false;
        pui.isActive = false;
        if(isDie) GameManager.Instance.Die();
    }

    IEnumerator Slow(int type)
    {
        switch (type)
        {
            //InSetting
            case 0:
                isChange = true;
                pui.Slow(true);
                isSlow = true;
                yield return StartCoroutine(canvas.gun_ui.InSetting());
                isChange = false;
                break;
            //OutSetting
            case 1:
                isChange = true;
                StopCoroutine(StartCoroutine(canvas.gun_ui.InSetting()));
                if (mainBookmarkIndex != weaponIndex)
                {
                    subBookmarkIndex = mainBookmarkIndex;
                    mainBookmarkIndex = weaponIndex;
                }
                pui.Slow(false);
                isSlow = false;
                yield return StartCoroutine(canvas.gun_ui.OutSetting());
                isChange = false;
                break;
            //Bookmark
            case 2:
                if (isChange) break;
                isChange = true;
                yield return StartCoroutine(canvas.gun_ui.Bookmark());
                isChange = false;
                break;
        }
    }

    IEnumerator WeaponChange(Vector2 scroll)
    {
        if (isChange) yield break;

        if (scroll.y > 0)
        {
            isChange = true;
            if (weaponIndex > 0) weaponIndex--;
            else weaponIndex = weaponList.Count - 1;
            yield return StartCoroutine(canvas.gun_ui.MoveUp());
            magazine.WeaponChange();
            isChange = false;
        }
        else if (scroll.y < 0)
        {
            isChange = true;
            if (weaponIndex < weaponList.Count - 1) weaponIndex++;
            else weaponIndex = 0;
            yield return StartCoroutine(canvas.gun_ui.MoveDown());
            magazine.WeaponChange();
            isChange = false;
        }
    }

    public void WeaponSprite()
    {
        nowWeapon.Change();
        nowWeapon.gameObject.SetActive(false);
        nowWeapon = weaponList[weaponIndex];
        nowWeapon.gameObject.SetActive(true);
        if (nowWeapon.TryGetComponent<ICharging>(out var charging))
            charging.Setting();

        if (!isSlow) mainBookmarkIndex = weaponIndex;
    }

    void Gun_UI()
    {
        if (Input.GetKey(KeyCode.LeftControl) && !isSlow)
        {
            if (cur_ui_delay > 0.2f)
                StartCoroutine(Slow(0));
            else cur_ui_delay += Time.deltaTime;
        }
        else if (!Input.GetKey(KeyCode.LeftControl) && cur_ui_delay != 0)
        {
            if (cur_ui_delay > 0.2f)
                StartCoroutine(Slow(1));
            else
                StartCoroutine(Slow(2));

            cur_ui_delay = 0;
        }

        if (!isSlow) return;
        Vector2 Scroll = Input.mouseScrollDelta;
        StartCoroutine(WeaponChange(Scroll));
    }

    void Map_UI()
    {
        if(Input.GetKeyDown(KeyCode.Tab)){
            pui.Map(true);
            isMap = true;
        }
        else if(Input.GetKeyUp(KeyCode.Tab)){
            pui.Map(false);
            isMap = false;
        }

        if(!isMap) return;
        Vector2 Scroll = Input.mouseScrollDelta;
        pui.MapCam(Scroll);
    }

    void ItemCheck()
    {
        var hits = Physics2D.OverlapBoxAll(pos, checkbox_Size, 0);
        foreach (var item in hits)
        {
            if (item.TryGetComponent<IItme>(out var hit))
            {
                if (Input.GetKeyDown(KeyCode.E)) hit.Use();
            }
        }
    }

    IEnumerator DashMove(Vector3 target, float sec)
    {
        float timer = 0f;
        Vector3 start = pos;
        bool isWall = false;

        while (timer <= sec)
        {
            Vector3 nextPos = Vector3.Lerp(start, target, Easing.easeOutQuint(timer / sec));

            var hits = Physics2D.OverlapBoxAll(nextPos, new Vector2(0.9f, 0.9f), 0);
            foreach (var item in hits)
            {
                if (item.CompareTag("Wall")) isWall = true;
            }
            if (isWall) nextPos = transform.position;

            if (timer >= sec * 0.65f && isDash) isDash = false;

            transform.position = nextPos;
            timer += Time.deltaTime;
            if (timer > sec * 0.9f) yield break;
            yield return null;
        }

        yield break;
    }

}
