using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Logo : MonoBehaviour
{
    [SerializeField] Data data;
    [SerializeField] SoundManager sound;
    [SerializeField] AudioClip bgm;

    [SerializeField] GameObject canvas;
    [SerializeField] GameObject title;
    [SerializeField] GameObject fadeObj;

    private void Start() {
        sound._Instance();
        data.InitData();
        StartCoroutine(Intro());
    }

    IEnumerator Intro(){
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(MoveTo(new Vector3(0,0), 1, 1));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(MoveTo(new Vector3(0,8), 1, 0));
        SoundManager.Instance.Sound(bgm, true, 1);
        yield return StartCoroutine(fadeOut(fadeObj, 1));
        gameObject.SetActive(false);
    }

    IEnumerator MoveTo(Vector3 target, float sec, int lerp)
    {
        float timer = 0f;
        Vector3 start = transform.position;

        while (timer <= sec)
        {
            switch (lerp)
            {
                case 0:
                    transform.position = Vector3.LerpUnclamped(start, target, Easing.easeInOutBack(timer / sec));
                    break;

                case 1:
                    transform.position = Vector3.LerpUnclamped(start, target, Easing.easeOutBack(timer / sec));
                    break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        yield break;
    }

    public IEnumerator fadeOut(GameObject fade, float sec)
    {
        float timer = 0f;

        while (timer <= sec)
        {
            fade.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1 - timer / sec);
            timer += Time.deltaTime;
            if (timer >= sec * 0.5f) On();

            yield return null;
        }
        Destroy(fade);

        yield break;
    }

    void On(){
        canvas.SetActive(true);
        title.SetActive(true);
    }
}
