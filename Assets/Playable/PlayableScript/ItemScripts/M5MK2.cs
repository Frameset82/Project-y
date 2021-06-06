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
        player = GameObject.FindGameObjectWithTag("Player");
      
    }
    public override void OnActive() //아이템 사용시 구현할 함수
    {

        Vector3 pos = player.transform.position;
        pos.x += 0.5f;
        pos.y += 1f;
        var Grenade = Instantiate(GrenadePrefab, pos, Quaternion.identity);

        rigid = Grenade.GetComponent<Rigidbody>();
        rigid.AddForce(player.transform.forward * force, ForceMode.Impulse);

    
    }

    void Update()
    {
       if(Input.GetMouseButtonDown(0))
       {
           Init();
       }
       if(Input.GetMouseButtonDown(1))
       {
           OnActive();
       }
    }

    public override void OnEquip()
    {
        throw new System.NotImplementedException();
    }

    public override void UnEquip()
    {
        throw new System.NotImplementedException();
    }
}
