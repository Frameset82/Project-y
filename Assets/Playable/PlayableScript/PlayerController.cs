using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    // 이동속도와 회전속도 회피속도
    public float dashPower = 100f;
    // 회피명령 딜레이 변수
    private float timeBetDodge = 1f;
    private float nextDodgeableTime = 0f;

    public InteractionObj targetInterObj {get; private set;}
    public static bool isInteraction;
    public GameObject playerAvatar;

    public PlayerState pState;

    public Transform FirePos; // 투사체 발사 위치
    public Transform FirePos2; // 총구 화염 위치

    public float currentAttackTime = 0.0f;
    public int comboCnt = 0;

    public GameObject effect; //총구 화염 이펙트

    private Vector3 moveDirection;

    [HideInInspector] Vector3 moveVec; // 움직임 벡터
    [HideInInspector] public Vector3 moveVec1; // 상태 초기화용 벡터
    private Rigidbody playerRigidbody;

    public AudioClip[] footStepSound;
    public int footStepTemp;

    private PlayerEquipmentManager playerEquipmentManager;  // 플레이어 장비 관리 스크립트(?)
    private PlayerAnimation playerAnimation;  // 플레이어 애니메이션 관리 스크립트
    private PlayerInput playerInput;  // 플레이어 조작 입력단 스크립트

    private IEnumerator playerStateCoroutine;

    public enum PlayerState // 플레이어 상태 리스트
    {
        Idle, // 가만히 서있는 상태
        Move, // 이동 중인 상태
        Dodge, // 회피 중인 상태
        BasicAttack, // 공격 중인 상태
        SpecialAttack, // 우클릭 공격 중인 상태
        Hit, // 맞고 있는 상태
        Die, // 사망한 상태
        Swap, // 스왑 상태
        Stun, // CC 상태
    }

    public EffectInfo[] Effects;

    [Header("파티클")]
    [SerializeField]
    public GameObject particleParent;

    [Header("약진 파티클")]
    public GameObject dashParticle;
    public Transform dashParticleTransform;

    [Header("일반 공격 파티클")]
    public GameObject basicAttackParticle;
    public Transform basicAttackParticleTransform;

    [Header("특수 공격 파티클")]
    public GameObject specialAttackParticle;
    public Transform specialAttackParticleTransform;

    // 플레이어 행동 제약에 사용할 논리 변수
    [HideInInspector]
    public bool canMove { get; private set; } = true;
    [HideInInspector]
    public bool canBasicAttack { get; private set; } = true;
    [HideInInspector]
    public bool canSpecialAttack { get; private set; } = true;
    [HideInInspector]
    public bool canDodge { get; private set; } = true;

    void ActiveParticle(GameObject particle, Vector3 targetPosition, Quaternion targetRotation, float duration) {  // 파티클 생성 담당 메서드
        GameObject instancedParticle = Instantiate(particle, targetPosition, targetRotation);
        instancedParticle.transform.parent = particleParent.transform;
        Destroy(instancedParticle, duration);

    }
    private void Start()
    {
        particleParent = transform.Find("Particles Parent").gameObject;
        playerRigidbody = GetComponent<Rigidbody>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerInput = GetComponent<PlayerInput>();
    }

    // 상호작용 범위에 들어갔을 때
    public void OnInteractionEnter(InteractionObj interObj){
        targetInterObj = interObj;
    }
    // 상효작용 범위를 벗어났을 때
    public void OnInteractionExit(){
        targetInterObj.InactiveUI();
        targetInterObj = null;
    }

    public void SetMoveDirection(Vector3 direction) {
        moveDirection = direction;
    }

    public void Move() {
        if (moveDirection != Vector3.zero) {
            playerAnimation.playerAnimator.SetBool("isMove", true);
            if (canMove) {
                pState = PlayerState.Move;
            }
        } else {
            if(pState == PlayerState.Move) {
                playerAnimation.playerAnimator.SetBool("isMove", false);
            }
        }

        Vector3 heading = PlayerInput.mainCamera.transform.localRotation * Vector3.forward;
        print(heading);
        heading = Vector3.Scale(heading, new Vector3(1, 0, 1)).normalized;
        moveVec = (moveDirection.x * heading + Quaternion.Euler(0, 90, 0) * heading * moveDirection.z).normalized;
        moveVec = Time.fixedDeltaTime * moveVec * playerInput.moveSpeed;

        playerRigidbody.MovePosition(playerRigidbody.position + moveVec);
        moveVec1 = moveVec;
        moveVec1.y = 0;

        transform.LookAt(transform.position + moveVec1);
    }

    // 캐릭터 회피명령
    public void Dodge(Vector3 dir)
    {
        if ((pState == PlayerState.Idle || pState == PlayerState.Move || pState == PlayerState.BasicAttack || pState == PlayerState.Stun || pState == PlayerState.SpecialAttack) && Time.time >= nextDodgeableTime)
        {
            if(pState == PlayerState.BasicAttack)
            {
                playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
                playerAnimation.playerAnimator.SetBool("isAttack", false);
                comboCnt = 0;
                playerInput.isBasicAttacking = false;
            }
            else if(pState == PlayerState.SpecialAttack)
            {
                playerInput.isSpecialAttacking = false;
            }
/*            playerRigidbody.constraints = RigidbodyConstraints.FreezePositionY;*/
            playerInput.isDodge = true;
            nextDodgeableTime = Time.time + timeBetDodge;
            StartCoroutine(DodgeCoroutine(dir));
        }
    }

    // 캐릭터 실제 회피
    public IEnumerator DodgeCoroutine(Vector3 dir)
    {
        try {
            ActiveParticle(dashParticle, dashParticleTransform.position, dashParticleTransform.rotation, 1.5f);  //  회피 파티클 재생, 1.5초 후 제거
        } catch {
            Debug.LogError("Dash Particle Error");
        }
        playerRigidbody.useGravity = false;
        playerRigidbody.AddForce(dir.normalized * dashPower, ForceMode.Impulse);
        playerRigidbody.velocity = Vector3.zero;
        playerAnimation.DodgeAni();
        for (float t = 0f; t < 0.45f; t += Time.fixedDeltaTime)
        {
            Physics.queriesHitBackfaces = true;
            RaycastHit hit;
            RaycastHit[] hits;
            int layerMask = 1 << LayerMask.NameToLayer("Ground");
            hits = Physics.RaycastAll(transform.position, Vector3.up, 10, layerMask);
            if (hits.Length > 0)
            {
                hit = hits[hits.Length - 1];
                gameObject.transform.position = hit.point + Vector3.up * 0.1f;
            }
            else if (Physics.Raycast(transform.position, Vector3.down, out hit, 10, layerMask))
            {
                gameObject.transform.position = hit.point + Vector3.up * 0.1f;
            }
            yield return new WaitForFixedUpdate();
        }
        Physics.queriesHitBackfaces = false;

        playerRigidbody.useGravity = true;

        playerRigidbody.velocity = Vector3.zero; // 가속도 초기화
    }

    public void BasicAttack(Vector3 destination, float delay, float animSpeed)
    { 
        if (pState == PlayerState.Idle || pState == PlayerState.Move || pState == PlayerState.BasicAttack)
        {
            StartCoroutine(BasicAttackCoroutine(destination, delay, animSpeed));
        }
    }


    // 실제 공격
    public IEnumerator BasicAttackCoroutine(Vector3 destination, float delay, float animSpeed)
    {
        playerInput.isBasicAttacking = true;
        pState = PlayerState.BasicAttack;

        gameObject.transform.LookAt(destination);

        if (playerEquipmentManager.equipWeapon == null)
        {
            Debug.Log("무기없음");
            yield return new WaitForSeconds(0.0f);
        } else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().wType == Weapon.WeaponType.Rifle) {
            playerAnimation.Attack();
            
            for (int i = 0; i < 3; i++)
            {
                CreateBullet(); //총알 생성하기
                yield return new WaitForSeconds(0.1f);
            }
            
            yield return new WaitForSeconds(0.2f); // 딜레이

            playerInput.isBasicAttacking = false;
            pState = PlayerState.Idle;
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().wType == Weapon.WeaponType.Melee)
        {
            playerAnimation.playerAnimator.SetBool("isAttack", true);
            currentAttackTime = Time.time; // 재생한 시점
            if (Time.time - currentAttackTime < 2f) // 공격 애니메이션 재생 후 1초가 지나지 않았다면
            {
                comboCnt += 1;
                comboCnt = Mathf.Clamp(comboCnt, 0, 3); // 0~3으로 제한  
                playerAnimation.playerAnimator.SetInteger("ComboCnt", comboCnt);
/*                playerEquipmentManager.equipWeapon.OnAttack();*/
                if (comboCnt == 3)
                {
                    comboCnt = 0;
                    yield return new WaitForSeconds(delay);
                }
            }
            yield return new WaitForSeconds(delay);
            
            playerInput.isBasicAttacking = false;
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().wType == Weapon.WeaponType.Sword)
        {
            playerAnimation.playerAnimator.SetBool("isAttack", true);
            currentAttackTime = Time.time; // 재생한 시점 
            if (Time.time - currentAttackTime < 2f) // 공격 애니메이션 재생 후 1초가 지나지 않았다면
            {
                comboCnt += 1;
                comboCnt = Mathf.Clamp(comboCnt, 0, 3);  // 0~3으로 제한
                playerAnimation.playerAnimator.SetInteger("ComboCnt", comboCnt);
                if (comboCnt == 3)
                {
                    comboCnt = 0;
                    yield return new WaitForSeconds(1f);
                }
            }
            yield return new WaitForSeconds(delay * 2f);
            playerEquipmentManager.equipWeapon.OnAttack();
            playerInput.isBasicAttacking = false;
            
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().wType == Weapon.WeaponType.Spear)
        {
            playerAnimation.playerAnimator.SetBool("isAttack", true);
            currentAttackTime = Time.time; // 재생한 시점
            if (Time.time - currentAttackTime < 2f) // 공격 애니메이션 재생 후 1초가 지나지 않았다면
            {
                comboCnt += 1;
                comboCnt = Mathf.Clamp(comboCnt, 0, 3); // 0~3으로 제한
                playerAnimation.playerAnimator.SetInteger("ComboCnt", comboCnt);
                if (comboCnt == 2)
                {
                    playerRigidbody.AddForce(transform.forward * 18f, ForceMode.Impulse);
                    playerRigidbody.velocity = Vector3.zero;
                }
                if (comboCnt == 3)
                    comboCnt = 0;
            }
            yield return new WaitForSeconds(delay * 1.2f);
            playerEquipmentManager.equipWeapon.OnAttack();
            playerInput.isBasicAttacking = false;
        }
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

   /* public void Grenade(Vector3 destination)
    {
        if (pState == PlayerState.Idle || pState == PlayerState.Movement || pState == PlayerState.Attack)
        {
            StartCoroutine(GrenadeCoroutine(destination));
        }
    }

    public IEnumerator GrenadeCoroutine(Vector3 destination)
    {
           gameObject.transform.LookAt(destination);
    }*/

    public void SpecialAttack(Vector3 destination)
    {
        if (pState == PlayerState.Idle || pState == PlayerState.Move || pState == PlayerState.BasicAttack)
        {
            StartCoroutine(SpecialAttackCoroutine(destination));
        }
    }

    public IEnumerator SpecialAttackCoroutine(Vector3 destination)
    {
/*        PlayerInput.isRight = true;
        pState = PlayerState.RIghtAttack;*/
        gameObject.transform.LookAt(destination);

        if (playerEquipmentManager.equipWeapon == null)
        {
            Debug.Log("무기없음");
            yield return new WaitForSeconds(0.0f);
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().wType == Weapon.WeaponType.Rifle)
        {
            playerAnimation.RightAttack();
            /*            yield return new WaitForSeconds(1); // 딜레이
                        PlayerInput.isRight = false;
                        pState = PlayerState.Idle;*/
            playerEquipmentManager.equipWeapon.OnActive();
        }
/*        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isGun == true)
        {
            playerAnimation.RightAttack();
            PlayerInput.isRight = false;
            yield return new WaitForSeconds(1f);
            pState = PlayerState.Idle;

        }*/
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().wType == Weapon.WeaponType.Melee)
        {
            playerAnimation.RightAttack();
            playerRigidbody.AddForce(transform.forward * 12f, ForceMode.Impulse);
            playerRigidbody.velocity = Vector3.zero;
            /*            PlayerInput.isRight = false;
                        yield return new WaitForSeconds(1);
                        pState = PlayerState.Idle;*/
            playerEquipmentManager.equipWeapon.OnActive();
            
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().wType == Weapon.WeaponType.Sword)
        {
            playerAnimation.RightAttack();
            /*            PlayerInput.isRight = false;
                        yield return new WaitForSeconds(1f);
                        pState = PlayerState.Idle;*/
            playerEquipmentManager.equipWeapon.OnActive();
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().wType == Weapon.WeaponType.Spear)
        {
            playerAnimation.RightAttack();
            playerRigidbody.AddForce(transform.forward * 12f, ForceMode.Impulse);
            playerRigidbody.velocity = Vector3.zero;
            /*            PlayerInput.isRight = false;
                        yield return new WaitForSeconds(1f);
                        pState = PlayerState.Idle;*/
            playerEquipmentManager.equipWeapon.OnActive();
        }
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    //오브젝트풀에서 총알 가져와서 생성하기
    public void CreateBullet()
    {
        var Bulletobj = BulletObjectPool.GetBullet(playerEquipmentManager); // 오브젝트 풀에서 총알 가져오기
        Bulletobj.transform.position = FirePos.transform.position; //위치 지정
        Bulletobj.transform.rotation = FirePos.transform.rotation;// 회전 지정
    }

    public void ComboMove()
    {
        playerRigidbody.AddForce(transform.forward * 24f, ForceMode.Impulse);
        playerRigidbody.velocity = Vector3.zero;
    }

    public void NuckBackMove()
    {
        playerRigidbody.AddForce(-transform.forward * 7f, ForceMode.Impulse);
        playerRigidbody.velocity = Vector3.zero;
    }
}
