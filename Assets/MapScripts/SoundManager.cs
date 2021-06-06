using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioMixer audioMixer;
    public AudioSource bgSound;
    public AudioSource enviroSound;
    public AudioClip[] bgList;
    public AudioClip enviroClip;

    int bgNum=-1;

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
    public void SFXPlay(AudioClip clip, GameObject obj){
        if(obj==null || clip==null) return;
        AudioSource audioSource = obj.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.PlayOneShot(clip);
    }

    // 환경음 재생
    public void EnviroPlay(){
        EnviroStop();
        if(enviroClip!=null){
            enviroSound.clip = enviroClip;
            enviroSound.loop = true;
            enviroSound.Play();
        }
    }
    // 환경음 정지
    public void EnviroStop(){
        if(enviroSound!=null) return;
        enviroSound.Stop();
        enviroSound.clip = null;
    }

    // 배경음 재생
    public void BgmPlay(){
        BgmStop();
        if(bgList[bgNum+1]!=null){
            bgNum++;
            bgSound.clip = bgList[bgNum];
            bgSound.loop = true;
            bgSound.Play();
        }
    }
    
    // 배경음 정지
    public void BgmStop(){
        if(bgSound!=null) return;
        bgSound.Stop();
        bgSound.clip = null;
    }
}
