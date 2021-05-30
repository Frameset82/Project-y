using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearWeapon : MeleeWeapon
{
    private Damage damage;

    public override void OnActive()
    {
        print("온액티브");
        damage.dType = Damage.DamageType.NuckBack; //데미지 종류 설정
        damage.ccTime = 2f;
        for (int i = 0; i < enemies.Count; i++)
        {
            damage.hitPoint = enemies[i].GetComponent<Collider>().ClosestPoint(transform.position);
            damage.hitNormal = transform.position - enemies[i].transform.position;
            enemies[i].GetComponent<LivingEntity>().OnDamage(damage);
        }
    }

    public override void OnAttack()
    {
        damage.dType = Damage.DamageType.Melee; //데미지 종류 설정
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
    }
    private void OnEnable()
    {
        damage.dValue = 10f; //초기 데미지값 설정
    }

    // Update is called once per frame
    void Update()
    {
        CollisionCheck();
    }
}
