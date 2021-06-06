using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gungnir : MeleeWeapon
{
    private Damage damage;

    public override void OnActive()
    {
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

    public override void TrChange()
    {
        if (player == null)
            return;
        trGameObject = GameObject.Find(player.name + "/Male/Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/" + gameObject.name);
        if (trGameObject != null)
            tr = trGameObject.transform;
    }

    public override void OnEquip()
    {
        damageValue = 30f;
        prevDamage = damageValue + playerInfo.defaultDamage;
        damage.dValue = prevDamage; //초기 데미지값 설정
        weaponTrChanged = true;
    }

    public override void UnEquip()
    {
        weaponTrChanged = false;
    }

    // Update is called once per frame
    void Update()
    {
        CollisionCheck();
        if(weaponTrChanged == false)
        {
            TrChange();
        }
        if(player != null)
        {
            if (damageValue + playerInfo.defaultDamage != prevDamage)
            {
                prevDamage = damageValue + playerInfo.defaultDamage;
                damage.dValue = prevDamage;
            }
        }
    }
}
