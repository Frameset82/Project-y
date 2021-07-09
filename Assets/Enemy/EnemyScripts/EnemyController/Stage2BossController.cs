using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.AI;

public class Stage2BossController : LivingEntity
{
    #region
    //보스 상태
    public enum BossState 
    {   None = 0, 
        Enable = 1, 
        MoveToTarget = 2,
        NormalAttack = 3,
        PowerAttack = 4,
        ComboAttack = 5,
        Dash = 6,
        BackStep = 7,
        Hide = 8,
        Stun = 9,
        Die = 10
    }
    public LivingEntity[] players;

    [SerializeField]
    private float moveSpeed; //이동속도
    [SerializeField]
    private LivingEntity target; // 공격대상

    private Vector3 targetPos; //공격 대상 위치값
    [SerializeField]
    private GameObject sturnEffect; //스턴 효과
    [SerializeField]
    private Transform sturnTrans; // 스턴 효과를 생성할 트랜스폼 변수

    [SerializeField]
    private BossState bState;

    [Header("대쉬 속성")]
    [SerializeField]
    private float dashSpeed;


    [Header("랜더러 속성")]
    private MeshRenderer[] meshes;
    private SkinnedMeshRenderer[] sMeshs;

    [Header("보스 UI")]
    [SerializeField]
    private GameObject bossUI;//보스 UI 오브젝트
    [SerializeField]
    private BossHpBar hpUI; //보스 Hp UI
    [SerializeField]
    private BossHpBar diffUI; //보스 방어도 UI

    private float timer; //타이머
    private int randState; //랜덤한 보스 상태
    private bool isMove; //움직임 관련
    
    [Header("컴포넌트")]
    private NavMeshAgent nav; 
   // private PhotonView pv; 
    private Rigidbody rigid;
    private Animator anim;

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

    [Header("전투 속성")]
    private Damage nDamage; //일반 공격 데미지
    private Damage pDamage; //강 공격 데미지
    public float attackRange = 7f; // 공격 사거리
    public float diff; //방어도

    [Header("공격범위 속성")]
    public float angleRange = 45f;
    [SerializeField]
    private bool isCollision = false;
    Color blue = new Color(0f, 0f, 1f, 0.2f);
    Color red = new Color(1f, 0f, 0f, 0.2f);
    float dotValue = 0f;
    Vector3 direction;
    #endregion


    private void Awake()
    {
        // 컴포넌트 불러오기
        //pv = GetComponent<PhotonView>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        nav.updateRotation = false; // 네비의회전 기능 비활성화

        meshes = GetComponentsInChildren<MeshRenderer>();
        sMeshs = GetComponentsInChildren<SkinnedMeshRenderer>();
        //pv.ObservedComponents[0] = this;
        //pv.Synchronization = ViewSynchronization.UnreliableOnChange;
    }

    protected override void OnEnable()
    {
        nDamage.dValue = 5f;
        nDamage.dType = Damage.DamageType.Melee; //데미지 종류 설정

        pDamage.dValue = 10f;
        pDamage.dType = Damage.DamageType.Stun;
        pDamage.ccTime = 0.5f;

        bState = BossState.None;
        this.startingHealth = 300f;
        this.diff = 100f;


        base.OnEnable();
    }

    private void FixedUpdate()
    {
        if (hasTarget)//타겟의 위치 지속적으로 업데이트
        {
            targetPos = target.gameObject.transform.position;
        }
    }

    private void Update()
    {
        
        if (hasTarget)
        { sectorCheck(); }

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(Hiding());
        }
    }

    //처음 시작시 실행되는 루틴
    private void StartRoutine()
    {
        StartCoroutine(Enable());
    }

    //첫 시작 모션
    private IEnumerator Enable()
    {
        bState = BossState.Enable;
        yield return new WaitForSeconds(0.5f);
        ShowAnimation((int)bState);
        yield return new WaitForSeconds(6.8f);

    }

    //플레이어 추적 동작
    private IEnumerator MoveRoutine()
    {
        Vector3 lookPosition = Vector3.zero;
        bState = BossState.MoveToTarget;
        lookPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);
        nav.isStopped = false;
        nav.SetDestination(lookPosition);
        transform.LookAt(lookPosition);

        bState = BossState.MoveToTarget;
        ShowAnimation((int)bState);

        yield return new WaitUntil(() => isCollision == true); //플레이어 에게 닿았을시

        ShowAnimation((int)bState);
      
        nav.isStopped = true;
        nav.velocity = Vector3.zero;

        StartCoroutine(ComboAttack()); //일반 공격 동작 실행
    }


    //애니메이션 재생
    private void ShowAnimation(int state)
    {
        switch(state)
        {
            case (int)BossState.Enable:
                anim.SetTrigger("Appear");
                break;
            case (int)BossState.MoveToTarget:
                if (!isCollision)
                    anim.SetTrigger("MoveToTarget");
                else
                    anim.SetTrigger("EndMove");
                break;
            case (int)BossState.NormalAttack:
                anim.SetTrigger("NormalAttack");
                break;
            case (int)BossState.PowerAttack:
                anim.SetTrigger("PowerAttack");
                break;
            case (int)BossState.Dash:
                anim.SetTrigger("DashWheel");
                break;
            case (int)BossState.ComboAttack:
                anim.SetTrigger("ComboAttack");
                break;
            case (int)BossState.BackStep:
                anim.SetTrigger("BackStep");
                break;
            case (int)BossState.Hide:
                anim.SetTrigger("Hide");
                break;
            case (int)BossState.Stun:
                anim.SetTrigger("Stun");
                break;
            case (int)BossState.Die:
                anim.SetTrigger("isDead");
                break;
        }
    }
   
    //랜덤 패턴 실행
    private IEnumerator Think()
    {
        yield return new WaitForSeconds(0.2f);

        randState = Random.Range(0, 8);

        if(isCollision)
        {
            switch(randState)
            {
                case 0:
                case 1:
                case 2:
                    break;

                case 3:
                case 4:
                case 5:
                    StartCoroutine(PowerAttack());
                    break;

                case 6:
                case 7:
                    break;
            }    
        }
    }
    
    //일반 공격
    private IEnumerator NormalAttack()
    {
        bState = BossState.NormalAttack;
        nav.isStopped = true;
        ShowAnimation((int)bState);

        yield return new WaitForSeconds(0.1f);
        StartCoroutine(Think());
    }

    //강 공격
    private IEnumerator PowerAttack()
    {
        bState = BossState.PowerAttack;
        nav.isStopped = true;
        ShowAnimation((int)bState);

        yield return new WaitForSeconds(0.1f);
        StartCoroutine(Think());

    }

    //연속공격
    private IEnumerator ComboAttack()
    {
        bState = BossState.ComboAttack;
        nav.isStopped = true;
        ShowAnimation((int)bState);

        yield return new WaitForSeconds(0.1f);
        StartCoroutine(Think());
    }

    //돌진
    private IEnumerator Dash()
    {
        bState = BossState.Dash;

        Vector3 movePosition = this.transform.position + (transform.forward * 16f);
        ShowAnimation((int)bState);

        nav.speed = dashSpeed;
        nav.acceleration = dashSpeed;

        yield return new WaitForSeconds(0.5f);
        nav.SetDestination(movePosition);
        nav.isStopped = false;

        if (direction.magnitude <= attackRange)
        {
            nav.velocity = Vector3.zero;
            nav.isStopped = true;
            nav.acceleration = 8f;
            nav.speed = moveSpeed;
            yield return new WaitForSeconds(0.2f);          
            StartCoroutine(NormalAttack());
        }

    }

    //은신 루틴
    private IEnumerator Hiding()
    {
        //이동할 지점 설정
        Vector3 tpPos = Random.insideUnitCircle * 4f;
        tpPos.x += target.gameObject.transform.position.x;
        tpPos.z = tpPos.y + target.gameObject.transform.position.z;
        tpPos.y = target.gameObject.transform.position.y;

        foreach (MeshRenderer mesh in meshes)
        {
            mesh.enabled = false;
        }
        foreach(SkinnedMeshRenderer sMesh in sMeshs)
        {
            sMesh.enabled = false;
        }


        yield return new WaitForSeconds(3f);

        this.transform.position = tpPos;

        foreach (MeshRenderer mesh in meshes)
        {
            mesh.enabled = true;
        }
        foreach (SkinnedMeshRenderer sMesh in sMeshs)
        {
            sMesh.enabled = true;
        }


    }

    // 부챗꼴 범위 충돌
    private void sectorCheck() 
    {
        dotValue = Mathf.Cos(Mathf.Deg2Rad * (angleRange / 2));
        direction = target.transform.position - transform.position;
        if (direction.magnitude <= attackRange)
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

 

    // 범위 그리기
    private void OnDrawGizmos() 
    {
        Handles.color = isCollision ? red : blue;
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angleRange / 2, attackRange);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angleRange / 2, attackRange);
    }

    //초기 설정 메소드
    public void Init(float _nDamage, float _pDamage, float _speed, float _diff, float _startHealth = 50f) 
    {
        nav.speed = _speed; //이동속도 설정
        nDamage.dValue = _nDamage; //초기 데미지값 설정
        nDamage.dType = Damage.DamageType.Melee; //데미지 종류 설정

        pDamage.dValue = _pDamage;
        pDamage.dType = Damage.DamageType.NuckBack;
        pDamage.ccTime = 0.5f;

        diff = _diff; //방어도 설정

        this.startingHealth = _startHealth; //초기 HP값 설정
    }

   
}
