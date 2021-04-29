using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class MeleeController : LivingEntity
{
    public enum MeleeState {None, Idle, MoveTarget, Attack, Die};

    [Header("기본속성")]
    public MeleeState mstate = MeleeState.None; // 근접적 상태변수
    public float MoveSpeed = 2.5f; //이동 속도
    public Vector3 targetPos; //공격 대상 위치
    public GameObject target; // 공격 대상
    public int Idlestate;

    private NavMeshAgent nav; // NavMesh 컴포넌트
    private Animator anim; // 애니메이터 컴포넌트


    [Header("전투 속성")]
    public float damage = 20f; // 공격력
    [SerializeField]
    private float attackRange = 2f; // 공격 사거리


    [Header("공격범위 속성")]
    public float angleRange = 45f;
    private bool isCollision = false;
    Color blue = new Color(0f, 0f, 1f, 0.2f);
    Color red = new Color(1f, 0f, 0f, 0.2f);
    float dotValue = 0f;
    Vector3 direction;

    private bool move = false; //움직임 관련 bool값
    private bool attack = false; // 공격 관련 bool값
    private bool isFirstAttack = true; //점프 공격 관련 bool값

    private bool hasTarget
    {
        get
        {
            if (target != null)
            {
                return true;
            }
            return false;
        }
    }

 

    private void Awake()
    {
        // 컴포넌트 불러오기
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

    }

    protected override void OnEnable()
    {
        //대기 상태로 설정
        mstate = MeleeState.Idle;
        // 이동 속도 설정
        nav.speed = MoveSpeed;
    }

    // 근접 적 상태 체크
    void CheckState()
    {
        switch(mstate)
        {
            case MeleeState.Idle:
                IdleUpdate();
                break;
            case MeleeState.MoveTarget:
                MoveUpdate();
                break;
            case MeleeState.Attack:
                AttackUpdate();
                break;
            case MeleeState.Die:
                Die();
                break;
        }
    }

    //애니메이션 상태 체크
    void AnimationCheck()
    {
        switch (mstate)
        {
            case MeleeState.Idle:
                Idlestate = Random.Range(0, 3);
                move = false;
                attack = false;
                anim.SetInteger("IdleCheck", Idlestate);
                break;

            case MeleeState.MoveTarget:
                move = true;
                attack = false;
                break;

            case MeleeState.Attack:             
                move = false;
                attack = true;              
                break;

            case MeleeState.Die:              
                break;
        }
    }

    // 대기 상태일때의 동작
    void IdleUpdate()
    {
        if (hasTarget) //타겟이 있으면
        {
            mstate = MeleeState.MoveTarget; // 추적 상태로 변환     
        }
        else// 타겟이 없으면
        {
           
            nav.isStopped = true; // 동작 정지
            nav.velocity = Vector3.zero; // 속도 0으로 지정

        }
       
    }

  

    void MoveUpdate() //추적시에
    {
        Vector3 lookAtPosition = Vector3.zero;

        if (hasTarget)
        {
            targetPos = target.transform.position;


            // 타겟이 공격 사거리에 들어온다면
            if (Vector3.Distance(this.transform.position, target.transform.position) < 4f && isFirstAttack)
            {            
                JumpAttack();
                return;              
            }
            else if(isCollision && !isFirstAttack)
            {
                nav.velocity = Vector3.zero;
                nav.isStopped = true;
                mstate = MeleeState.Attack; // 공격상태로 변환
            }

            lookAtPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z); //이동시 바라볼 방향 체크
       
        }

       
        nav.isStopped = false; // 추적 실행
        nav.SetDestination(lookAtPosition); // 목적지 설정
      
    }


    void JumpAttack()
    {
      
        anim.SetBool("isFirstAttack", isFirstAttack);
        if (Vector3.Distance(this.transform.position, target.transform.position) <= 2f)
        {
            
            isFirstAttack = false;
            anim.SetBool("isFirstAttack", isFirstAttack);
            mstate = MeleeState.Attack; // 공격상태로 변환
        }
    }
    
    // 공격시
    void AttackUpdate()
    {
        if(!isCollision) //공격범위보다 멀면
        {
            mstate = MeleeState.MoveTarget; //추적상태로 변환
        }
        else
        {
            nav.isStopped = true; // 네비 멈추기
        }      
    }

    //공격 적용
    public void OnDamageEvent()
    {

        DummyPlayerController dm = target.GetComponent<DummyPlayerController>();

        Vector3 hitPoint = target.GetComponent<Collider>().ClosestPoint(transform.position);

        Vector3 hitNormal = transform.position - target.transform.position;
      
        dm.OnDamage(damage, hitPoint, hitNormal);
    }


    void OnSetTarget(GameObject _target)
    {
        if (hasTarget) //이미 타겟이 있다면
        {
            return;
        }

        
        target = _target;
        targetPos = target.transform.position;
        //타겟을 향해 이동하는 상태로 전환
        mstate = MeleeState.MoveTarget;
    }

    // 공격을 당했을때
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        anim.SetTrigger("isHit"); // 트리거 실행
        
        health -= damage; // 체력 감소

        if (health <= 0 && !dead) // 체력이 0보다 작고 사망상태가 아닐때
        {
            mstate = MeleeState.Die; // 죽음 상태로 변환
        }
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
        if(hasTarget) //타겟이 있다면
        {
            sectorCheck();//공격범위 체크
        }

        CheckState(); //상태 체크
        AnimationCheck(); //애니메이션 상태 체크

        anim.SetBool("isRun", move);
        anim.SetBool("isAttack", attack);    
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
