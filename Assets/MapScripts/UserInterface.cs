﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 모든 UI가 상속 받아야할 추상 클래스
public abstract class UserInterface : MonoBehaviour
{
    public GameObject uiCanvas; // UI 할당
    public InteractionObj interactionObj;
    // UI 활성화
    public void OpenUI(){
        if(PlayerKeyboardController.isInteraction == true) return;
        uiCanvas.SetActive(true);
        PlayerKeyboardController.isInteraction = true;
    }
    // UI 비활성화
    public void CloseUI(){
        uiCanvas.SetActive(false);
        PlayerKeyboardController.isInteraction = false;
    }
    // UI 변경
    public void ChangeUI(UserInterface ui){
        if(interactionObj != null) {
            interactionObj.ChageCanvas(ui);
        }
    }
}
