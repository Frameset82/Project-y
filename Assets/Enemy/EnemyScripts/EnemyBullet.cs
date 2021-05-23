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
        rigid.velocity = this.transform.forward * 50f;
    }

    private void FixedUpdate()
    {
      
        if (time> 4f)
        {
            ReturnToPool();
        }

        time += Time.deltaTime;
    }

  

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8) //충돌대상이 플레이어일때
        {
            LivingEntity enemytarget = other.gameObject.GetComponent<LivingEntity>();
            enemytarget.OnDamage(damage);
            ReturnToPool();
        }
  
    }

    private void ReturnToPool() //탄환 반납
    {
        switch(damage.dType)
        {
            case Damage.DamageType.Melee:
                ObjectPool.ReturnBullet(this);
                break;
            case Damage.DamageType.Stun:
                ObjectPool.ReturnSBullet(this);
                break;
        }
    }
}
