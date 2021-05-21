using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using UnityEngine.AI;

public class BossController : LivingEntity
{
   public enum BossState { None, MoveTarget, NormalAttack, SpawnRobot, AmimingShot, BackDash, Dash, Stun,Die}; //보스 상태

    public BossState bState = BossState.None; // 보스 상태 변수
    public float MoveSpeed; // 이동속도
    public GameObject target; // 공격대상
    public Vector3 targetPos; // 공격 대상 위치 
    private bool isRun = false;
    private int randState;

    private NavMeshAgent nav; // NavMesh 컴포넌트
    private Rigidbody rigid; 
    private Animator anim; // 애니메이터 컴포넌트                         
    public BossGun bGun; //보스 총 컴포넌트




 
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
    private Damage nDamage; //노말 데미지
    private Damage sDamge; // 스나이핑 데미지
    public float attackRange = 7f; // 공격 사거리
    public float diff; //방어도

    [Header("대쉬 속성")]
    public float dashSpeed;
    public float dashTime;

    [Header("공격범위 속성")]
    public float angleRange = 45f;
    private bool isCollision = false;
    Color blue = new Color(0f, 0f, 1f, 0.2f);
    Color red = new Color(1f, 0f, 0f, 0.2f);
    float dotValue = 0f;
    Vector3 direction;


    private void Awake()
    {
   
       // bulletLineRenderer = GetComponent<LineRenderer>();
     

        // 컴포넌트 불러오기
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
       // bGun = GetComponentInChildren<BossGun>(); 
        nav.updateRotation = false; // 네비의회전 기능 비활성화
    }

    protected override void OnEnable()
    {
        nDamage.dValue = 50f; //초기 데미지값 설정
        nDamage.dType = Damage.DamageType.Melee; //데미지 종류 설정

        sDamge.dValue = 60f;
        sDamge.dType = Damage.DamageType.NuckBack;
        sDamge.ccTime = 0.5f;

        bState = BossState.None;
        this.startingHealth = 50f; //테스트용 설정



        base.OnEnable();
    }

    public void Init(float _nDamage, float _sDamage, float _speed, float _diff, float _startHealth = 50f) //초기 설정 메소드
    {
        nav.speed = _speed; //이동속도 설정
        nDamage.dValue = _nDamage; //초기 데미지값 설정
        nDamage.dType = Damage.DamageType.Melee; //데미지 종류 설정

        sDamge.dValue = _sDamage;
        sDamge.dType = Damage.DamageType.NuckBack;
        sDamge.ccTime = 0.5f; 

        diff = _diff; //방어도 설정

        this.startingHealth = _startHealth; //초기 HP값 설정
    }



    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bGun.StartFiring(nDamage);
            anim.SetTrigger("Shoot");
            //StartCoroutine(NormalAttack());
            //CreateBomobRobot();
            //StartCoroutine(SnipingShot());
            //StartCoroutine(BackDash());
            //StartCoroutine(Enable());
        }

        sectorCheck();
        CheckState();

        targetPos = target.transform.position;
        anim.SetBool("isRun", isRun);
    }

    // 근접 적 상태 체크
    void CheckState()
    {
        switch (bState)
        {
            case BossState.MoveTarget:
                Run();
                break;
            
        }
    }

    void Run() //타겟으로 이동
    {
        isRun = true;
        Vector3 lookPosition = Vector3.zero;
        bState = BossState.MoveTarget;
        lookPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);
        nav.isStopped = false;
        nav.SetDestination(lookPosition);
        transform.LookAt(lookPosition);

        float dist = Vector3.Distance(lookPosition, transform.position);

        if (dist <= attackRange)
        {
            isRun = false;
            nav.isStopped = true;
            nav.velocity = Vector3.zero;

            bState = BossState.NormalAttack;
            StartCoroutine(NormalAttack());
        }
    }

    IEnumerator Enable() //처음 실행되는 모션
    {
        anim.SetTrigger("Enable");

        yield return new WaitForSeconds(6f);

       
    }

    IEnumerator AThink() // 공격후 생각
    {
        yield return new WaitForSeconds(0.1f);

        randState = Random.Range(0, 3);

        if(direction.sqrMagnitude <= 5f)
        {
            switch(randState)
            {
                case 0 :
                case 1 :
                   // StartCoroutine(BackStep());
                    break;
                case 2:
                    CreateBomobRobot();
                    break;
            }
        }
        else
        {
            switch(randState)
            {
                case 0:
                    bState = BossState.MoveTarget;
                    break;
                case 1:
                    CreateBomobRobot();
                    break;
                case 2:
                    StartCoroutine(SnipingShot());
                    break;

            }
        }
    }

    IEnumerator BThink()
    {
        yield return new WaitForSeconds(0.1f);

        randState = Random.Range(0, 3);

        if(direction.sqrMagnitude <= 5f)
        {
            switch(randState)
            {
                case 0:
                    CreateBomobRobot();
                    break;
                case 1:
                    StartCoroutine(NormalAttack());
                    break;
                case 2:
                  //  StartCoroutine(BackStep());
                    break;
            }
        }
    }

    IEnumerator NormalAttack() //일반 공격
    {   
      for(int i = 0; i< 3; i++)
      {
           
        Vector3 lookPosition = Vector3.zero;
        nav.isStopped = true;
           
        lookPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);

        transform.LookAt(lookPosition);
        anim.SetTrigger("Shoot");

     
      yield return new WaitForSeconds(0.9f);
            
       }

       //StartCoroutine(AThink());
    }



    void CreateBomobRobot()
    {

        bState = BossState.SpawnRobot;

        for (int i= 0; i<4; i++)
        {
            var BombRobot = ObjectPool.GetRobot();
            Vector3 spawnPos = Random.insideUnitCircle * 4f; ;
            spawnPos.x += this.transform.position.x;
            spawnPos.z = spawnPos.y + this.transform.position.z;
            spawnPos.y = this.transform.position.y;

            BombRobot.transform.position = spawnPos;
            BombRobot.SetTarget(target);

            anim.SetTrigger("Spawn");
        }

        //StartCoroutine(Think());
    }
  
    bool WallCheck() //뒷쪽에 장애물이 있는지 체크
    {
        RaycastHit hit; //레이캐스트 
        Vector3 hitPosition = Vector3.zero; //레이를 쏠 방향
        
        if(Physics.Raycast(this.transform.position, this.transform.forward * -1f, out hit, 6f))
        {
            if(hit.collider.gameObject.tag == "Wall")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

    }

    IEnumerator SnipingShot()
    {

        bState = BossState.AmimingShot;

        anim.SetTrigger("Sniping");
        yield return new WaitForSeconds(0.7f);

        for (int i = 0; i < 6; i++)
        {
            yield return new WaitForSeconds(0.1f);
            bState = BossState.AmimingShot;
            Vector3 lookPosition = Vector3.zero;
            nav.isStopped = true;
            lookPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);
            transform.LookAt(lookPosition);
            anim.SetTrigger("SnipingShoot");

            yield return new WaitForSeconds(0.7f);
        }
        anim.SetTrigger("SnipingEnd");

       // StartCoroutine(Think());
    }


    IEnumerator Dash() //대쉬
    {
        bState = BossState.Dash;

        float startTime = Time.time;
        Vector3 lookPosition = Vector3.zero;

        

        while (Time.time < startTime + dashTime)
        {
            lookPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);
            nav.SetDestination(lookPosition);
            nav.speed = dashSpeed;
            nav.isStopped = false;
            nav.acceleration = dashSpeed;           
            transform.LookAt(lookPosition);
            anim.SetTrigger("Dash");

            yield return null;
           // yield return new WaitForSeconds(0.3f);
        }

        nav.velocity = Vector3.zero;
        nav.isStopped = true;
        nav.acceleration = 8f;
        nav.speed = MoveSpeed;

       // StartCoroutine(Think());
    }



    IEnumerator BackDash()
    {

        bState = BossState.BackDash;
        float startTime = Time.time;
        Vector3 movePosition = this.transform.position - (transform.forward * 6f);

    
        while (Time.time < startTime + dashTime)
        {
            nav.SetDestination(movePosition);
            nav.speed = dashSpeed;
            nav.isStopped = false;
            nav.acceleration = dashSpeed;
            //lookPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);
            //transform.LookAt(lookPosition);
            anim.SetTrigger("BackDash");
            yield return null;
        }

        nav.velocity = Vector3.zero;
        nav.isStopped = true;
        nav.acceleration = 8f;
        nav.speed = MoveSpeed;
    }


    void sectorCheck() // 부챗꼴 범위 충돌
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

    /*
    private void OnDrawGizmos() // 범위 그리기
    {
        Handles.color = isCollision ? red : blue;
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angleRange / 2, attackRange);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angleRange / 2, attackRange);
    }*/


  

}
