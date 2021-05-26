using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour, IDamageSource
{
    [SerializeField]
    public string weaponName; //무기 이름
    public string explain; // 아이템 설명
    public float damageValue; //데미지량
    public float delay; // 딜레이
    public float attackSpeed = 1.0f; //공격속도
    public bool canAttack; //공격가능 유무
    [Header("장착 무기 타입")]
    public bool isMelee;
    public bool isSword;
    public bool isSpear;
    public bool isGun;
    public bool isRifle;

    [Header("플레이어")]
    public GameObject player;
    public PlayerAnimation playerAnimation;
    public PlayerInfo playerInfo; //플레이어 스탯

    public abstract void OnAttack();    // 공격 기능
    public abstract void OnActive();    // 무기 액티브 스킬 기능
    public abstract void ChangeAnimator(); // 애니메이터 바꾸기
}
