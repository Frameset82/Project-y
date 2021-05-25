﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : LivingEntity
{
    [Header("체력바 관련")]
    public Slider healthSlider; // 체력바
    public GameObject healthText; // 체력 텍스트
    [Header("플레이어 기본 속성들")]
    public float maxHealth; // 최대체력( 시작 시 기본체력 )
    public float defaultDamage; // 기본 데미지
    public float AtkSpeed; // 공격속도
    public float MoveSpeed; // 이동속도
    [Header("플레이어 추가 속성들")]
    public float shield; // 보호막
    public float criProbability; // 치명타 확률
    public float criDamage; // 치명타 데미지

    public static bool canDamage = true; // 데미지를 받을 수 있는 상태

    private PlayerAnimation playerAnimation;
    private PlayerKeyboardController playerKeyboardController;
    private Damage damage;
    public float timer = 0f;

    private void Awake()
    {
        startingHealth = 1000.0f; // 시작체력
        maxHealth = startingHealth;
        playerAnimation = GetComponent<PlayerAnimation>();
        playerKeyboardController = GetComponent<PlayerKeyboardController>();
        damage.dValue = 10f; //초기 데미지값 설정(발판)
    }

    protected override void OnEnable() // PlayerHealth 컴포넌트가 활성화될때마다 실행
    {
        // LivingEntity의 OnEnable() 실행 (상태 초기화)
        base.OnEnable();

        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = startingHealth; // 초기체력으로 설정
        healthSlider.value = health; // 현재 체력으로 설정
    }

    public override void RestoreHealth(float newHealth)
    {
        // LivingEntity의 RestoreHealth() 실행 (체력증가)
        base.RestoreHealth(newHealth);

        healthSlider.value = health; // 갱신된 체력으로 슬라이더 갱신
    }


    public override void OnDamage(Damage dInfo)
    {
        if (canDamage)
        {
            if (dInfo.dType == Damage.DamageType.NuckBack && !dead) // 넉백공격일때
            {
                PlayerKeyboardInput.maxCcTime = dInfo.ccTime;
                playerAnimation.OnNuckBack();
                PlayerKeyboardInput.onNuckBack = true;
                playerKeyboardController.pState = PlayerKeyboardController.PlayerState.onCC;
                playerKeyboardController.NuckBackMove();
            }
            else if((dInfo.dType == Damage.DamageType.Melee || dInfo.dType == Damage.DamageType.None) && !dead) // 일반공격일때
            {
                canDamage = false;
                PlayerKeyboardInput.onHit = true;
                playerAnimation.OnHit();
            }
            else if(dInfo.dType == Damage.DamageType.Stun && !dead)
            {
                PlayerKeyboardInput.maxCcTime = dInfo.ccTime;
                playerAnimation.OnStun();
                PlayerKeyboardInput.onStun = true;
                playerKeyboardController.pState = PlayerKeyboardController.PlayerState.onCC;
            }
        }

        if (canDamage)
        {
            health -= dInfo.dValue; //체력 감소
            healthSlider.value = health;
        }
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public override void Die()
    {
        base.Die();
        playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Death;
        playerAnimation.Dead();
        healthSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        healthText.GetComponent<Text>().text = healthSlider.value + " / " + maxHealth; // 체력 갱신

        if (!canDamage) // 무적시간 계산
        {
            timer += Time.deltaTime;
        }
        if (timer >= 1.5f)
        {
            canDamage = true;
            PlayerKeyboardInput.onHit = false;
            timer = 0.0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            GetComponent<LivingEntity>().OnDamage(damage);
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "HealKit")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                RestoreHealth(5.0f);
            }
        }
    }
}
