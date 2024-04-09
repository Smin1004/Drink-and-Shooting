using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun_UI : MonoBehaviour
{
    [SerializeField] List<GameObject> gunBox = new List<GameObject>();
    public List<Sprite> gun_sprite = new List<Sprite>();

    Text curAmmo;
    Player p;
    GameObject move;
    MainCanvas canvas;

    public int gun_index;
    public float moveTime;

    public List<Coroutine> gunBoxMoveCoroutines = new List<Coroutine>();

    private void Start()
    {
        p = Player.Instance;
        canvas = MainCanvas.Instance;

        curAmmo = canvas.curAmmo;
        gun_index = 0;
        InputSprite();
        for(int i = 1; i < gunBox.Count; i++){
            gunBox[i].transform.localPosition = new Vector3(3.5f, -4.5f);
            gunBox[i].SetActive(false);
        }
    }

    public void InputSprite()
    {
        gun_index = Player.Instance.weaponIndex;
        int j = gun_index;
        for (int i = 0; i < gunBox.Count; i++)
        {
            if (i > 3)
            {
                j = 7 - i - gun_index;
                do
                {
                    j = Mathf.Abs(gun_sprite.Count - j);
                } while (j < 0 || j > gun_sprite.Count - 1);
                gunBox[i].GetComponent<SpriteRenderer>().sprite = gun_sprite[j];
                continue;
            }


            if (j > gun_sprite.Count - 1)
                j = 0;


            gunBox[i].GetComponent<SpriteRenderer>().sprite = gun_sprite[j];

            j++;
        }
    }

    public IEnumerator InSetting(){
        float x = -11.5f;
        float y = -0.75f;
        curAmmo.gameObject.SetActive(false);

        for(int i = 1; i < gunBox.Count; i++){
            gunBox[i].SetActive(true);
            if(i < 4){
                gunBoxMoveCoroutines.Add(StartCoroutine(MoveTo(gunBox[i], new Vector3(3.5f, y), moveTime / 6, false)));
                y += 3.75f;
            }
            else{
                gunBoxMoveCoroutines.Add(StartCoroutine(MoveTo(gunBox[i], new Vector3(x, -4.5f), moveTime / 6, false)));
                x += 5;
            }
        }

        yield return new WaitForSeconds(moveTime / 6);
    }

    public IEnumerator OutSetting(){
        for(int i = gunBoxMoveCoroutines.Count - 1; i >= 0; i--) StopCoroutine(gunBoxMoveCoroutines[i]);
        gunBoxMoveCoroutines.Clear();
        for(int i = 1; i < gunBox.Count; i++)
        {
            StartCoroutine(MoveTo(gunBox[i], new Vector3(3.5f, -4.5f), moveTime, false));
        }
        yield return new WaitForSeconds(moveTime);
        for(int i = 1; i < gunBox.Count; i++){
            gunBox[i].SetActive(false);
        }
        curAmmo.gameObject.SetActive(true);
    }

    public IEnumerator Bookmark(){
        curAmmo.gameObject.SetActive(false);
        yield return StartCoroutine(MoveTo(gunBox[0], new Vector3(13.5f, -4.5f), moveTime, false));

        p.weaponIndex = p.subBookmarkIndex;
        p.subBookmarkIndex = p.mainBookmarkIndex;
        p.mainBookmarkIndex = p.weaponIndex;
        InputSprite();
        canvas.magazine.WeaponChange();
        
        yield return StartCoroutine(MoveTo(gunBox[0], new Vector3(3.5f, -4.5f), moveTime, true));
        curAmmo.gameObject.SetActive(true);
    }

    public IEnumerator MoveUp()
    {
        if (gun_index != 0) gun_index--;
        else gun_index = gun_sprite.Count - 1;
        for (int i = gunBox.Count - 1; i >= 0; i--)
        {
            if (i == 3)
            {
                int index = gun_index;
                for(int j = 0; j < 3; j++){
                    index--;
                    if(index < 0) index = gun_sprite.Count - 1;
                }

                gunBox[i].GetComponent<SpriteRenderer>().sprite = gun_sprite[index];
                gunBox[i].transform.localPosition = new Vector3(-16.5f, -4.5f);
                StartCoroutine(MoveTo(gunBox[i], gunBox[i].transform.localPosition + new Vector3(5, 0), moveTime / 5, true));
            }
            else if (i < 3)
                StartCoroutine(MoveTo(gunBox[i], gunBox[i].transform.localPosition + new Vector3(0, 3.75f), moveTime / 5, true));
            else
                StartCoroutine(MoveTo(gunBox[i], gunBox[i].transform.localPosition + new Vector3(5, 0), moveTime / 5, true));

            if (i == gunBox.Count - 1) move = gunBox[i];
            else gunBox[i + 1] = gunBox[i];
        }

        gunBox[0] = move;
        yield return new WaitForSeconds(moveTime / 5);
    }

    public IEnumerator MoveDown()
    {
        if (gun_index < gun_sprite.Count - 1) gun_index++;
        else gun_index = 0;
        for (int i = 0; i < gunBox.Count; i++)
        {
            if (i == 4)
            {
                int index = gun_index + 3;
                while (index > gun_sprite.Count - 1 || index < 0)
                {
                    index = Mathf.Abs(gun_sprite.Count - index);
                }

                gunBox[i].GetComponent<SpriteRenderer>().sprite = gun_sprite[index];
                gunBox[i].transform.localPosition = new Vector3(3.5f, 10.5f);
                StartCoroutine(MoveTo(gunBox[i], gunBox[i].transform.localPosition + new Vector3(0, -3.75f), moveTime / 5, true));
            }
            else if (i < 4 && i != 0)
                StartCoroutine(MoveTo(gunBox[i], gunBox[i].transform.localPosition + new Vector3(0, -3.75f), moveTime / 5, true));
            else
                StartCoroutine(MoveTo(gunBox[i], gunBox[i].transform.localPosition + new Vector3(-5, 0), moveTime / 5, true));

            if (i == 0) move = gunBox[i];
            else gunBox[i - 1] = gunBox[i];
        }

        gunBox[gunBox.Count - 1] = move;
        yield return new WaitForSeconds(moveTime / 5);
    }

    public IEnumerator MoveTo(GameObject obj, Vector3 target, float sec, bool isScroll)
    {
        float timer = 0f;
        Vector3 start = obj.transform.localPosition;

        while (timer <= sec)
        {
            obj.transform.localPosition = Vector3.Lerp(start, target, Easing.easeInOutQuart(timer / sec));
            timer += Time.deltaTime;

            if (timer >= sec * 0.5f && isScroll) p.WeaponSprite();
            yield return null;
        }

        yield break;
    }

}
