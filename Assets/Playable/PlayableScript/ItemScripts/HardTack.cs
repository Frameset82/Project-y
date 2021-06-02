using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardTack : ActiveItem
{
    private PlayerInfo pInfo; //플레이어 인포 변수
    private float prevDamage; //플레이어의 기본 공격력을 저장할 변수

    void Init()  //아이템 초기화 함수
    {
        pInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInfo>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Init();
        }
        if (Input.GetMouseButtonDown(1))
        {
            OnActive();
        }
    }
    public override void OnActive() //아이템 사용시 구현할 함수
    {
        prevDamage = pInfo.defaultDamage;
      
        StartCoroutine(HardTeckActive());
    }
    IEnumerator HardTeckActive()
    {
        pInfo.defaultDamage += 10;
        pInfo.shield += 100;
    
        yield return new WaitForSeconds(15f);

        pInfo.defaultDamage = prevDamage;
      
    }

}
