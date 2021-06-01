using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceAmericano : ActiveItem
{
    private PlayerInfo pInfo; //플레이어 인포 변수
    private float prevAttack; //플레이어의 기본 공격 속도를 저장할 변수
    private float prevMove; // 플레이어의 기본 이동 속도를 저장할 변수

    void Init()  //아이템 초기화 함수
    {
        pInfo = PlayerKeyboardInput.playerInfo;
    }
    public override void OnActive() //아이템 사용시 구현할 함수
    {
        prevAttack = pInfo.AtkSpeed; 
        prevMove = pInfo.MoveSpeed;
   
        StartCoroutine(AmericanoActive());
    }



    IEnumerator AmericanoActive() //15초 동안 플레이어의 이동속도 공격속도 상승
    {
        pInfo.AtkSpeed += (prevAttack * 20) / 100;
        pInfo.MoveSpeed += (prevMove * 20) / 100;
        
        yield return new WaitForSeconds(15f);
        
        pInfo.AtkSpeed = prevAttack;
        pInfo.MoveSpeed = prevMove;
  
    }
}
