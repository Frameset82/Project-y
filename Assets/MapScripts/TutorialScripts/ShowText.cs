using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowText : MonoBehaviour
{
    public GameObject TextUI; //상호작용 설명 UI
    public GameObject ExPanel;  //기본키 설명UI
    private Transform cam; //카메라 트랜스폼
    private Transform UITrans; //설명 텍스트 트랜스폼

    void Start()
    {
        cam = Camera.main.transform;
        UITrans = TextUI.GetComponent<Transform>();
        TextUI.SetActive(false);
        ExPanel.SetActive(false);
    }

   
    void Update()
    {
        UITrans.LookAt(UITrans.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);

      
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            TextUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.G))
            {
                ExPanel.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            TextUI.SetActive(false);
            ExPanel.SetActive(false);
        }
    }
}
