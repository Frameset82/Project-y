using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 조작에 의해 캐릭터 상태를 변화시킴
public class PlayerController : MonoBehaviour
{
    // 이동속도와 회전속도 회피속도
    public float moveSpeed = 5f;
    public float rotateSpeed = 0.15f;
    private float dodgePower = 600f;
    
    // 회피명령 딜레이 변수
    private float timeBetDodge = 1f;
    private float nextDodgeableTime = 0f;

    private Vector3 moveDestination;

    private PlayerInput playerInput; // 플레이어 입력 컴포넌트
    private Rigidbody playerRigidbody; // 캐릭터 리지드바디
    public PlayerAnimation playerAnimation;
    public GameObject playerAvatar;

    public PlayerState pState;
    public enum PlayerState // 플레이어 상태 리스트
    {
        Idle, // 가만히 서있는 상태
        Movement, // 이동중인 상태
        Dodge, // 회피중인 상태
        Attack // 공격중인 상태
    }
    
    // 사용할 컴포넌트 할당(애니메이터는 수동할당)
    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    // 캐릭터 이동감지
    private void FixedUpdate() {
        Movement();
    }

    // 캐릭터 이동명령
    public void Move(Vector3 destination)
    {
        moveDestination = destination;
        if(pState == PlayerState.Idle){
            pState = PlayerState.Movement;
        }
    }
    // 캐릭터 실제 이동
    public void Movement(){
        if (pState == PlayerState.Movement)
        {
            if (Vector3.Distance(
                new Vector3(playerRigidbody.transform.position.x, 0, playerRigidbody.transform.position.z),
                new Vector3(moveDestination.x, 0, moveDestination.z)
                ) < .25f)
            {
                playerAnimation.MoveAni(false);
                pState = PlayerState.Idle;
                return;
            }
            Vector3 direction = moveDestination - transform.position;
            Vector3 targetDirection = moveDestination - transform.position;
            targetDirection.y = 0;

            playerRigidbody.transform.Translate(
                new Vector3(direction.normalized.x, 0, direction.normalized.z)
                * moveSpeed
                * Time.deltaTime);
            playerAvatar.transform.rotation = Quaternion.Slerp(
                playerAvatar.transform.rotation, Quaternion.LookRotation(targetDirection), rotateSpeed);
            playerAnimation.MoveAni(true);
        }  
    }

    
    // 캐릭터 회피명령
    public void Dodge(Vector3 destination)
    {
        if((pState == PlayerState.Idle || pState == PlayerState.Movement) && Time.time >= nextDodgeableTime){
            nextDodgeableTime = Time.time+timeBetDodge;
            StartCoroutine(DodgeCoroutine(destination));
        }
    }
    // 회피중 이동명령 예약
    private bool willMove = false;
    public void WillMove(){
        willMove = true;
    }
    // 캐릭터 실제 회피
    public IEnumerator DodgeCoroutine(Vector3 destination){
        pState = PlayerState.Dodge;
        willMove = false;
        Vector3 power = (destination - transform.position).normalized*dodgePower;
        playerAvatar.transform.rotation = Quaternion.LookRotation(destination-transform.position);
        playerRigidbody.AddForce(power, ForceMode.Force);
        playerAnimation.DodgeAni();
        playerAnimation.MoveAni(false);
        yield return new WaitForSeconds(.5f); // 회피 지속시간
        playerRigidbody.velocity = Vector3.zero; // 가속도 초기화
        // 회피중 이동명령을 받았는지 체크
        if(willMove){
            pState = PlayerState.Movement;
        } else {
            pState = PlayerState.Idle;
        }
    }

    // 캐릭터 공격 명령
    public void Attack(Vector3 destination)
    {
        StartCoroutine(AttackCoroutine(destination));
    }
    // 실제 공격
    public IEnumerator AttackCoroutine(Vector3 destination)
    {
        pState = PlayerState.Attack;


        playerAnimation.Attack();
        yield return new WaitForSeconds(0.8f); // 딜레이
        pState = PlayerState.Idle;

    }

}