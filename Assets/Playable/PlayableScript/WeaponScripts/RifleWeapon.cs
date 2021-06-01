using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleWeapon : RangeWeapon
{
    private Damage damage;
    private RuntimeAnimatorController anim;

    public override void OnAttack()
    {
        //데미지 처리 구조체 오류
        //Bullet.rangeHitTarget.GetComponent<LivingEntity>().OnDamage(damage + playerInfo.damage, transform.forward, transform.forward);
    }

    public override void OnActive()
    {
        throw new System.NotImplementedException();
    }

    private void OnEnable()
    {
        damage.dValue = 10f; //초기 데미지값 설정
        damage.dType = Damage.DamageType.Melee; //데미지 종류 설정
    }

    // Update is called once per frame
    void Update()
    {

    }
}
