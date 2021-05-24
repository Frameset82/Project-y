using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UserInterface : MonoBehaviour
{
    public GameObject uiCanvas;

    public void OpenUI(){
        if(PlayerKeyboardController.isInteraction == true) return;
        uiCanvas.SetActive(true);
        PlayerKeyboardController.isInteraction = true;
    }

    public void CloseUI(){
        uiCanvas.SetActive(false);
        PlayerKeyboardController.isInteraction = false;
    }
}
