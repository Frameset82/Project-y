﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class WeaponKeyboard : MeleeWeapon    //상속
{
    private Damage damage; // Damgae 스크립트
    public PhotonView pv;

    public Vector3 currPos = Vector3.zero;
    public Quaternion currRot = Quaternion.identity;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        /*        pv.ObservedComponents[0] = gameObject.transform;*/

        currPos = transform.position;
        currRot = transform.rotation;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }

    public override void OnActive() // 우클릭 할당
    {
        damage.dType = Damage.DamageType.NuckBack; //데미지 종류 설정
        damage.ccTime = 2f; //cc 시간 
        for (int i = 0; i < enemies.Count; i++) //공격 범위에 있는 애들한테 데미지 처리(복붙)
        {
            damage.hitPoint = enemies[i].GetComponent<Collider>().ClosestPoint(transform.position);
            damage.hitNormal = transform.position - enemies[i].transform.position;
            enemies[i].GetComponent<LivingEntity>().OnDamage(damage);
        }
    }

    public override void OnAttack()
    {
        damage.dType = Damage.DamageType.Melee; //데미지 종류 설정 (좌클릭)
        for (int i = 0; i < enemies.Count; i++)
        {
            playerInfo.RestoreHealth(1f); // 도란검 효과
            damage.hitPoint = enemies[i].GetComponent<Collider>().ClosestPoint(transform.position);
            damage.hitNormal = transform.position - enemies[i].transform.position;
            enemies[i].GetComponent<LivingEntity>().OnDamage(damage);
        }
    }

    [PunRPC]
    public void DamageCarculate()
    {

    }

    [PunRPC]
    public override void TrChange()
    {
        if (player == null)
            return;
        trGameObject = GameObject.Find(player.name + "/Male/Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/" + gameObject.name);
        if (trGameObject != null)
            tr = trGameObject.transform;
    }

    public override void OnEquip() // 기본 효과들
    {
        pv = GetComponent<PhotonView>();
        damageValue = 10f; // 도란검 기본 공격력
        prevDamage = damageValue + playerInfo.defaultDamage;
        damage.dValue = prevDamage; //초기 데미지값 설정
        weaponTrChanged = true;
        playerInfo.maxHealth += 20f; //최대 체력 올라가는거
        playerInfo.CalculateHealthPoint(); // 체력바 동기화
    }

    public override void UnEquip()
    {
        pv = null;
        weaponTrChanged = false;
        playerInfo.maxHealth -= 20f;
        playerInfo.CalculateHealthPoint(); // 체력바 동기화
    }

    void Update()
    {
        CollisionCheck();
        if (weaponTrChanged == false)
        {
            TrChange();
        }
        if (player != null)
        {
            if (damageValue + playerInfo.defaultDamage != prevDamage)
            {
                prevDamage = damageValue + playerInfo.defaultDamage;
                damage.dValue = prevDamage;
            }
        }
    }
}
