using System.Collections;
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
    public float damage; // 기본 데미지
    public float AtkSpeed; // 공격속도
    public float MoveSpeed; // 이동속도
    [Header("플레이어 추가 속성들")]
    public float shield; // 보호막
    public float criProbability; // 치명타 확률
    public float criDamage; // 치명타 데미지

    public static bool canDamage = true; // 데미지를 받을 수 있는 상태

    private PlayerAnimation playerAnimation;
    private keyboardController keyboardController;

    private void Awake()
    {
        startingHealth = 30.0f; // 시작체력
        maxHealth = startingHealth;
        playerAnimation = GetComponent<PlayerAnimation>();
        keyboardController = GetComponent<keyboardController>();
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

    /*
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (canDamage)
        {
            if (!dead)
            {
                // 사망하지않은경우에만 효과음 재생
            }

            base.OnDamage(damage, hitPoint, hitNormal);
            healthSlider.value = health;
        }
    } 이거 데미지 구조체로 바꾼거 때문에 오류남 고치셈*/

    public override void Die()
    {
        base.Die();
        keyboardController.pState = keyboardController.PlayerState.Death;
        playerAnimation.Dead();
        healthSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        healthText.GetComponent<Text>().text = healthSlider.value + " / " + maxHealth; // 체력 갱신
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            //OnDamage(10.0f, Vector3.forward, Vector3.forward); 요것도 오류나서 주석처리함
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
