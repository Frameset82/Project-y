using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
//using UnityEditor;
using UnityEngine.Animations.Rigging;
using UnityEngine.AI;

public class RifleController : LivingEntity, IPunObservable
{
    public enum RifleState { None, Idle, MoveTarget, KnockBack, Stun, Attack, Die };

    [Header("기본속성")]
    public RifleState rstate = RifleState.None; // 근접적 상태변수
    public float MoveSpeed = 3.5f; //이동 속도
    public Vector3 targetPos; //공격 대상 위치
    public GameObject target; // 공격 대상
    public int Idlestate;

    private PhotonView pv;
    private NavMeshAgent nav; // NavMesh 컴포넌트
    private Rigidbody rigid; 
    private Animator anim; // 애니메이터 컴포넌트
    public EnemyGun eGun; 

    [SerializeField]
    private Healthbar healthbar;

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
    public Damage damage; // 공격력
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
        pv = GetComponent<PhotonView>();
        nav = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        nav.updateRotation = false; // 네비의회전 기능 비활성화

        pv.ObservedComponents[0] = this;
        pv.Synchronization = ViewSynchronization.UnreliableOnChange;
    }

    protected override void OnEnable()
    {
        //대기 상태로 설정
        rstate = RifleState.Idle;
        this.startingHealth = 50f; //테스트용 설정
        healthbar.SetMaxHealth((int)startingHealth);
        base.OnEnable();
    }

    [PunRPC]
    public void Init(float _damage, float _speed, float _startHealth = 50f) //초기 설정 메소드
    {
        nav.speed = _speed; //이동속도 설정
        damage.dValue = _damage; //초기 데미지값 설정
        damage.dType = Damage.DamageType.Melee; //데미지 종류 설정
        this.startingHealth = _startHealth; //초기 HP값 설정
    }

    
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
                break;

            case RifleState.MoveTarget:
                move = true;
                attack = false;
                break;

            case RifleState.Attack:
                move = false;
                attack = true;
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
            else
            {
                lookAtPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);
                // 추적 실행
                nav.isStopped = false;
                transform.LookAt(lookAtPosition);
                nav.SetDestination(lookAtPosition); // 목적지 설정
             
            }          
        }   
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
    public void OnAttackEvent()
    {

        StopAllCoroutines();
        eGun.Fire(damage,attackRange);

       // attackTarget.OnDamage(damage, hitPoint, hitNormal);
    }

    // 공격을 당했을때
   
    [PunRPC]
    public override void OnDamage(Damage dInfo)
    {
        if (dead) return;

        StopAllCoroutines();


        health -= dInfo.dValue; // 체력 감소


        if (health <= 0 && this.gameObject.activeInHierarchy && !dead) // 체력이 0보다 작고 사망상태가 아닐때
        {
            Die();
            //StartCoroutine(Die());
        }
        else
        {
            switch (dInfo.dType)
            {
                case Damage.DamageType.Melee:
                    StartCoroutine(NormalDamageRoutine());//일반 공격일시
                    break;

                case Damage.DamageType.NuckBack:
                    rstate = RifleState.KnockBack;
                    StartCoroutine(NuckBackDamageRoutine(dInfo.ccTime));
                    break;
                case Damage.DamageType.Stun:
                    rstate = RifleState.Stun;
                    StartCoroutine(StunRoutine(dInfo.ccTime));
                    break;
            }
        }

        healthbar.SetHealth((int)health);
        Debug.Log(health);
    }

    IEnumerator NormalDamageRoutine()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.KnockBack"))
        { anim.SetTrigger("isHit"); } // 트리거 실행}


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

        while (Time.time < startTime + 3.8f)
        {
            rigid.angularVelocity = Vector3.zero;

            yield return null;
        }

        if (isCollision)
        {
            rstate = RifleState.Attack;
        }
        else
        {
            rstate = RifleState.MoveTarget;
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
            rstate = RifleState.Attack;
        }
        else
        {
            rstate = RifleState.MoveTarget;
        }
    }


    void OnSetTarget(GameObject _target) //타겟설정
    {
        if (hasTarget || !PhotonNetwork.IsMasterClient) //이미 타겟이 있거나 마스터 클라이언트가 아니라면
        {
            return;
        }
        target = _target;
        //타겟을 향해 이동하는 상태로 전환
        rstate = RifleState.MoveTarget;
    }

    public override void Die()
    {
        base.Die();
        StartCoroutine(Death());
    }

    //죽었을때
    public IEnumerator Death()
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


        rstate = RifleState.Die; // 죽음상태로 변경

        nav.isStopped = true; //네비 멈추기
        nav.enabled = false; // 네비 비활성화
        dead = true;

        Collider[] enemyColliders = GetComponents<Collider>();

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
        if (!PhotonNetwork.IsMasterClient)
        { return; }

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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(rstate);
      
        }
        else
        {
            rstate = (RifleState)stream.ReceiveNext();
  
        }
     
    }

    /*
    private void OnDrawGizmos() // 범위 그리기
    {

        Handles.color = isCollision ? red : blue;
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angleRange / 2, attackRange);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angleRange / 2, attackRange);

    }*/
}
