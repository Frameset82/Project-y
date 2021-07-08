using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStageEnter : UserInterface
{
    public Button soloModeBtn;             // 솔로 모드 버튼
    public Button multiModeBtn;            // 멀티 모드 버튼
    public Button escapeBtn;               // ESC 버튼
    public UserInterface uiNetworkManager; // 네트워크 UI
    public NetworkManager networkManager;

    // 버튼 클릭 메서드
    // 솔로 모드 버튼 클릭
    public void OnSoloMode(){
        //networkManager.LoadSceneProcess();
        Debug.Log("OnSoloMode()");
    }
    // 멀티 모드 버튼 클릭
    public void OnMultiMode(){
        CloseUI();
        uiNetworkManager.OpenUI();
        ChangeUI(uiNetworkManager);
    }
    // ESC 버튼 클릭
    public void ClickEscapeBtn(){
        CloseUI();
    }
}
