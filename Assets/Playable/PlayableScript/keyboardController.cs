using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyboardController : MonoBehaviour
{
    // 이동속도와 회전속도 회피속도
    public float dodgePower = 400f;

    // 회피명령 딜레이 변수
    private float timeBetDodge = 1f;
    private float nextDodgeableTime = 0f;

    private keyboardInput keyboardInput; // 플레이어 입력 컴포넌트
    private PlayerEquipmentManager playerEquipmentManager;
    private Rigidbody playerRigidbody; // 캐릭터 리지드바디
    private PlayerAnimation playerAnimation;
    public GameObject playerAvatar;

    public PlayerState pState;

    public Transform FirePos; // 투사체 발사 위치
    bool isSwap;

    public float currentAttackTime = 0.0f;
    public int comboCnt = 0;

    public enum PlayerState // 플레이어 상태 리스트
    {
        Idle, // 가만히 서있는 상태
        Movement, // 이동중인 상태
        Dodge, // 회피중인 상태
        Attack, // 공격중인 상태
        onHit, // 맞고있는 상태
        Death // 사망한 상태
    }

    // 사용할 컴포넌트 할당(애니메이터는 수동할당)
    private void Start()
    {
        keyboardInput = GetComponent<keyboardInput>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
    }

    // 캐릭터 이동감지
    private void FixedUpdate()
    {

    }

    // 캐릭터 이동명령
    public void Move()
    {
        if (pState == PlayerState.Idle)
        {
            pState = PlayerState.Movement;
            if (isSwap)
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
    public void Dodge()
    {
        if ((pState == PlayerState.Idle || pState == PlayerState.Movement) && Time.time >= nextDodgeableTime)
        {
            nextDodgeableTime = Time.time + timeBetDodge;
            Vector3 destination = Vector3.forward;
            StartCoroutine(DodgeCoroutine(destination));
        }
    }
    // 회피중 이동명령 예약
    private bool willMove = false;
    public void WillMove()
    {
        willMove = true;
    }
    // 캐릭터 실제 회피
    public IEnumerator DodgeCoroutine(Vector3 destination)
    {
        /*        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;*/
        PlayerInfo.canDamage = false; // 무적으로 전환
        pState = PlayerState.Dodge;
        willMove = false;
        Vector3 power = (destination - transform.position).normalized * dodgePower;
        playerRigidbody.AddForce(gameObject.transform.forward * dodgePower);
        playerRigidbody.velocity = Vector3.zero;
        playerAnimation.DodgeAni();
        yield return new WaitForSeconds(.5f); // 회피 지속시간
        PlayerInfo.canDamage = true; // 비무적으로 전환
        playerRigidbody.velocity = Vector3.zero; // 가속도 초기화
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
        // 구르기 중이 아닐 때 사격 가능
        if (pState == PlayerState.Idle || pState == PlayerState.Movement || pState == PlayerState.Attack)
        {
            StartCoroutine(AttackCoroutine(destination));
        }
    }

    // 실제 공격
    public IEnumerator AttackCoroutine(Vector3 destination)
    {
        playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        keyboardInput.isShoot = true;
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
            
            for (int i = 0; i < 3; i++)
            {
                CreateBullet(); //총알 생성하기
                yield return new WaitForSeconds(0.1f);
            }
            
            yield return new WaitForSeconds(0.2f); // 딜레이
            
            keyboardInput.isShoot = false;
            pState = PlayerState.Idle;
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isGun == true)
        {
            playerAnimation.Attack();
            CreateBullet(); //총알 생성하기

            yield return new WaitForSeconds(0.2f); // 딜레이
            
            keyboardInput.isShoot = false;
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
            }
            yield return new WaitForSeconds(0f);
            playerEquipmentManager.weapon.OnAttack();
            keyboardInput.isShoot = false;
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isSword == true)
        {
            playerAnimation.Attack();
            yield return new WaitForSeconds(3.5f);
            playerEquipmentManager.weapon.OnAttack();
            keyboardInput.isShoot = false;
            pState = PlayerState.Idle;
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isSpear == true)
        {
            playerAnimation.Attack();
            yield return new WaitForSeconds(2.6f);
            playerEquipmentManager.weapon.OnAttack();
            keyboardInput.isShoot = false;
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

    private void Update()
    {

    }
}
