using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveItem : MonoBehaviour
{
    public GameObject player; // 플레이어 게임오브젝트
    public string itemName; // 아이템 이름
    public string explain; // 아이템 설명
    public string option; //  아이템 설정
    public Sprite ItemSprite; // 아이템 스프라이트
    public float coolTime; //쿨타임

    public virtual void OnActive() //아이템 사용시 구현할 함수
    {
        throw new System.NotImplementedException();
    }

  

    void Start()
    {
        
    }
}
