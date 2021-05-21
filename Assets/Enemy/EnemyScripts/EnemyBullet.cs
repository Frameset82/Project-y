using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Rigidbody rigid;
    private float time;
    public Damage damage;
 

    private void Awake()
    {
        rigid = this.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        time = 0f; // 시간을 0으로 초기화
        rigid.velocity = this.transform.forward * 5f;
    }

    private void FixedUpdate()
    {
       
        if(time> 4f)
        {
            ObjectPool.ReturnBullet(this);
        }

        time += Time.deltaTime;
    }

  

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            //Debug.Log("aaaaaa");
            LivingEntity enemytarget = other.gameObject.GetComponent<LivingEntity>();
            enemytarget.OnDamage(damage);
            ObjectPool.ReturnBullet(this);
        }

        
    }
}
