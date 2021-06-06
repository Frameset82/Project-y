using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    // 사운드 매니저 싱글톤
    private void Awake() {
        if(instance == null){
            instance = this;
        } else {
            if(instance!=this){
                Destroy(gameObject);
            }
        }
    }
    // 효과음 재생
    public void SFXPlay(string sfxName, AudioClip clip){
        GameObject go = new GameObject(sfxName + "Sound");
        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();
        Destroy(go,clip.length);
    }
}
