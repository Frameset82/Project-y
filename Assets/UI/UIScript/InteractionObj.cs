using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObj : MonoBehaviour
{
    public GameObject interactionCanvas;    // 상호작용 가능표시 UI
    public GameObject uiCanvas;             // 상호작용 UI
    UserInterface userInterface;     // 추상 클래스 UserInterface
    public UserInterface originUi;     

    void Start() {
        userInterface = originUi;
        userInterface.interactionObj = this;
    }
    // 플레이어 충돌 체크
    // 플레이어 충돌 시 상호작용 가능표시 UI 활성화
    // 플레이어 충돌 시 컨트롤러로 UI 할당
    void OnTriggerEnter(Collider coll) {
        if(coll.tag == "Player"){
            interactionCanvas.SetActive(true);
            coll.GetComponent<PlayerController>().OnInteractionEnter(this);
        }
    }

    // 플레이어 충돌 종료 체크
    // 플레이어 충돌 종료 시 상호작용 가능표시 UI 비활성화
    // 플레이어 충돌 종료 시 컨트롤러로 전달한 UI 초기화
    void OnTriggerExit(Collider coll) {
        if(coll.tag == "Player"){
            interactionCanvas.SetActive(false);
            coll.GetComponent<PlayerController>().OnInteractionExit();
        }
    }

    // UI 활성화
    public void ActiveUI(){
        userInterface.OpenUI();
    }
    // UI 비활성화
    public void InactiveUI(){
        userInterface.CloseUI();
        userInterface = originUi;
    }
    public void ChageCanvas(UserInterface ui) {
        userInterface = ui;
        userInterface.interactionObj = this;
    }
}
