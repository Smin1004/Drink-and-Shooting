using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Gun : MonoBehaviour, IItme
{
    public GameObject me;
    public GameObject preview;
    [SerializeField] Transform my_Gun_pos;
    [SerializeField] Transform p_Gun_pos;
    Player p;
    MainCanvas canvas;

    [SerializeField] Vector3 boxPos;
    [SerializeField] Vector2 boxSize;

    bool isUse;

    private void Start()
    {
        p = Player.Instance;
        canvas = MainCanvas.Instance;

        p_Gun_pos = p.gunPos;
        Instantiate(me, my_Gun_pos);

        StartCoroutine(Setting());
    }

    public void Use()
    {
        if (!isUse) return;

        Destroy(my_Gun_pos.gameObject);
        var temp = Instantiate(me, p_Gun_pos).GetComponent<Gun_Base>();
        p.prefabList.Add(me.GetComponent<Gun_Base>());
        p.weaponList.Add(temp);
        p.weaponIndex = Player.Instance.weaponList.Count - 1;
        p.WeaponSprite();
        temp.isActive = true;
        temp.gameObject.SetActive(true);
        canvas.gun_ui.gun_sprite.Add(me.GetComponent<Gun_Base>().ui);
        canvas.magazine.WeaponChange();
        canvas.gun_ui.InputSprite();
        Destroy(gameObject);
    }

    public IEnumerator Setting()
    {
        p.isActive = false;
        yield return MoveTo(transform.localPosition + new Vector3(0, 1), 0.3f);
        yield return MoveTo(transform.localPosition + new Vector3(0, -1), 0.3f);
        isUse = true;
        p.isActive = true;
    }

    IEnumerator MoveTo(Vector3 target, float sec)
    {
        float timer = 0f;
        Vector3 start = transform.position;

        while (timer <= sec)
        {
            transform.position = Vector3.Lerp(start, target, timer / sec);
            timer += Time.deltaTime;
            yield return null;
        }

        yield break;
    }
}
