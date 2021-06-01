using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKControl : MonoBehaviour
{
    //public Transform gunPivot; // 총 배치의 기준점
    public Transform leftHandMount; // 총의 왼쪽 손잡이, 왼손이 위치할 지점
    public Transform rightHandMount; // 오른손으로 총 잡는 지점
    public Animator playerAnimator; // 애니메이터 컴포넌트
    private PlayerEquipmentManager playerEquipmentManager;
    private GameObject player;
    public string weaponName;
    private string weaponRoot = "Player/Male/Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/";

/*    void Start()
    {
        player = GameObject.Find("Player");
        playerEquipmentManager = player.GetComponent<PlayerEquipmentManager>();
    }

    private void Update()
    {
        playerAnimator = gameObject.GetComponent<Animator>();

        if (playerEquipmentManager.equipWeapon != null)
        {
            *//*if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isSpear == true)
            {
                weaponName = playerEquipmentManager.equipWeapon.name;
                leftHandMount = GameObject.Find(weaponRoot + weaponName + "/left").transform;
                rightHandMount = GameObject.Find(weaponRoot + weaponName + "/right").transform;
            }*/
            /*if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isMelee == true)
           {
               weaponName = playerEquipmentManager.equipWeapon.name;
               leftHandMount = GameObject.Find(weaponRoot + weaponName + "/left").transform;
               rightHandMount = GameObject.Find(weaponRoot + weaponName + "/right").transform;
           }*/
            /* if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isSword == true)
             {
                 weaponName = playerEquipmentManager.equipWeapon.name;
                 leftHandMount = GameObject.Find(weaponRoot + weaponName + "/left").transform;
                 rightHandMount = GameObject.Find(weaponRoot + weaponName + "/right").transform;
             }*/
/*            if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isGun == true)
            {
                weaponName = playerEquipmentManager.equipWeapon.name;
                leftHandMount = GameObject.Find(weaponRoot + weaponName + "/left").transform;
                rightHandMount = GameObject.Find(weaponRoot + weaponName + "/right").transform;
            }
            else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isRifle == true)
            {
                weaponName = playerEquipmentManager.equipWeapon.name;
                leftHandMount = GameObject.Find(weaponRoot + weaponName + "/left").transform;
                rightHandMount = GameObject.Find(weaponRoot + weaponName + "/right").transform;
            }*//*
        }
    }*/

/*    void OnAnimatorIK(int layerIndex)
    {
        if (playerEquipmentManager.equipWeaponScript.GetComponent<Weapon>().isRifle == true && playerEquipmentManager.equipWeaponScript.GetComponent<Weapon>().isGun == true)
        {
            // IK를 사용하여 왼손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

            playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position);
            playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);
            playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMount.position);
            playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMount.rotation);

            *//*playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, sleftHandMount.position);
            playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, sleftHandMount.rotation);
            playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, srightHandMount.position);
            playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, srightHandMount.rotation);*//*
        }
    }*/
}
