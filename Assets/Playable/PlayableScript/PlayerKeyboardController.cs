﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyboardController : MonoBehaviour
{
    // 이동속도와 회전속도 회피속도
    public float dodgePower = 400f;
    public ParticleSystem ps;
    // 회피명령 딜레이 변수
    private float timeBetDodge = 1f;
    private float nextDodgeableTime = 0f;

    public InteractionObj targetInterObj {get; private set;}
    public static bool isInteraction;

    private PlayerKeyboardInput playerKeyboardInput; // 플레이어 입력 컴포넌트
    private PlayerEquipmentManager playerEquipmentManager;
    public Rigidbody playerRigidbody; // 캐릭터 리지드바디
    private PlayerAnimation playerAnimation;
    public GameObject playerAvatar;

    public PlayerState pState;

    public Transform FirePos; // 투사체 발사 위치
    public Transform FirePos2; // 총구 화염 위치

    public float currentAttackTime = 0.0f;
    public int comboCnt = 0;

    public GameObject effect; //총구 화염 이펙트

    public enum PlayerState // 플레이어 상태 리스트
    {
        Idle, // 가만히 서있는 상태
        Movement, // 이동중인 상태
        Dodge, // 회피중인 상태
        Attack, // 공격중인 상태
        onHit, // 맞고있는 상태
        Death, // 사망한 상태
        Swap // 스왑 상태
    }

    // 사용할 컴포넌트 할당(애니메이터는 수동할당)
    private void Start()
    {
        playerKeyboardInput = GetComponent<PlayerKeyboardInput>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        
    }

    public void OnInteractionEnter(InteractionObj interObj){
        targetInterObj = interObj;
    }

    public void OnInteractionExit(){
        targetInterObj.InactiveUI();
        targetInterObj = null;
    }
    
    // // 캐릭터 이동감지
    // private void FixedUpdate()
    // {

    // }

    // 캐릭터 이동명령
    public void Move()
    {
        if (pState == PlayerState.Idle )
        {
            pState = PlayerState.Movement;
            if (PlayerKeyboardInput.isSwap)
                pState = PlayerState.Idle;
        }
    }
    public void unMove()
    {
        if (pState == PlayerState.Movement)
        {
            pState = PlayerState.Idle;
        }
    }

    // 캐릭터 회피명령
    public void Dodge(Vector3 dir)
    {
        if ((pState == PlayerState.Idle || pState == PlayerState.Movement || pState == PlayerState.Attack) && Time.time >= nextDodgeableTime)
        {
            if(pState == PlayerState.Attack)
            {
                playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
                playerAnimation.playerAnimator.SetBool("isAttack", false);
                comboCnt = 0;
                PlayerKeyboardInput.isShoot = false;
                playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
            PlayerKeyboardInput.isDodge = true;
            nextDodgeableTime = Time.time + timeBetDodge;
            StartCoroutine(DodgeCoroutine(dir));
        }
    }
    // 회피중 이동명령 예약
    private bool willMove = false;
    public void WillMove()
    {
        willMove = true;
    }
    // 캐릭터 실제 회피
    public IEnumerator DodgeCoroutine(Vector3 dir)
    {
        /*        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;*/
        PlayerInfo.canDamage = false; // 무적으로 전환
        pState = PlayerState.Dodge;
        willMove = false;
        /*        Vector3 power = (destination - transform.position).normalized * dodgePower;*/
        playerRigidbody.AddForce(dir.normalized * dodgePower);
        playerRigidbody.velocity = Vector3.zero;
        playerAnimation.DodgeAni();
        yield return new WaitForSeconds(0.5f); // 회피 지속시간
        PlayerInfo.canDamage = true; // 비무적으로 전환
        playerRigidbody.velocity = Vector3.zero; // 가속도 초기화
        PlayerKeyboardInput.isDodge = false;
/*        playerRigidbody.constraints = RigidbodyConstraints.None;*/

        // 회피중 이동명령을 받았는지 체크
        if (willMove)
        {
            pState = PlayerState.Movement;
        }
        else
        {
            pState = PlayerState.Idle;
        }
    }

    public void Attack(Vector3 destination)
    {
        if (pState == PlayerState.Idle || pState == PlayerState.Movement || pState == PlayerState.Attack)
        {
            StartCoroutine(AttackCoroutine(destination));
        }
    }

    // 실제 공격
    public IEnumerator AttackCoroutine(Vector3 destination)
    {
/*        playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;*/
        PlayerKeyboardInput.isShoot = true;
        pState = PlayerState.Attack;

        gameObject.transform.LookAt(destination);

        if (playerEquipmentManager.equipWeapon == null)
        {
            Debug.Log("무기없음");
            yield return new WaitForSeconds(0.0f);
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isRifle == true)
        {
            playerAnimation.Attack();
            ps.Emit(1);
            
            for (int i = 0; i < 3; i++)
            {
               /* Instantiate(effect, FirePos2.transform.position, FirePos2.transform.rotation);*/
                /*Destroy(effect);*/
                CreateBullet(); //총알 생성하기
                yield return new WaitForSeconds(0.1f);
            }
            
            yield return new WaitForSeconds(0.2f); // 딜레이

            PlayerKeyboardInput.isShoot = false;
            pState = PlayerState.Idle;
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isGun == true)
        {
            playerAnimation.Attack();
            CreateBullet(); //총알 생성하기
            yield return new WaitForSeconds(0.1f);

            PlayerKeyboardInput.isShoot = false;
            yield return new WaitForSeconds(0.3f);
            pState = PlayerState.Idle;
            
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isMelee == true)
        {
            playerAnimation.playerAnimator.SetBool("isAttack", true);
            currentAttackTime = Time.time; // 재생한 시점
            if (Time.time - currentAttackTime < 2f) // 공격 애니메이션 재생 후 1초가 지나지 않았다면
            {
                Debug.Log(Time.time - currentAttackTime + " 콤보 이어짐");
                comboCnt += 1;
                comboCnt = Mathf.Clamp(comboCnt, 0, 3); // 0~3으로 제한  
                playerAnimation.playerAnimator.SetInteger("ComboCnt", comboCnt);
                if (comboCnt == 3)
                {
                    yield return new WaitForSeconds(0.2f);
                }
            }
            yield return new WaitForSeconds(0.1f);
            playerEquipmentManager.equipWeaponScript.OnAttack();
            PlayerKeyboardInput.isShoot = false;
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isSword == true)
        {
            playerAnimation.playerAnimator.SetBool("isAttack", true);
            currentAttackTime = Time.time; // 재생한 시점 
            if (Time.time - currentAttackTime < 2f) // 공격 애니메이션 재생 후 1초가 지나지 않았다면
            {
                Debug.Log(Time.time - currentAttackTime + " 콤보 이어짐");
                comboCnt += 1;
                comboCnt = Mathf.Clamp(comboCnt, 0, 3);  // 0~3으로 제한
                playerAnimation.playerAnimator.SetInteger("ComboCnt", comboCnt);
                if (comboCnt == 3) 
                {
                    yield return new WaitForSeconds(0.67f);
                }   
            }
            yield return new WaitForSeconds(0.5f);
            playerEquipmentManager.equipWeaponScript.OnAttack();
            PlayerKeyboardInput.isShoot = false;
            
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isSpear == true)
        { 
            playerAnimation.playerAnimator.SetBool("isAttack", true);
            currentAttackTime = Time.time; // 재생한 시점
            if (Time.time - currentAttackTime < 2f) // 공격 애니메이션 재생 후 1초가 지나지 않았다면
            {
                Debug.Log(Time.time - currentAttackTime + " 콤보 이어짐");
                comboCnt += 1;
                comboCnt = Mathf.Clamp(comboCnt, 0, 3); // 0~3으로 제한
                playerAnimation.playerAnimator.SetInteger("ComboCnt", comboCnt);
                playerAnimation.playerAnimator.SetInteger("ComboCnt", comboCnt);
                if (comboCnt == 3)
                {
                    yield return new WaitForSeconds(0.67f);
                }
            }
            yield return new WaitForSeconds(0.4f);
            playerEquipmentManager.equipWeaponScript.OnAttack();
            PlayerKeyboardInput.isShoot = false;
        }
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void RightAttack(Vector3 destination)
    {
        if (pState == PlayerState.Idle || pState == PlayerState.Movement || pState == PlayerState.Attack)
        {
            StartCoroutine(RightAttackCoroutine(destination));
        }
    }

    public IEnumerator RightAttackCoroutine(Vector3 destination)
    {
        PlayerKeyboardInput.isShoot = true;
        pState = PlayerState.Attack;

        gameObject.transform.LookAt(destination);

        if (playerEquipmentManager.equipWeapon == null)
        {
            Debug.Log("무기없음");
            yield return new WaitForSeconds(0.0f);
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isRifle == true)
        {
            playerAnimation.RightAttack();
            yield return new WaitForSeconds(0.2f); // 딜레이
            PlayerKeyboardInput.isShoot = false;
            pState = PlayerState.Idle;
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isGun == true)
        {
            playerAnimation.RightAttack();
            PlayerKeyboardInput.isShoot = false;
            yield return new WaitForSeconds(0.3f);
            pState = PlayerState.Idle;

        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isMelee == true)
        {
            playerAnimation.RightAttack();
            PlayerKeyboardInput.isShoot = false;
            yield return new WaitForSeconds(0.3f);
            pState = PlayerState.Idle;
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isSword == true)
        {
            playerAnimation.RightAttack();
            PlayerKeyboardInput.isShoot = false;
            yield return new WaitForSeconds(0.3f);
            pState = PlayerState.Idle;

        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isSpear == true)
        {
            playerAnimation.RightAttack();
            PlayerKeyboardInput.isShoot = false;
            yield return new WaitForSeconds(0.3f);
            pState = PlayerState.Idle;
        }
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    //오브젝트풀에서 총알 가져와서 생성하기
    public void CreateBullet()
    {
        var Bulletobj = BulletObjectPool.GetBullet(); // 오브젝트 풀에서 총알 가져오기
        Bulletobj.transform.position = FirePos.transform.position; //위치 지정
        Bulletobj.transform.rotation = FirePos.transform.rotation;// 회전 지정
        
    }

    public void ComboMove()
    {
        playerRigidbody.AddForce(transform.forward * 300f);
        playerRigidbody.velocity = Vector3.zero;
    }
}
