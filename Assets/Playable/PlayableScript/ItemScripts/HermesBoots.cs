using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermesBoots : ActiveItem
{
    private PlayerInfo pInfo; //플레이어 인포 변수
    private float prevMove; // 플레이어의 기본 이동 속도를 저장할 변수

    void Init()  //아이템 초기화 함수
    {
        pInfo = PlayerKeyboardInput.playerInfo;
    }

    public override void OnActive() //아이템 사용시 구현할 함수
    {
        prevMove = pInfo.MoveSpeed;

        StartCoroutine(BootsUpdate());
    }

    IEnumerator BootsUpdate() //10초동안 이속 상승
    {

        pInfo.MoveSpeed += (prevMove * 10) / 100;

        yield return new WaitForSeconds(10f);

        pInfo.MoveSpeed = prevMove;

    }

}
