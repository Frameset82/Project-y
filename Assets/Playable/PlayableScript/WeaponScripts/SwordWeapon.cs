using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : MeleeWeapon
{
    public PlayerInfo playerInfo;

    public override void OnActive()
    {
        throw new System.NotImplementedException();
    }

    public override void OnAttack()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            //데미지 구조체 변경 오류
           // enemies[i].GetComponent<LivingEntity>().OnDamage(damage + playerInfo.damage, Vector3.forward, Vector3.forward);
        }
    }
    private void Awake()
    {
        playerInfo = GameObject.Find("Player").GetComponent<PlayerInfo>();
        damage = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        CollisionCheck();
    }
}
