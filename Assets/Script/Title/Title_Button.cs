using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title_Button : MonoBehaviour
{
    [SerializeField] GameObject exitWindow_button;
    [SerializeField] Transform helpBox;
    [SerializeField] Transform creditBox;

    public bool isWindow = false;
    bool isCredit;

    public void GameStart()
    {
        if(isWindow) return;
        Data.Instance.Reset();
        SceneManager.LoadScene("InGame");
    }

    public void Help()
    {
        if(isWindow) return;
        isWindow = true;
        exitWindow_button.SetActive(true);
        helpBox.position = new Vector3(20, 0);
        StartCoroutine(MoveTo(helpBox, new Vector3(0,0), 1, 1));
    }

    public void Credit()
    {
        if(isWindow) return;
        isWindow = true;
        isCredit = true;
        exitWindow_button.SetActive(true);
        creditBox.position = new Vector3(-20,0);
        StartCoroutine(MoveTo(creditBox, new Vector3(0,0), 1, 1));
    }

    public void Exit()
    {
        if(isWindow) return;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Exit_Window(){
        exitWindow_button.SetActive(false);
        StartCoroutine(wait());
    }

    IEnumerator wait(){
        if(isCredit) {
            isCredit = false;
            yield return StartCoroutine(MoveTo(creditBox, new Vector3(20,0), 1, 0));
        }
        else yield return StartCoroutine(MoveTo(helpBox, new Vector3(-20,0), 1, 0));
        isWindow = false;
    }

    IEnumerator MoveTo(Transform moveObj, Vector3 target, float sec, int lerp)
    {
        float timer = 0f;
        Vector3 start = moveObj.position;

        while (timer <= sec)
        {
            switch (lerp)
            {
                case 0:
                    moveObj.position = Vector3.LerpUnclamped(start, target, Easing.easeInOutBack(timer / sec));
                    break;

                case 1:
                    moveObj.position = Vector3.LerpUnclamped(start, target, Easing.easeOutBack(timer / sec));
                    break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        yield break;
    }
}
