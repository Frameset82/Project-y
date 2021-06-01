using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M5MK2 : ActiveItem
{


    public float force;
    public GameObject GrenadePrefab;
    private Rigidbody rigid;
 

    void Init()
    {
        player = PlayerKeyboardInput.player;
      
    }
    public override void OnActive() //아이템 사용시 구현할 함수
    {
        var Grenade = Instantiate(GrenadePrefab, player.transform.position, Quaternion.identity);

        rigid = Grenade.GetComponent<Rigidbody>();
        rigid.AddForce(player.transform.forward * force, ForceMode.Impulse);

    
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Init();
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnActive();
        }
    }

   

   
}
