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
    public LayerMask targetLayer; // 공격 대상 레이어
    public LivingEntity target; // 공격 대상
    public float fRange = 10f; // 수색범위

    private UnityEngine.AI.NavMeshAgent nav; // NavMesh 컴포넌트
    private Animator anim; // 애니메이터 컴포넌트

    private bool move = false; //움직임 관련 bool값
    private bool attack = false; // 공격 관련 bool값

    private bool hasTarget
    {
        get
        {
            if (target != null && !target.dead)
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
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

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

    // 대기 상태일때의 동작
    void IdleUpdate()
    {
        if (hasTarget) //타겟이 존재할때
        {
            rstate = RogueState.MoveTarget;
            return;
        }
        else
        {

            nav.isStopped = true; // 동작 정지
            attack = false; // 공격 false
            move = false; // 이동 false

        
            FindNearEnemy(targetLayer); //가까운 상대 찾기 
        }

    }

    //가까운 거리의 적찾기
    void FindNearEnemy(LayerMask tlayer)
    {

        Collider[] colliders = Physics.OverlapSphere(this.transform.position, fRange, tlayer);//콜라이더 설정하기
        Collider colliderMin = null; // 가장가까운 대상의 콜라이더
        float fPreDist = 99999999.0f; // 가장가까운 대상 거리 float값

        //찾은대상중 가장 가까운 대상을 찾는다.
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            float fDist = Vector3.Distance(collider.transform.position, this.transform.position);
            //콜라이더를 통해 찾은 타겟과의 거리를 float값으로 계산

            if (colliderMin == null || fPreDist > fDist) // 조건문으로 가장 가까운 대상 찾기
                colliderMin = collider;
            fPreDist = fDist;

        }
       
        if(colliderMin != null) //콜라이더가 비어있지 않으면
        { 
            LivingEntity livingEntity = colliderMin.GetComponent<LivingEntity>();


            if (livingEntity != null && !livingEntity.dead) //찾은 리빙엔티티가 죽지않고 null값이 아닐때
            {
                target = livingEntity;
                rstate = RogueState.MoveTarget;
            }
        }
    }


    void MoveUpdate()
    {
        move = true; //이동 활성화
        attack = false; // 공격 비활성화
        
        if(bTeleportation)//텔레포트가 가능하면
        {
            rstate = RogueState.Teleport;
            return;
        }


        Vector3 temp = target.transform.position;
        temp.y = this.transform.position.y;
        this.transform.LookAt(temp); // 공격대상 바라보기

        // 추적 실행
        nav.isStopped = false;
        nav.SetDestination(target.transform.position); // 목적지 설정


        // 타겟이 공격 사거리에 들어온다면

        CheckDistance();
      
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
            CheckDistance();
        }
        else
        {
            if (curTpTime + tpCooldown <= Time.deltaTime) //텔포 쿨타임 지나면
            { bTeleportation = true; }

            else
            { CheckDistance();  } //거리 확인시킴
        }
    }

    // 타겟과의 거리 체크
    void CheckDistance()
    {
        // 타겟이 공격 사거리에 들어온다면
        if (Vector3.Distance(this.transform.position, target.transform.position) <= attackRange)
        {
            rstate = RogueState.Attack; // 공격상태로 변환
        }
        else
        {
            rstate = RogueState.MoveTarget; // 이동상태로 변환
        }
      
    }

    // 공격시
    void AttackUpdate()
    {
        //dist = target.transform.position - this.transform.position;

        if(Vector3.Distance(this.transform.position, target.transform.position) > attackRange + 0.5f)
        {
            rstate = RogueState.MoveTarget;
            return;
        }
        else
        {
            nav.isStopped = true; // 네비 멈추기
            attack = true;  // 공격 활성화
            move = false; // 이동 비활성화
           
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

    //공격 딜레이
    IEnumerator WaitUpdate()
    {
        anim.SetBool("Attack", attack);
        yield return new WaitForSeconds(2f);
        Debug.Log("2");
       
    }

    // 공격을 당했을때
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        anim.SetTrigger("isHit"); // 트리거 실행

        health -= damage; // 체력 감소

        if (health <= 0 && !dead) // 체력이 0보다 작고 사망상태가 아닐때
        {
            rstate = RogueState.Die; // 죽음 상태로 변환
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
        CheckState();
        anim.SetBool("Attack", attack);
        anim.SetBool("isRun", move);

        if(hasTarget)
        {
            sectorCheck();
            transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position); //타겟 바라보기    
        }
         
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, fRange);

        Handles.color = isCollision ? red : blue;
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angleRange / 2, attackRange);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angleRange / 2, attackRange);
    
    }
}
