using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Animations.Rigging;
using UnityEngine.AI;

public class RifleController : LivingEntity
{
    public enum RifleState { None, Idle, MoveTarget,  Attack, Die };

    [Header("기본속성")]
    public RifleState rstate = RifleState.None; // 근접적 상태변수
    public float MoveSpeed = 3.5f; //이동 속도
    public Vector3 targetPos; //공격 대상 위치
    public GameObject target; // 공격 대상
    public int Idlestate; 

    private NavMeshAgent nav; // NavMesh 컴포넌트
    private Animator anim; // 애니메이터 컴포넌트
    public Rig aimLayer; //리그 레이어 
    public float aimDuration = 0.3f; // 공격 상태로 전환 시간 

    private bool move = false; //움직임 관련 bool값
    private bool attack = false; // 공격 관련 bool값
  

    private bool hasTarget
    {
        get
        {
            if (target != null )
            {
                return true;
            }
            return false;
        }
    }

    [Header("전투 속성")]
    public float damage = 20f; // 공격력
    public float attackRange = 7f; // 공격 사거리



    [Header("공격범위 속성")]
    public float angleRange = 45f;
    private bool isCollision = false;
    Color blue = new Color(0f, 0f, 1f, 0.2f);
    Color red = new Color(1f, 0f, 0f, 0.2f);
    float dotValue = 0f;
    Vector3 direction;

    private void Awake()
    {
        // 컴포넌트 불러오기
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();  
    }

    protected override void OnEnable()
    {
        //대기 상태로 설정
        rstate = RifleState.Idle;
        nav.speed = MoveSpeed;
    }

    // 근접 적 상태 체크
    void CheckState()
    {
        switch (rstate)
        {
            case RifleState.Idle:
                IdleUpdate();
                break;
            case RifleState.MoveTarget:
                MoveUpdate();
                break;
            case RifleState.Attack:
                AttackUpdate();
                break;
            case RifleState.Die:
                Die();
                break;
        }
    }


    void AnimationCheck()
    {
        switch (rstate)
        {
            case RifleState.Idle:
                Idlestate = Random.Range(0, 3);
                move = false;
                attack = false;
                anim.SetInteger("Idlestate", Idlestate);
                aimLayer.weight -= Time.deltaTime / aimDuration;
                break;

            case RifleState.MoveTarget:
                move = true;
                attack = false;
                aimLayer.weight -= Time.deltaTime / aimDuration;
                break;
            case RifleState.Attack:
                move = false;
                attack = true;
                aimLayer.weight += Time.deltaTime / aimDuration;
                break;
            case RifleState.Die:
     
                break;
        }
    }

    // 대기 상태일때의 동작
    void IdleUpdate()
    {
        if (hasTarget) //타겟이 존재할때
        {
            rstate = RifleState.MoveTarget;       
        }
        else
        {
            nav.isStopped = true; // 동작 정지
            nav.velocity = Vector3.zero; // 속도 0으로 지정
        }

    }

   

    void MoveUpdate()
    {
        Vector3 lookAtPosition = Vector3.zero;

        if(hasTarget)
        {
            targetPos = target.transform.position;

            if(isCollision)
            {
                rstate = RifleState.Attack;
            }

            lookAtPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);
        }

        // 추적 실행
        nav.isStopped = false;
        nav.SetDestination(lookAtPosition); // 목적지 설정

    }


    // 공격시
    void AttackUpdate()
    {
       

        if (!isCollision)
        {
            rstate = RifleState.MoveTarget;        
        }
        else
        {
            nav.isStopped = true; // 네비 멈추기
            nav.velocity = Vector3.zero;
            transform.LookAt(target.transform);

        }
    }

    //공격 적용
    public void OnDamageEvent()
    {
        LivingEntity attackTarget = target.GetComponent<LivingEntity>();


        Vector3 hitPoint = target.GetComponent<Collider>().ClosestPoint(transform.position);

        Vector3 hitNormal = transform.position - target.transform.position;

        attackTarget.OnDamage(damage, hitPoint, hitNormal);
    }

    // 공격을 당했을때
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        anim.SetTrigger("isHit"); // 트리거 실행

        health -= damage; // 체력 감소

        if (health <= 0 && !dead) // 체력이 0보다 작고 사망상태가 아닐때
        {
            rstate = RifleState.Die; // 죽음 상태로 변환
        }
    }

    void OnSetTarget(GameObject _target) //타겟설정
    {
        if (hasTarget) //이미 타겟이 있다면
        {
            return;
        }
        target = _target;
        //타겟을 향해 이동하는 상태로 전환
        rstate = RifleState.MoveTarget;
    }


    //죽었을때
    public override void Die()
    {

        base.Die();

        Collider[] enemyColliders = GetComponents<Collider>();

        // 콜라이더 다끄기
        for (int i = 0; i < enemyColliders.Length; i++)
        {
            enemyColliders[i].enabled = false;
        }

        nav.isStopped = true; //네비 멈추기
        nav.enabled = false; // 네비 비활성화
        anim.SetTrigger("isDead"); // 트리거 활성화
    }

    private void Update()
    {
        if (hasTarget) //타겟이 있다면
        {
            sectorCheck();//공격범위 체크
        }


        CheckState();
        AnimationCheck(); //애니메이션 상태 체크

        anim.SetBool("isAttack", attack);
        anim.SetBool("isRun", move);


    }




    void sectorCheck() // 부챗꼴 범위 충돌
    {
        dotValue = Mathf.Cos(Mathf.Deg2Rad * (angleRange / 2));
        direction = target.transform.position - transform.position;
        if (direction.magnitude < attackRange)
        {
            if (Vector3.Dot(direction.normalized, transform.forward) > dotValue)
            {
                isCollision = true;
            }
            else
                isCollision = false;
        }
        else
            isCollision = false;
    }


    private void OnDrawGizmos() // 범위 그리기
    {

        Handles.color = isCollision ? red : blue;
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angleRange / 2, attackRange);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angleRange / 2, attackRange);

    }
}
