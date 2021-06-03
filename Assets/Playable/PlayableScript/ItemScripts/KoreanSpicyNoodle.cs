using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoreanSpicyNoodle : ActiveItem
{
    private float prevAttack; //플레이어의 기본 공격 속도를 저장할 변수

    void Init()  //아이템 초기화 함수
    {
       
    }

    public override void OnActive() //아이템 사용시 구현할 함수
    {
        prevAttack = playerInfo.AtkSpeed;
    

        StartCoroutine(NoodleUpdate());
    }

    IEnumerator NoodleUpdate() 
    {
        playerInfo.AtkSpeed += (prevAttack * 10) / 100;


        yield return new WaitForSeconds(8f);

        playerInfo.AtkSpeed = prevAttack;
     

    }
}
