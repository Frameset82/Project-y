using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleWeapon : RangeWeapon
{
    private Damage damage;

    public override void OnAttack()
    {
        //데미지 처리 구조체 오류
        //Bullet.rangeHitTarget.GetComponent<LivingEntity>().OnDamage(damage + playerInfo.damage, transform.forward, transform.forward);
    }

    public override void OnActive()
    {
        throw new System.NotImplementedException();
    }

    private void Awake()
    {
        player = GameObject.Find("Player");
        playerAnimation = player.GetComponent<PlayerAnimation>();
        playerInfo = player.GetComponent<PlayerInfo>();
        damage.dValue = 10f; //초기 데미지값 설정
        damage.dType = Damage.DamageType.Melee; //데미지 종류 설정
        playerAnimation.playerAnimator.runtimeAnimatorController = Resources.Load("PlayerAnimator/Rifle") as RuntimeAnimatorController;
    }

    public override void ChangeAnimator()
    {
        print("라이플");
        playerAnimation.playerAnimator.runtimeAnimatorController = Resources.Load("PlayerAnimator/Rifle") as RuntimeAnimatorController;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
