using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testgril1 : LivingEntity

{
    private Damage damage;

    public override void OnDamage(Damage dInfo)
    {
        health -= dInfo.dValue; //체력 감소
        print(health);
    }

    private void Start()
    {
        damage.dType = Damage.DamageType.Stun;
        damage.dValue = 10f;
        damage.hitNormal = gameObject.transform.position;
        damage.ccTime = 1.5f;
        startingHealth = 1000f;
        health = startingHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerKeyboardInput.player.GetComponent<LivingEntity>().OnDamage(damage);
        }
    }

    private void Update()
    {
        
    }
}
