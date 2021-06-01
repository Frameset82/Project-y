using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoreanSpicyNoodle : ActiveItem
{
    private PlayerInfo pInfo; //플레이어 인포 변수
    private float prevAttack; //플레이어의 기본 공격 속도를 저장할 변수

    void Init()  //아이템 초기화 함수
    {
        pInfo = PlayerKeyboardInput.playerInfo;
    }

    public override void OnActive() //아이템 사용시 구현할 함수
    {
        prevAttack = pInfo.AtkSpeed;
    

        StartCoroutine(NoodleUpdate());
    }

    IEnumerator NoodleUpdate() 
    {
        pInfo.AtkSpeed += (prevAttack * 10) / 100;


        yield return new WaitForSeconds(8f);

        pInfo.AtkSpeed = prevAttack;
     

    }
}
