using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObj : MonoBehaviour
{
    // 상호작용 UI 할당
    [SerializeField] GameObject interactionCanvas;
    [SerializeField] GameObject uiCanvas;
    public UserInterface userInterface;

    void OnTriggerEnter(Collider coll) {
        if(coll.tag == "Player"){
            interactionCanvas.SetActive(true);
            coll.GetComponent<PlayerKeyboardController>().OnInteractionEnter(this);
        }
    }

    void OnTriggerExit(Collider coll) {
        if(coll.tag == "Player"){
            interactionCanvas.SetActive(false);
            coll.GetComponent<PlayerKeyboardController>().OnInteractionExit();
        }
    }

    public void ActiveUI(){
        userInterface.OpenUI();
    }

    public void InactiveUI(){
        userInterface.CloseUI();
    }
}
