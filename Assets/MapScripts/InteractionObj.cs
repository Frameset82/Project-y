using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObj : MonoBehaviour
{
    // 상호작용 UI 할당
    [SerializeField] GameObject interactionCanvas;
    // 상호작용 가능 판단
    public bool isInteractive = false;

    void Start()
    {
        
    }

    void OnTriggerEnter(Collider coll) {
        if(coll.tag == "Player"){
            interactionCanvas.SetActive(true);
            isInteractive = true;
        }
    }

    void OnTriggerExit(Collider coll) {
        if(coll.tag == "Player"){
            interactionCanvas.SetActive(false);
            isInteractive = false;
        }
    }
}
