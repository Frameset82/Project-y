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
    
    public Animator animator;

    public abstract void OnAttack();    // 공격 기능
    public abstract void OnActive();    // 무기 액티브 스킬 기능
}
