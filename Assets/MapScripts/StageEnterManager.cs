using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEnterManager : MonoBehaviour
{
    [SerializeField] GameObject stageEnterCanvas;
    InteractionObj interactionObj;

    void Start()
    {
        interactionObj = GetComponent<InteractionObj>();
    }

    
    void Update()
    {
        
    }
}
