using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon1 : MeleeWeapon
{
    private Damage damage;
    private RuntimeAnimatorController anim;

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
        trGameObject = GameObject.Find(PlayerKeyboardInput.player.name + "/Male/Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/" + gameObject.name);
        if (trGameObject != null)
            tr = trGameObject.transform;
    }

    private void OnEnable()
    {
        damage.dValue = 10f; //초기 데미지값 설정
    }

    // Update is called once per frame
    void Update()
    {
        CollisionCheck();
        TrChange();
    }
}
