using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerInfo : LivingEntity
{
    private PlayerAnimation playerAnimation;
    private PlayerKeyboardController playerKeyboardController;
    private PlayerKeyboardInput playerKeyboardInput; 

    [Header("체력 속성")]
    public Material healthMaterial; // 체력 머테리얼
    public GameObject healthBar; // 체력 바
    public Text healthText; // 체력 수치 텍스트
    public GameObject playerUI; // 하단 UI
    [HideInInspector]public bool playerUIEnable = false;
    [Header("플레이어 기본 속성들")]
    public float maxHealth; // 최대체력( 시작 시 기본체력 )
    public float defaultDamage; // 기본 데미지
    public float AtkSpeed ; // 공격속도
    public float MoveSpeed = 7.0f ; // 추가 이동속도
    [Header("플레이어 추가 속성들")]
    public float shield; // 보호막
    public float criProbability; // 치명타 확률
    public float criDamage; // 치명타 데미지

    public bool canDamage = true; // 데미지를 받을 수 있는 상태

    private Damage damage;
    public float timer = 0f;
    public PhotonView pv = null;

    private void Awake()
    {
        startingHealth = 100.0f; // 시작체력
        maxHealth = startingHealth;
        health = startingHealth;

        playerAnimation = GetComponent<PlayerAnimation>();
        playerKeyboardController = GetComponent<PlayerKeyboardController>();
        playerKeyboardInput = GetComponent<PlayerKeyboardInput>();
        pv = GetComponent<PhotonView>();
        damage.dValue = 10f; //초기 데미지값 설정(발판)

        if (GameManager.isMulti && pv.IsMine)
            playerUI.SetActive(true);
        else
            playerUI.SetActive(true);
        CalculateHealthPoint();
    }

    protected override void OnEnable() // PlayerHealth 컴포넌트가 활성화될때마다 실행
    {
        // LivingEntity의 OnEnable() 실행 (상태 초기화)
        base.OnEnable();
        healthBar.gameObject.SetActive(true);
        CalculateHealthPoint();
    }

    public override void RestoreHealth(float newHealth)
    {
        // LivingEntity의 RestoreHealth() 실행 (체력증가)
        base.RestoreHealth(newHealth);
        CalculateHealthPoint(); // 체력 갱신
    }

    // 체력 변동시 남은 체력의 퍼센트 계산 후 UI 적용
    public void CalculateHealthPoint(){
        healthMaterial.SetFloat("_HeightPercent", health/maxHealth*100);
        healthText.text = health + " / " + maxHealth; // 체력 갱신
    }

    public override void OnDamage(Damage dInfo)
    {
        if (canDamage)
        {
            if (dInfo.dType == Damage.DamageType.NuckBack && !dead) // 넉백공격일때
            {
                playerKeyboardInput.isShoot = false;
                playerAnimation.playerAnimator.SetBool("isAttack", false);
                playerKeyboardController.comboCnt = 0;
                playerAnimation.playerAnimator.SetInteger("ComboCnt", playerKeyboardController.comboCnt);
                playerKeyboardInput.maxCcTime = dInfo.ccTime;
                playerAnimation.OnNuckBack();
                playerKeyboardInput.onNuckBack = true;
                playerKeyboardController.pState = PlayerKeyboardController.PlayerState.onCC;
                playerKeyboardController.NuckBackMove();
            }
            else if((dInfo.dType == Damage.DamageType.Melee || dInfo.dType == Damage.DamageType.None) && !dead) // 일반공격일때
            {
                playerKeyboardInput.isShoot = false;
                playerAnimation.playerAnimator.SetBool("isAttack", false);
                playerKeyboardController.comboCnt = 0;
                playerAnimation.playerAnimator.SetInteger("ComboCnt", playerKeyboardController.comboCnt);
                canDamage = false;
                playerKeyboardInput.onHit = true;
                playerAnimation.OnHit();
            }
            else if(dInfo.dType == Damage.DamageType.Stun && !dead)
            {
                playerKeyboardInput.isShoot = false;
                playerAnimation.playerAnimator.SetBool("isAttack", false);
                playerKeyboardController.comboCnt = 0;
                playerAnimation.playerAnimator.SetInteger("ComboCnt", playerKeyboardController.comboCnt);
                playerKeyboardInput.maxCcTime = dInfo.ccTime;
                playerAnimation.OnStun();
                playerKeyboardInput.onStun = true;
                playerKeyboardController.pState = PlayerKeyboardController.PlayerState.onCC;
            }
        }

        if (canDamage)
        {
            health -= dInfo.dValue; // 체력 감소
            CalculateHealthPoint(); // 체력 갱신
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
    }

    private void Update()
    {
        if (!canDamage) // 무적시간 계산
        {
            timer += Time.deltaTime;
        }
        if (timer >= 1.5f)
        {
            canDamage = true;
            playerKeyboardInput.onHit = false;
            timer = 0.0f;
        }
    }

   /* private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            GetComponent<LivingEntity>().OnDamage(damage);
            return;
        }
    }*/

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "HealKit")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                RestoreHealth(5.0f);
            }
        }
        /*if (other.tag == "Enemy")
        {
            GetComponent<LivingEntity>().OnDamage(damage);
            return;
        }*/
    }
}
