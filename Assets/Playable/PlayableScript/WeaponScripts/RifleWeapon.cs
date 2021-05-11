using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleWeapon : RangeWeapon
{
    public PlayerInfo playerInfo;

    public override void OnAttack()
    {
        Bullet.rangeHitTarget.GetComponent<LivingEntity>().OnDamage(damage + playerInfo.damage, transform.forward, transform.forward);
    }

    public override void OnActive()
    {
        throw new System.NotImplementedException();
    }

    private void Awake()
    {
        playerInfo = GameObject.Find("Player").GetComponent<PlayerInfo>();
        damage = 10f;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
