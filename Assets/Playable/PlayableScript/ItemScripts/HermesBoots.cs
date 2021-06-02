using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermesBoots : ActiveItem
{
    private float prevMove; // 플레이어의 기본 이동 속도를 저장할 변수

    void Init()  //아이템 초기화 함수
    {
       
    }

    public override void OnActive() //아이템 사용시 구현할 함수
    {
        prevMove = PlayerInfo.MoveSpeed;

        StartCoroutine(BootsUpdate());
    }

    IEnumerator BootsUpdate() //10초동안 이속 상승
    {

        PlayerInfo.MoveSpeed += (prevMove * 10) / 100;

        yield return new WaitForSeconds(10f);

        PlayerInfo.MoveSpeed = prevMove;

    }

}
