using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour, IDamageSource
{
    [SerializeField]
    public float damege;
    public float delay;
    public float attackSpeed = 1.0f;
    public bool canAttack;
    [Header("장착 무기 타입")]
    public bool isMelee;
    public bool isSword;
    public bool isSpear;
    public bool isGun;
    public bool isRifle;

    public abstract void OnAttack();    // 공격 기능
    public abstract void OnActive();    // 무기 액티브 스킬 기능
}
