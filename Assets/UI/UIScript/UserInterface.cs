using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 모든 UI가 상속 받아야할 추상 클래스
public abstract class UserInterface : MonoBehaviour
{
    public GameObject uiCanvas; // UI 할당
    public InteractionObj interactionObj;
    public AudioSource uiAudioSource;
    public AudioClip hoverBtnClip;
    public AudioClip clickBtnClip;
    public AudioClip closeBtnClip;
    public AudioClip startBtnClip;
    
    // UI 활성화
    public void OpenUI(){
        if(PlayerController.isInteraction == true) return;
        uiCanvas.SetActive(true);
        PlayerController.isInteraction = true;
    }
    // UI 비활성화
    public virtual void CloseUI(){
        uiCanvas.SetActive(false);
        PlayerController.isInteraction = false;
    }
    // UI 변경
    public void ChangeUI(UserInterface ui){
        if(interactionObj != null) {
            interactionObj.ChageCanvas(ui);
        }
    }
    // 마우스 호버 클립 재생
    public void PlayHoverBtnClip(){
        SoundManager.instance.SFXPlay(hoverBtnClip, gameObject);
    }
    // 마우스 클릭 클립 재생
    public void PlayClickBtnClip(){
        SoundManager.instance.SFXPlay(clickBtnClip, gameObject);
    }
    // UI 닫기 클립 재생
    public void PlayCloseBtnClip(){
        SoundManager.instance.SFXPlay(closeBtnClip, gameObject);
    }
    public void PlayStartBtnClip(){
        SoundManager.instance.SFXPlay(startBtnClip, gameObject);
    }

}
