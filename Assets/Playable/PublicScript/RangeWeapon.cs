using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : Weapon
{
    public override void OnAttack()
    {
        Bullet.rangeHitTarget.GetComponent<LivingEntity>().OnDamage(5.0f, transform.forward, transform.forward);
    }
    
    public override void OnActive()
    {
        throw new System.NotImplementedException();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
