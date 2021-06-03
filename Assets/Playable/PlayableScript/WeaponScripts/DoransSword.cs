using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoransSword : SwordWeapon
{

    private void OnEnable()
    {
        damage.dType = Damage.DamageType.Melee; //데미지 상태 설정
        damage.dValue = 10f; //초기 데미지값 설정

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

    public override void TrChange()
    {
    //    if (PlayerKeyboardInput.playerEquipmentManager.nearObject != null)
    //        trGameObject = GameObject.Find(PlayerKeyboardInput.player.name + "/Male/Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/" + gameObject.name);
    //    if (trGameObject != null)
    //        tr = trGameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        CollisionCheck();
        TrChange();
    }
}
