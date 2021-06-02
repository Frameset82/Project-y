using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoransShield : ActiveItem
{
    private PlayerInfo pInfo; //플레이어 인포 변수

    void Init()  //아이템 초기화 함수
    {
        pInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
    }

    public override void OnActive() //아이템 사용시 구현할 함수
    {
        pInfo.shield += 50;
    }

  
}
