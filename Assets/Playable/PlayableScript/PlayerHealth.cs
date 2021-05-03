using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : LivingEntity
{
    public Slider healthSlider; // 체력바
    public GameObject healthText; // 체력 텍스트
    public float maxHealth;

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

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dead)
        {
            // 사망하지않은경우에만 효과음 재생
        }

        base.OnDamage(damage, hitPoint, hitNormal);
        healthSlider.value = health;
    }

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
            OnDamage(10.0f, Vector3.forward, Vector3.forward);
            /*Debug.Log("으악");*/
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "HealKit")
        {
           /* Debug.Log("접촉");*/
            if (Input.GetKeyDown(KeyCode.E))
            {
                RestoreHealth(5.0f);
               /* Debug.Log("오우야");*/
            }
        }
    }
}
