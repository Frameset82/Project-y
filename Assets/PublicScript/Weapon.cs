using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour, IDamageSource
{
    [SerializeField]
    public string weaponName; //무기 이름
    public string explain; // 아이템 설명
    public Sprite weaponSprite; // 무기 스프라이트
    public float damageValue; //데미지량
    public float delay; // 딜레이
    public float attackSpeed = 1.0f; //공격속도
    public bool canAttack; //공격가능 유무
    protected Damage damage;
    [Header("장착 무기 타입")]
    public WeaponType wType;

    [Header("플레이어")]
    public Transform tr; // 무기가 들어갈 위치
    public GameObject trGameObject;
    [HideInInspector]public GameObject player;
    [HideInInspector]public PlayerEquipmentManager playerEquipmentManager;
    [HideInInspector]public PlayerInfo playerInfo;

    public enum WeaponType // 플레이어 상태 리스트
    {
        Melee = 0,
        Sword = 1,
        Spear = 2,              
        Rifle = 3
    }

    public void SetPlayer(GameObject playerObj)
    {
        player = playerObj;
        playerEquipmentManager = player.GetComponent<PlayerEquipmentManager>();
        playerInfo = player.GetComponent<PlayerInfo>();
    }

    public abstract void OnAttack(); // 공격 기능
    public abstract void OnActive(); // 무기 액티브 스킬 기능
    public abstract void TrChange(); // 무기 위치 교체
    public abstract void OnEquip(); // 무기 장착
    public abstract void UnEquip(); // 무기 해제
}
