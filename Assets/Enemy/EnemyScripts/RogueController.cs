using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class RogueController : LivingEntity
{
    public enum RogueState { None, Idle, MoveTarget, Teleport, Attack, Die };

    [Header("기본속성")]
    public RogueState rstate = RogueState.None; // 근접적 상태변수
    public float MoveSpeed = 3.5f; //이동 속도
    public Vector3 targetPos; //공격 대상 위치
    public GameObject target; // 공격 대상
    public int Idlestate;

    private NavMeshAgent nav; // NavMesh 컴포넌트
    private Animator anim; // 애니메이터 컴포넌트
    private Rigidbody rigid;

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

    private float timeBetAttack = 0.5f; // 공격 딜레이
                                        // private float lastAttackTime = 0f; // 마지막 공격 시점
    private Vector3 dist; // 공격 대상과의 거리                              

    private float attackRange = 2f; // 공격 사거리


    [Header("텔레포트 속성")]
    private float curTpTime; //최근 텔레포트한 시간
    public float tpCooldown = 10f; //텔레포트 대기시간
    private bool bTeleportation = true; // 텔레포트 가능 여부

    private Vector3 tpPos; //텔레포트에 사용할 좌표


    [Header("공격범위 속성")]
    public float angleRange = 45f;
    private bool isCollision = false;
    Color blue = new Color(0f, 0f, 1f, 0.2f);
    Color red = new Color(1f, 0f, 0f, 0.2f);
    float dotValue = 0f;
    Vector3 direction;

    private void Start()
    {
        //대기 상태로 설정
        rstate = RogueState.Idle;

        // 컴포넌트 불러오기
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();

        nav.speed = MoveSpeed;
    }

    // 근접 적 상태 체크
    void CheckState()
    {
        switch (rstate)
        {
            case RogueState.Idle:
                IdleUpdate();
                break;
            case RogueState.MoveTarget:
                MoveUpdate();
                break;
            case RogueState.Attack:
                AttackUpdate();
                break;
            case RogueState.Teleport:
                TeleportUpdate();
                break;
            case RogueState.Die:
                Die();
                break;
        }
    }


    //애니메이션 상태 체크
    void AnimationCheck()
    {
        switch (rstate)
        {
            case RogueState.Idle:
                Idlestate = Random.Range(0, 3);
                move = false;
                attack = false;
                anim.SetInteger("IdleCheck", Idlestate);
                break;
            case RogueState.MoveTarget:
                move = true;
                attack = false;
                break;
            case RogueState.Attack:
                move = false;
                attack = true;
                break;
            
            case RogueState.Die:
                break;
        }
    }

    // 대기 상태일때의 동작
    void IdleUpdate()
    {
        if (hasTarget) //타겟이 존재할때
        {
            rstate = RogueState.MoveTarget;
        }
        else
        {
            nav.isStopped = true; // 동작 정지
            nav.velocity = Vector3.zero; 
        }

    }

   
    void MoveUpdate()
    {
        Vector3 lookAtPosition = Vector3.zero;

        if (hasTarget)
        {
            targetPos = target.transform.position;

            if (bTeleportation)//텔레포트가 가능하면
            {
                rstate = RogueState.Teleport;
                return;
            }
            else if (isCollision && !bTeleportation)// 공격범위에 충돌하고 텔레포트가 불가능할시
            {
                rstate = RogueState.Attack; //공격 상태로 변환
            }

            lookAtPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z); //이동시 바라볼 방향 체크

        }


        // 추적 실행
        nav.isStopped = false;
        nav.SetDestination(lookAtPosition); // 목적지 설정


    }

    //텔레포트시
    void TeleportUpdate()
    {
        if(bTeleportation)
        {
            //추적대상 근처 랜덤 위치 계산
            //Random.insideUnityCircle은 x, y값만 계산해서 y값을 z값에 더함, y값은 그냥 y값으로 함
            tpPos = Random.insideUnitCircle * 4f;
            tpPos.x += target.gameObject.transform.position.x;
            tpPos.z = tpPos.y + target.gameObject.transform.position.z;
            tpPos.y = target.gameObject.transform.position.y;

            this.transform.position = tpPos;

            //시간 갱신
            curTpTime = Time.time;
            //순간이동 가능 여부 false로 변경
            bTeleportation = false;

            if (isCollision) //타겟과 충돌시
            {
                rstate = RogueState.Attack; //공격으로 상태 전환
            }
            else
            {
                rstate = RogueState.MoveTarget; //추적으로 상태 전환
            }
        }
    }

    void CheckTelTime() //텔레포트 쿨타임 체크
    {
        if (!bTeleportation)
        {
            if (curTpTime + tpCooldown <= Time.time) //텔포 쿨타임 지나면
            { bTeleportation = true; }
        }
    }

    // 공격시
    void AttackUpdate()
    {
       if (!isCollision) //공격범위보다 멀면
        {
            rstate = RogueState.MoveTarget;
        }
        else
        {
            nav.isStopped = true; // 네비 멈추기        
            nav.velocity = Vector3.zero; // 이동속도 줄이기
            transform.LookAt(target.transform);
        }
    }

    //공격 적용
    public void OnAttackEvent()
    {
        LivingEntity attackTarget = target.GetComponent<LivingEntity>();


        Vector3 hitPoint = target.GetComponent<Collider>().ClosestPoint(transform.position);

        Vector3 hitNormal = transform.position - target.transform.position;

       // attackTarget.OnDamage(damage, hitPoint, hitNormal);
    }

  

    // 공격을 당했을때
    public override void OnDamage(Damage dInfo)
    {
        anim.SetTrigger("isHit"); // 트리거 실행

        health -= damage; // 체력 감소

        if (health <= 0 && !dead) // 체력이 0보다 작고 사망상태가 아닐때
        {
            rstate = RogueState.Die; // 죽음 상태로 변환
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
        rstate = RogueState.MoveTarget;
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
            sectorCheck();
        }

        CheckState(); //상태 체크
        AnimationCheck();
        CheckTelTime();// 텔레포트 가능 여부 체크

        anim.SetBool("Attack", attack);
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
