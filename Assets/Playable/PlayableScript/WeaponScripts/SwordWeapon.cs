using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : MeleeWeapon
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
            damage.hitPoint = enemies[i].GetComponent<Collider>().ClosestPoint(transform.position);
            damage.hitNormal = transform.position - enemies[i].transform.position;
            enemies[i].GetComponent<LivingEntity>().OnDamage(damage);
        }
    }
    private void Awake()
    {
        player = GameObject.Find("Player");
        playerAnimation = player.GetComponent<PlayerAnimation>();
        playerInfo = player.GetComponent<PlayerInfo>();
        anim = Resources.Load("PlayerAnimator/TestSword") as RuntimeAnimatorController;
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
