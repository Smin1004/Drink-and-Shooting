using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance = null;
    public static SoundManager Instance => _instance;

    GameObject curBGM;

    public void _Instance(){
        _instance = this;
    }

    public void Sound(AudioClip clip, bool isLoop, float volume){
        GameObject obj = new GameObject("obj");
        AudioSource audio = obj.AddComponent<AudioSource>();
        audio.clip = clip;
        audio.loop = isLoop;
        audio.volume = volume;
        audio.Play();

        if(!isLoop) Destroy(obj, clip.length);
        else {
            if(curBGM != null) Destroy(curBGM);
            curBGM = obj;
        }
    }
}
