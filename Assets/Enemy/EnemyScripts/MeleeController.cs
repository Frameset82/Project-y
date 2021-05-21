using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using UnityEditor;

public class MeleeController : LivingEntity
{
    public enum MeleeState {None, Idle, MoveTarget, JumpAttack, NuckBack, Stun, Attack, Die};
   
    [Header("전투 속성")]
    private Damage damage;


    [Header("기본속성")]
    public MeleeState mstate = MeleeState.None; // 근접적 상태변수
    public Vector3 targetPos; //공격 대상 위치
    public GameObject target; // 공격 대상
    public int Idlestate;


    private NavMeshAgent nav; // NavMesh 컴포넌트
    private Animator anim; // 애니메이터 컴포넌트
    private Rigidbody rigid;
    [SerializeField]
    private Healthbar healthbar;

 

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
        rigid = GetComponent<Rigidbody>();
        nav.updateRotation = false; // 네비의회전 기능 비활성화
    }

    protected override void OnEnable()
    {
        //대기 상태로 설정
        mstate = MeleeState.Idle;
        this.startingHealth = 50f;
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
        switch(mstate)
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
                AttackUpdate();
                break;
            case MeleeState.JumpAttack:
                JumpAttackRoutine();
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

            case MeleeState.JumpAttack:
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

        if (hasTarget) //타겟이 있다면
        {
            targetPos = target.transform.position; //타겟의 위치 지정


            // 타겟이 공격 사거리에 들어오고 첫번쨰 공격이라면
            if (Vector3.Distance(this.transform.position, target.transform.position) < 4f && isFirstAttack)
            {
                mstate = MeleeState.JumpAttack;
            }
            else if(isCollision && !isFirstAttack) 
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

    void JumpAttackRoutine()
    {
        

       StartCoroutine(JumpAttack(targetPos));
      
       
    }

    /*
    void JumpAttack(Vector3 tPos) //점프 공격
    {

        nav.SetDestination(tPos);

        anim.SetBool("isFirstAttack", true); //공격실행
        damage *= 2; // 점프공격시 데미지 2배 적용

    

        if (Vector3.Distance(this.transform.position, target.transform.position) <= 2f)
        {
            isFirstAttack = false;
            anim.SetBool("isFirstAttack", false);// 공격 종료
            mstate = MeleeState.Attack; // 공격상태로 변환
            damage /= 2; //데미지 원상복귀
        }
    }*/
    
    IEnumerator JumpAttack(Vector3 tPos)
    {
        Vector3 lookAtPosition = Vector3.zero;

        lookAtPosition = new Vector3(tPos.x, this.transform.position.y, tPos.z);

        nav.SetDestination(lookAtPosition); //목적지 설정
        damage.dValue *= 2; // 점프공격시 데미지 2배 적용
        //this.transform.LookAt(lookAtPosition);

        if(isFirstAttack)
        {
            anim.SetTrigger("JumpAttack"); //공격실행
            isFirstAttack = false;
        }
       

        yield return new WaitForSeconds(0.89f);
        nav.isStopped = true;

        damage.dValue /= 2; //데미지 원상복귀
       
        if(isCollision)
        {
            //yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
            mstate = MeleeState.Attack;
        }
        else
        {
            //yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
            mstate = MeleeState.MoveTarget;
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
            nav.velocity = Vector3.zero;
            transform.LookAt(target.transform);
        }      
    }

    //공격 적용
    public void OnAttackEvent()
    {
        StopAllCoroutines();

        LivingEntity enemytarget = target.GetComponent<LivingEntity>(); //타겟의 리빙엔티티 가져오기

        damage.hitPoint = target.GetComponent<Collider>().ClosestPoint(transform.position);

        damage.hitNormal = transform.position - target.transform.position;
      
        if(isCollision) //공격범위 안이라면
        {
           enemytarget.OnDamage(damage); //데미지 이벤트 실행
          
        } 
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
    public override void OnDamage(Damage dInfo)
    {
        if (dead) return;
        health -= dInfo.dValue; //체력 감소

        StopAllCoroutines();

        if (health <= 0 && !dead && this.gameObject.activeInHierarchy) // 체력이 0보다 작고 사망상태가 아닐때
        {
            StartCoroutine(Die());
        }
        else
        {
            switch(dInfo.dType)
            {
                case Damage.DamageType.Melee:
                    if (mstate == MeleeState.JumpAttack)
                      { mstate = MeleeState.NuckBack;  StartCoroutine(NuckBackDamageRoutine(dInfo.ccTime));  }
                    else
                      { StartCoroutine(NormalDamageRoutine()); }//일반 공격일시
                    break;

                case Damage.DamageType.NuckBack:
                    mstate = MeleeState.NuckBack;
                    StartCoroutine(NuckBackDamageRoutine(dInfo.ccTime));
                    break;
                case Damage.DamageType.Stun:
                    mstate = MeleeState.Stun;
                    StartCoroutine(StunRoutine(dInfo.ccTime));
                    break;
            }

        }
        healthbar.SetHealth((int)health);

    }
    
    IEnumerator NormalDamageRoutine()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.KnockBack"))
        { anim.SetTrigger("isHit"); }// 트리거 실행

      
        float startTime = Time.time; //시간체크

        nav.velocity = Vector3.zero;
    
        while (Time.time < startTime + 0.8f)
        {
            nav.velocity = Vector3.zero;
            yield return null;
        }
    }


    IEnumerator NuckBackDamageRoutine(float nuckTime) //넉백시
    {

        nav.velocity = Vector3.zero;

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.KnockBack"))
        { anim.SetTrigger("isKnockBack"); }// 트리거 실행

        float startTime = Time.time;

        while (Time.time < startTime + nuckTime)
        {
            nav.isStopped = true;
           rigid.angularVelocity = Vector3.zero;
            yield return null;
        }

        startTime = Time.time;
        anim.SetTrigger("wakeUp");

        while (Time.time < startTime + 2.8f)
        {
            rigid.angularVelocity = Vector3.zero;
           // nav.isStopped = true;
            yield return null;
        }

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
        { anim.SetTrigger("isStun"); } // 트리거 실행

        float startTime = Time.time;

        while (Time.time < startTime + nuckTime)
        {
            nav.isStopped = true;
            rigid.angularVelocity = Vector3.zero;
            yield return null;
        }


        anim.SetTrigger("wakeUp");

        yield return new WaitForSeconds(0.2f);

        if (isCollision)
        {
            mstate = MeleeState.Attack;
        }
        else
        {
            mstate = MeleeState.MoveTarget;
        }
    }

    //죽었을때
    public IEnumerator Die()
    {

        rigid.isKinematic = true;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.KnockBack"))
        {
            anim.SetTrigger("Lying");
        }
        else
        {
            anim.SetTrigger("isDead"); // 트리거 활성화
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

    

        yield return new WaitForSeconds(1f); // 1초 대기

        //ObjectPool.ReturnMeleeEnemy(this); //다시 오브젝트 풀에 반납

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




   
    /*
    private void OnDrawGizmos() // 범위 그리기
    {
        Handles.color = isCollision ? red : blue;
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angleRange / 2, attackRange);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angleRange / 2, attackRange);

    }*/
}
