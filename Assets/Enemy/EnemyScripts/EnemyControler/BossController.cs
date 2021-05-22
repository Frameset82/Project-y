using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.AI;

public class BossController : LivingEntity
{
   public enum BossState { None, MoveTarget, NormalAttack, SpawnRobot, AmimingShot, BackDash, Dash, Stun,Die}; //보스 상태

    public Text text;
    public Text text2;

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
        sDamge.dType = Damage.DamageType.Stun;
        sDamge.ccTime = 0.5f;

        bState = BossState.None;
        this.startingHealth = 10000f; //테스트용 설정
        this.diff = 50f;

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
        text.text = "diff: " + diff;
        text2.text = "HP: " + health;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //StartCoroutine(NormalAttack());
            //anim.SetTrigger("Shoot");
            //StartCoroutine(NormalAttack());
            //CreateBomobRobot();
          // StartCoroutine(SnipingShot());
           // StartCoroutine(BackDash());
            StartCoroutine(Enable());
        }

        sectorCheck();
        CheckState();

        targetPos = target.transform.position;
   
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

        StartCoroutine(Dash());
       
    }

    IEnumerator Think() // 패턴설정
    {     
        yield return new WaitForSeconds(0.2f);
        //StopAllCoroutines();

        randState = Random.Range(0, 7);

        if(direction.magnitude <= attackRange)
        {
            switch (randState)
            {
                case 0:
                case 1:
                case 4:
                case 5:
                    if (!WallCheck())
                    { StartCoroutine(BackDash()); }
                    else
                    { StartCoroutine(CreateBomobRobot()); }
                    break;
                case 2:
                case 3:
                case 6:
                    StartCoroutine(CreateBomobRobot());
                    break;
            }
        }
        else
        {
            switch(randState)
            {
                case 0:
                case 3:
                case 4:
                    StartCoroutine(Dash());
                    break;
                case 1:
                case 5:
                    StartCoroutine(CreateBomobRobot());
                    break;
                case 2:
                case 6:
                    StartCoroutine(SnipingShot());
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

     
        yield return new WaitForSeconds(0.1f);
        bGun.StartFiring(nDamage);

        yield return new WaitForSeconds(0.5f);
        }

       StartCoroutine(Think());
    }

    IEnumerator CreateBomobRobot()
    { 
        bState = BossState.SpawnRobot;

        randState = Random.Range(1, 4);

        for (int i= 0; i< randState; i++)
        {
            var BombRobot = ObjectPool.GetRobot();
            Vector3 spawnPos = Random.insideUnitCircle * 4f; ;
            spawnPos.x += this.transform.position.x;
            spawnPos.z = spawnPos.y + this.transform.position.z;
            spawnPos.y = this.transform.position.y;

            BombRobot.transform.position = spawnPos;
            BombRobot.SetTarget(target);
        }
        anim.SetTrigger("Spawn");

        yield return new WaitForSeconds(0.3f);

        StartCoroutine(Think());
    }//로봇소환
  
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

        nav.isStopped = true;
        anim.SetTrigger("Sniping");
        yield return new WaitForSeconds(0.7f);


        for (int i=0; i < 3; i++)
        {
       
            transform.LookAt(targetPos);
            anim.SetTrigger("SnipingShoot");
            bGun.StartFiring(sDamge);
            //Vector3 lookPosition = Vector3.zero;

            //lookPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);
            yield return new WaitForSeconds(0.6f);
        }


        yield return new WaitForSeconds(2f);
        anim.SetTrigger("SnipingEnd");

        
        StartCoroutine(Think());
    } //스나이핑 샷


    IEnumerator Dash() //대쉬
    {
        bState = BossState.Dash;

        float startTime = Time.time;
        Vector3 lookPosition = Vector3.zero;

        //Time.time < startTime + dashTime
        while (!isCollision)
        {
            lookPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);
            nav.SetDestination(lookPosition);
            nav.speed = dashSpeed;
            nav.isStopped = false;
            nav.acceleration = dashSpeed;           
            transform.LookAt(lookPosition);
            anim.SetTrigger("Dash");

            yield return null;        
        }

        nav.velocity = Vector3.zero;
        nav.isStopped = true;
        nav.acceleration = 8f;
        nav.speed = MoveSpeed;


        yield return new WaitForSeconds(0.3f);

        StartCoroutine(NormalAttack());
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

        yield return new WaitForSeconds(0.2f);
        StartCoroutine(SnipingShot());
    }//백 스텝

    // 공격을 당했을때
    public override void OnDamage(Damage dInfo)
    {
        if (dead) return;

        //StopAllCoroutines();


        health -= dInfo.dValue; // 체력 감소

        diff -= (int)((dInfo.dValue * dInfo.inCapValue) / 100); //총공격력에서 무력화수치 퍼센트만큼 방어도 감소

        if (diff <= 0 && bState != BossState.Stun)
        {          
            StopAllCoroutines();
            StartCoroutine(Stun());
        }
        else if(diff<= 0)
        {
            diff = 0;
        }
     
        if (health <= 0 && this.gameObject.activeInHierarchy && !dead) // 체력이 0보다 작고 사망상태가 아닐때
        {
            StopAllCoroutines();
            StartCoroutine(Die());
        }
        
    }

    IEnumerator Stun()
    {
        bState = BossState.Stun;

        anim.SetTrigger("isStun");


        yield return new WaitForSeconds(3f);

        diff = 50f;

        StartCoroutine(Think());
    }


    //죽었을때
    public new IEnumerator Die()
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


        bState = BossState.Die; // 죽음상태로 변경

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

    
    private void OnDrawGizmos() // 범위 그리기
    {
        Handles.color = isCollision ? red : blue;
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angleRange / 2, attackRange);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angleRange / 2, attackRange);
    }


  

}
