using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SwordGram : MeleeWeapon
{
    private Damage damage;
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
        playerInfo.maxHealth += 50f;
        playerInfo.CalculateHealthPoint(); // 체력바 동기화
    }

    public override void UnEquip()
    {
        weaponTrChanged = false;
        playerInfo.maxHealth -= 50f;
        playerInfo.CalculateHealthPoint(); // 체력바 동기화
    }

    // Update is called once per frame
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
