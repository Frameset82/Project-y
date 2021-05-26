using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon1 : MeleeWeapon
{
    private Damage damage;
    private RuntimeAnimatorController anim;

    public override void OnActive()
    {
        throw new System.NotImplementedException();
    }

    public override void OnAttack()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            //데미지 구조체 오류처리
            //enemies[i].GetComponent<LivingEntity>().OnDamage(damage + playerInfo.damage, Vector3.forward, Vector3.forward);
        }
    }
    private void Awake()
    {
        player = GameObject.Find("Player");
        playerAnimation = player.GetComponent<PlayerAnimation>();
        playerInfo = player.GetComponent<PlayerInfo>();
        anim = Resources.Load("PlayerAnimator/TestMelee") as RuntimeAnimatorController;
    }

    private void OnEnable()
    {
        damage.dValue = 10f; //초기 데미지값 설정
        damage.dType = Damage.DamageType.Melee; //데미지 종류 설정
        playerAnimation.playerAnimator.runtimeAnimatorController = anim;
    }

    // Update is called once per frame
    void Update()
    {
        CollisionCheck();
    }
}
