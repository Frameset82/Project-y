using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using UnityEngine.AI;

public class RogueController : LivingEntity, IPunObservable
{
    public enum RogueState { None, Idle, MoveTarget, Teleport, KnockBack, Stun, Attack, Die };

    [Header("기본속성")]
    public RogueState rstate = RogueState.None; // 근접적 상태변수
    public float MoveSpeed = 3.5f; //이동 속도
    public Vector3 targetPos; //공격 대상 위치
    public GameObject target; // 공격 대상
    public int Idlestate;
    public Transform pTr;
    public AudioClip clip;
    private PhotonView pv;
    private NavMeshAgent nav; // NavMesh 컴포넌트
    private Animator anim; // 애니메이터 컴포넌트
    private Rigidbody rigid;

    private new static float health;

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
    public Damage damage; // 공격속성           

    private float attackRange = 2f; // 공격 사거리


    [Header("텔레포트 속성")]
    private float curTpTime; //최근 텔레포트한 시간
    private float tpCooldown = 17f; //텔레포트 대기시간
    private bool bTeleportation = true; // 텔레포트 가능 여부

    private Vector3 tpPos; //텔레포트에 사용할 좌표
    private Vector3 lookAtPosition;

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
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        nav.updateRotation = false; // 네비의회전 기능 비활성화

        pv.ObservedComponents[0] = this;
        pv.Synchronization = ViewSynchronization.UnreliableOnChange;


    }


    protected override void OnEnable()
    {
        //대기 상태로 설정
        rstate = RogueState.Idle;
        this.startingHealth = 50f; //테스트용 설정
        healthbar.SetMaxHealth((int)startingHealth);
        base.OnEnable();
    }


    private void Start()
    {
        //this.transform.parent = ObjectPool.objectTrans;
        //this.gameObject.SetActive(false);
    }

    [PunRPC]
    public void Init(float _damage, float _speed, float _startHealth = 50f) //초기 설정 메소드
    {
        nav.speed = _speed; //이동속도 설정
        damage.dValue = _damage; //초기 데미지값 설정
        damage.dType = Damage.DamageType.Melee; //데미지 종류 설정
        this.startingHealth = _startHealth; //초기 HP값 설정
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
                StartCoroutine(AttackUpdate());
                break;
            case RogueState.Teleport:
                TeleportUpdate();
                break;
            case RogueState.Die:

                break;
        }
    }


    [PunRPC]
    void ShowAnimation(int a)
    {
        switch (a)
        {
            case 1:
                anim.SetTrigger("isKnockBack");
                break;
            case 2:
                anim.SetTrigger("wakeUp");
                break;
            case 3:
                anim.SetTrigger("isStun");               
                break;
            case 4:
                anim.SetTrigger("isHit");
                break;
            case 5:
                anim.SetTrigger("Lying");
                break;
            case 6:
                anim.SetTrigger("isDead");
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
     
        if (hasTarget)
        {
            targetPos = target.transform.position;

            sectorCheck();

            if (bTeleportation)//텔레포트가 가능하면
            {
                rstate = RogueState.Teleport;
                return;
            }
            else if (isCollision && !bTeleportation)// 공격범위에 충돌하고 텔레포트가 불가능할시
            {
                rstate = RogueState.Attack; //공격 상태로 변환
            }
            else
            {
                lookAtPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z); //이동시 바라볼 방향 체크                                                                                           
                nav.isStopped = false;   // 추적 실행
                transform.LookAt(lookAtPosition);
                nav.SetDestination(lookAtPosition); // 목적지 설정
            }
        }
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

            sectorCheck();
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
    IEnumerator AttackUpdate()
    {
        nav.isStopped = true; // 네비 멈추기        
        nav.velocity = Vector3.zero; // 이동속도 줄이기
        transform.LookAt(target.transform);

        yield return new WaitForSeconds(2f);

        sectorCheck();
        if (!isCollision) //공격범위보다 멀면
        {
            rstate = RogueState.MoveTarget;
        }

    }

    //공격 적용
    public void OnAttackEvent()
    {
        if (!PhotonNetwork.IsMasterClient)
        { return; }

        LivingEntity attackTarget = target.GetComponent<LivingEntity>();


        damage.hitPoint = target.GetComponent<Collider>().ClosestPoint(transform.position);

        damage.hitNormal = transform.position - target.transform.position;

        sectorCheck();

        if (isCollision)
        {
            attackTarget.OnDamage(damage);
        }
    }

    void OnSetTarget(GameObject _target) //타겟설정
    {
        if (hasTarget || !PhotonNetwork.IsMasterClient) //이미 타겟이 있다면
        {
            return;
        }
        target = _target;
        //타겟을 향해 이동하는 상태로 전환
        rstate = RogueState.MoveTarget;
    }

    // 공격을 당했을때
    public override void OnDamage(Damage dInfo)
    {
        if (dead) return;

        else if (PhotonNetwork.IsMasterClient)
        {
            health -= dInfo.dValue; //체력 감소      

            if (health <= 0 && !dead && this.gameObject.activeInHierarchy) // 체력이 0보다 작고 사망상태가 아닐때
            {
                Die();             
            }
            else
            {
                StopAllCoroutines();

                DamageEvent((int)dInfo.dType, dInfo.ccTime);
                SoundManager.instance.SFXPlay(clip, this.gameObject);
            }
        }
        else
        {
            health -= dInfo.dValue;
        }
    }

    void DamageEvent(int dType, float ccTime)
    {
        
        switch (dType)
        {
            case 1:
                StartCoroutine(NormalDamageRoutine());//일반 공격일시
                break;
            case 2:
                rstate = RogueState.Stun;
                StartCoroutine(StunRoutine(ccTime));
                break;
            case 3:
                rstate = RogueState.KnockBack;
                StartCoroutine(NuckBackDamageRoutine(ccTime));
                break;

        }
 
    }

    IEnumerator NormalDamageRoutine()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.KnockBack"))
        { pv.RPC("ShowAnimation", RpcTarget.All, 4); } // 트리거 실행}


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
            rstate = RogueState.Attack;
        }
        else
        {
            rstate = RogueState.MoveTarget;
        }
    }

    IEnumerator NuckBackDamageRoutine(float nuckTime) //넉백시
    {

        nav.velocity = Vector3.zero;

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.KnockBack"))
        { pv.RPC("ShowAnimation", RpcTarget.All, 1); }// 트리거 실행

        float startTime = Time.time;

        while (Time.time < startTime + nuckTime)
        {
            nav.isStopped = true;
            rigid.angularVelocity = Vector3.zero;
            yield return null;
        }

        startTime = Time.time;
        pv.RPC("ShowAnimation", RpcTarget.All, 2);

        while (Time.time < startTime + 3.8f)
        {
            rigid.angularVelocity = Vector3.zero;          
            yield return null;
        }


        sectorCheck();
        if (isCollision)
        {
            rstate = RogueState.Attack;
        }
        else
        {
            rstate = RogueState.MoveTarget;
        }
    }

    IEnumerator StunRoutine(float nuckTime) //스턴
    {
        nav.velocity = Vector3.zero;

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.KnockBack") )
        { pv.RPC("ShowAnimation", RpcTarget.All, 3); } // 트리거 실행

        float startTime = Time.time;

        while (Time.time < startTime + nuckTime)
        {
            nav.isStopped = true;
            rigid.angularVelocity = Vector3.zero;
            yield return null;
        }


        pv.RPC("ShowAnimation", RpcTarget.All, 2);

        yield return new WaitForSeconds(0.2f);

        sectorCheck();
        if (isCollision)
        {
            rstate = RogueState.Attack;
        }
        else
        {
            rstate = RogueState.MoveTarget;
        }
    }



    [PunRPC]
    public override void Die()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("Die", RpcTarget.Others);
        }

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
            ShowAnimation(5);
        }
        else
        {
            ShowAnimation(6);
        }


        rstate = RogueState.Die; // 죽음상태로 변경

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

        ObjectPool.ReturnRogue(this); //다시 오브젝트 풀에 반납
    }

    private void Update()
    {
        healthbar.SetHealth((int)health);

        if (!PhotonNetwork.IsMasterClient)
        { return; }

        if (hasTarget) //타겟이 있다면
        {
            targetPos = target.transform.position;
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {  
            health = (float)stream.ReceiveNext();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && PhotonNetwork.IsMasterClient)
        { rigid.isKinematic = true; }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && PhotonNetwork.IsMasterClient)
        { rigid.isKinematic = false; }
    }

    //private void OnDrawGizmos() // 범위 그리기
    //{
    //    Handles.color = isCollision ? red : blue;
    //    Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angleRange / 2, attackRange);
    //    Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angleRange / 2, attackRange);

    //}
}
