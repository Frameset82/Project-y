using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class BossController : LivingEntity
{
   public enum BossState { None, MoveTarget, NormalAttack, SpawnRobot, AmimingShot, Avoid, Die}; //보스 상태

    public BossState bState = BossState.None; // 보스 상태 변수
    public float MoveSpeed; // 이동속도
    public GameObject target; // 공격대상
    public Vector3 targetPos; // 공격 대상 위치 
    private bool isRun = false;


    private NavMeshAgent nav; // NavMesh 컴포넌트
    private Animator anim; // 애니메이터 컴포넌트                         
  

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
    public float damage = 20f; // 공격력
    public float attackRange = 7f; // 공격 사거리

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
 
    }

    protected override void OnEnable()
    { 
        nav.speed = MoveSpeed; //스피드 설정
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            //StartCoroutine(NormalAttack());
            //CreateBomobRobot();
             //StartCoroutine(SnipingShot());
            StartCoroutine(Dash(targetPos));
            
        }
        sectorCheck();
        //Run();
        targetPos = target.transform.position;

    
    }


    IEnumerator NormalAttack()
    {
      for(int i = 0; i< 3; i++)
        {
            bState = BossState.NormalAttack;
            Vector3 lookPosition = Vector3.zero;
            nav.isStopped = true;
           
            lookPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);

            transform.LookAt(lookPosition);
            anim.SetTrigger("Shoot");

            yield return new WaitForSeconds(0.5f);
        }     

    }

    void CreateBomobRobot()
    {
        for(int i= 0; i<4; i++)
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
    }
  

    IEnumerator SnipingShot()
    {
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
    }


    IEnumerator Dash(Vector3 dashPos) //대쉬
    {
        float startTime = Time.time;
        Vector3 lookPosition = Vector3.zero;

        while (Time.time < startTime + dashTime)
        {
            nav.SetDestination(dashPos);
            nav.speed = dashSpeed;
            nav.isStopped = false;
            nav.acceleration = dashSpeed;
            lookPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);
            transform.LookAt(lookPosition);
            anim.SetTrigger("Dash");

            yield return null;
        }

        nav.velocity = Vector3.zero;
        nav.isStopped = true;
        nav.acceleration = 8f;
        nav.speed = MoveSpeed;

    }

    void Run()
    {
        Vector3 lookPosition = Vector3.zero;
        bState = BossState.MoveTarget;
        lookPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);
        nav.isStopped = false;
        nav.SetDestination(lookPosition);
        isRun = true;
        anim.SetBool("isRun", isRun);

        if (isCollision&& isRun)
        {
            isRun = false;
            nav.isStopped = true;
            nav.velocity = Vector3.zero;
            anim.SetBool("isRun", isRun);
            return;
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

        Handles.color = isCollision ? red : blue;
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angleRange / 2, attackRange);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angleRange / 2, attackRange);

    }
}
