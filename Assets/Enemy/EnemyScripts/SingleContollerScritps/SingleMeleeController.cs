using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;
using UnityEngine.AI;

public class SingleMeleeController : LivingEntity
{
    public enum MeleeState { None, Idle, MoveTarget, JumpAttack, NuckBack, Stun, Attack, Die };

    [Header("전투 속성")]
    private Damage damage;


    [Header("기본속성")]
    public MeleeState mstate = MeleeState.None; // 근접적 상태변수
    public Vector3 targetPos; //공격 대상 위치
    public GameObject target; // 공격 대상
    public int Idlestate; // 아이들 상태


    private NavMeshAgent nav; // NavMesh 컴포넌트
    private Animator anim; // 애니메이터 컴포넌트
    private Rigidbody rigid; //리지드 바디 컴포넌트
    private PhotonView pv; //포톤뷰 컴포넌트
    [SerializeField]
    private Healthbar healthbar;


    [SerializeField]
    private float attackRange = 2f; // 공격 사거리
    Vector3 lookAtPosition; //회전방향

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
        rigid = GetComponent<Rigidbody>();
        nav.updateRotation = false; // 네비의회전 기능 비활성화
    }

    protected override void OnEnable()
    {
        //대기 상태로 설정
        isFirstAttack = true;
        mstate = MeleeState.Idle;
        this.startingHealth = 50f; //스포너에 들어갈시 삭제
        healthbar.SetMaxHealth((int)startingHealth);
        base.OnEnable();
    }

    public void Init(float _damage, float _speed, float _startHealth = 50f) //초기 설정 메소드
    {
        nav.speed = _speed; //이동속도 설정
        damage.dValue = _damage; //초기 데미지값 설정
        damage.dType = Damage.DamageType.Melee; //데미지 종류 설정
        this.startingHealth = _startHealth; //초기 HP값 설정
    }


    // 근접 적 상태 체크
    void CheckState()
    {
        switch (mstate)
        {
            case MeleeState.Idle:
                IdleUpdate();
                break;
            case MeleeState.MoveTarget:
                MoveUpdate();
                break;
            case MeleeState.NuckBack:
                break;
            case MeleeState.Attack:
                StartCoroutine(AttackUpdate());
                break;
            case MeleeState.JumpAttack:
                JumpAttackRoutine();
                break;
            case MeleeState.Die:
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

            case MeleeState.JumpAttack:
                move = false;
                attack = false;
                break;

            case MeleeState.Die:
                break;
        }
    }


    void ShowAnimation(int a)
    {
        switch (a)
        {
            case 1:
                anim.SetTrigger("JumpAttack");
                break;
            case 2:
                anim.SetTrigger("isKnockBack");
                break;
            case 3:
                anim.SetTrigger("wakeUp");
                break;
            case 4:
                anim.SetTrigger("isStun");
                break;
            case 5:
                anim.SetTrigger("isHit");
                break;
            case 6:
                anim.SetTrigger("Lying");
                break;
            case 7:
                anim.SetTrigger("isDead");
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

        if (hasTarget) //타겟이 있다면
        {
            targetPos = target.transform.position; //타겟의 위치 지정

            sectorCheck();//공격범위 체크

            if (isFirstAttack)
            {
                // 타겟이 공격 사거리에 들어오고 첫번쨰 공격이라면
                if (Vector3.Distance(this.transform.position, target.transform.position) < 4f)
                {
                    mstate = MeleeState.JumpAttack;
                    if (isFirstAttack)
                    {
                        ShowAnimation(1);
                        isFirstAttack = false;
                    }
                }
                else
                {
                    lookAtPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z); //이동시 바라볼 방향 체크
                    nav.isStopped = false; // 추적 실행
                    transform.LookAt(lookAtPosition);
                    nav.SetDestination(lookAtPosition); // 목적지 설정
                }
            }
            else
            {
                if (isCollision)
                {
                    nav.velocity = Vector3.zero;
                    nav.isStopped = true;
                    mstate = MeleeState.Attack; // 공격상태로 변환
                }
                else
                {
                    lookAtPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z); //이동시 바라볼 방향 체크
                    nav.isStopped = false; // 추적 실행
                    transform.LookAt(lookAtPosition);
                    nav.SetDestination(lookAtPosition); // 목적지 설정
                }
            }



        }
    }


    void JumpAttackRoutine()
    {
        StartCoroutine(JumpAttack());
    }


    IEnumerator JumpAttack()
    {

        lookAtPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);

        nav.SetDestination(lookAtPosition); //목적지 설정
        damage.dValue *= 2; // 점프공격시 데미지 2배 적용

        yield return new WaitForSeconds(0.89f);

        nav.velocity = Vector3.zero;
        nav.isStopped = true;

        damage.dValue /= 2; //데미지 원상복귀

        yield return new WaitForSeconds(0.6f);

        sectorCheck();//공격범위 체크
        if (isCollision)
        {
            mstate = MeleeState.Attack;
        }
        else
        {
            mstate = MeleeState.MoveTarget;
        }


    }

    // 공격시
    IEnumerator AttackUpdate()
    {

        nav.isStopped = true; // 네비 멈추기
        nav.velocity = Vector3.zero;
        transform.LookAt(target.transform);

        yield return new WaitForSeconds(2.3f);

        sectorCheck();//공격범위 체크

        if (!isCollision) //공격범위보다 멀면
        {
            mstate = MeleeState.MoveTarget; //추적상태로 변환
        }


    }

    //공격 적용
    public void OnAttackEvent()
    {
       
        LivingEntity enemytarget = target.GetComponent<LivingEntity>(); //타겟의 리빙엔티티 가져오기

        damage.hitPoint = target.GetComponent<Collider>().ClosestPoint(transform.position);

        damage.hitNormal = transform.position - target.transform.position;

        sectorCheck();

        if (isCollision) //공격범위 안이라면
        {
            enemytarget.OnDamage(damage); //데미지 이벤트 실행

        }
    }


    void OnSetTarget(GameObject _target)//타겟 지정
    {
        if (hasTarget) //이미 타겟이 있거나 마스터 클라이언트가 아니라면
        {
            return;
        }
        target = _target;
        //타겟을 향해 이동하는 상태로 전환
        mstate = MeleeState.MoveTarget;
    }

    // 공격을 당했을때
    public override void OnDamage(Damage dInfo)
    {
        if (dead) return;
        
        health -= dInfo.dValue; //체력 감소      
        if (health <= 0 && !dead && this.gameObject.activeInHierarchy) // 체력이 0보다 작고 사망상태가 아닐때
        {
            Die();
        }
        else
        {
            StopAllCoroutines();
            DamageEvent((int)dInfo.dType, dInfo.ccTime);
        }
    }

    void DamageEvent(int dType, float ccTime)
    {
        switch (dType)
        {
            case 1:
                if (mstate == MeleeState.JumpAttack)
                { mstate = MeleeState.NuckBack; StartCoroutine(NuckBackDamageRoutine(ccTime)); }
                else
                { StartCoroutine(NormalDamageRoutine()); }//일반 공격일시
                break;
            case 2:
                mstate = MeleeState.Stun;
                StartCoroutine(StunRoutine(ccTime));
                break;

            case 3:
                mstate = MeleeState.NuckBack;
                StartCoroutine(NuckBackDamageRoutine(ccTime));
                break;

        }

    }

    IEnumerator NormalDamageRoutine() //일반 피격 이벤트
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.KnockBack") )
        { ShowAnimation(5); }// 트리거 실행


        float startTime = Time.time; //시간체크

        nav.velocity = Vector3.zero;

        while (Time.time < startTime + 0.8f)
        {
            nav.velocity = Vector3.zero;
            yield return null;
        }

        sectorCheck();

        if (isCollision)
        {
            mstate = MeleeState.Attack;
        }
        else
        {
            mstate = MeleeState.MoveTarget;
        }

    }


    IEnumerator NuckBackDamageRoutine(float nuckTime) //넉백시
    {
        nav.velocity = Vector3.zero;

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.KnockBack"))
        { ShowAnimation(2); }// 트리거 실행

        float startTime = Time.time;
        nav.isStopped = true;

        while (Time.time < startTime + nuckTime)
        {
            
            rigid.angularVelocity = Vector3.zero;
            yield return null;
        }

        startTime = Time.time;
        ShowAnimation(3);


        while (Time.time < startTime + 2.8f)
        {
            rigid.angularVelocity = Vector3.zero;
            // nav.isStopped = true;
            yield return null;
        }

        sectorCheck();

        if (isCollision)
        {
            mstate = MeleeState.Attack;
        }
        else
        {
            mstate = MeleeState.MoveTarget;
        }
    }

    IEnumerator StunRoutine(float nuckTime) //스턴
    {
        nav.velocity = Vector3.zero;
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.KnockBack"))
        { ShowAnimation(4); } // 트리거 실행

        float startTime = Time.time;

        while (Time.time < startTime + nuckTime)
        {
            nav.isStopped = true;
            rigid.angularVelocity = Vector3.zero;
            yield return null;
        }

        ShowAnimation(3);


        yield return new WaitForSeconds(0.2f);


        sectorCheck();

        if (isCollision)
        {
            mstate = MeleeState.Attack;
        }
        else
        {
            mstate = MeleeState.MoveTarget;
        }
    }

    public override void Die()
    {

        base.Die();
        StopAllCoroutines();
        StartCoroutine(Death());
    }

    //죽었을때
    public IEnumerator Death()
    {

        rigid.isKinematic = true;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.KnockBack"))
        {
            ShowAnimation(6);
        }
        else
        {
            ShowAnimation(7);
        }


        mstate = MeleeState.Die; //상태를 죽음으로 변경

        nav.isStopped = true; //네비 멈추기
        nav.enabled = false; // 네비 비활성화
        dead = true; //죽음 확성화

        Collider[] enemyColliders = GetComponents<Collider>(); //모든 콜라이더 가져오기

        // 콜라이더 다끄기
        for (int i = 0; i < enemyColliders.Length; i++)
        {
            enemyColliders[i].enabled = false;
        }



        yield return new WaitForSeconds(2f); // 1초 대기

        //ObjectPool.ReturnMeleeEnemy(this); //다시 오브젝트 풀에 반납

    }

    private void Update()
    {

        healthbar.SetHealth((int)health);

        if (hasTarget) //타겟이 있다면
        {
            targetPos = target.transform.position;
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




    //private void OnDrawGizmos() // 범위 그리기
    //{
    //    Handles.color = isCollision ? red : blue;
    //    Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angleRange / 2, attackRange);
    //    Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angleRange / 2, attackRange);

    //}
}
