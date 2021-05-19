﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class BossController : LivingEntity
{
   public enum BossState { None, MoveTarget, NormalAttack, SpawnRobot, AmimingShot, Avoid, Dash, Stun,Die}; //보스 상태

    public BossState bState = BossState.None; // 보스 상태 변수
    public float MoveSpeed; // 이동속도
    public GameObject target; // 공격대상
    public Vector3 targetPos; // 공격 대상 위치 
    private bool isRun = false;
    private int randState;

    private NavMeshAgent nav; // NavMesh 컴포넌트
    private Rigidbody rigid; 
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
    private Damage damage;
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
        nav.updateRotation = false; // 네비의회전 기능 비활성화
    }

    protected override void OnEnable()
    {
        bState = BossState.None;
        this.startingHealth = 50f; //테스트용 설정
        base.OnEnable();
    }

    public void Init(float _damage, float _speed, float _diff, float _startHealth = 50f) //초기 설정 메소드
    {
        nav.speed = _speed; //이동속도 설정
        damage.dValue = _damage; //초기 데미지값 설정
        damage.dType = Damage.DamageType.Melee; //데미지 종류 설정
        diff = _diff;
        this.startingHealth = _startHealth; //초기 HP값 설정
    }



    private void Update()
    {
        StartCoroutine(Enable());
        if (Input.GetKey(KeyCode.Space))
        {
            //StartCoroutine(NormalAttack());
            //CreateBomobRobot();
            //StartCoroutine(SnipingShot());
            //StartCoroutine(Dash(targetPos));
            
        }

        sectorCheck();
        targetPos = target.transform.position;
    }


    IEnumerator Enable() //처음 실행되는 모션
    {
        anim.SetTrigger("Enable");

        yield return new WaitForSeconds(4f);

        Run();
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        randState = Random.Range(0, 3);

        switch (bState)
        {
            case BossState.MoveTarget: 
                break;
            case BossState.NormalAttack:
             

      

                break;
            case BossState.AmimingShot:
                break;
            case BossState.Avoid:
                break;
            case BossState.SpawnRobot:
                break;
            case BossState.Dash:
                break;
        }
    }


    IEnumerator NormalAttack() //일반 공격
    {
        bState = BossState.NormalAttack; 
        
      for(int i = 0; i< 3; i++)
      {
           
        Vector3 lookPosition = Vector3.zero;
        nav.isStopped = true;
           
        lookPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);

        transform.LookAt(lookPosition);
        anim.SetTrigger("Shoot");

        yield return new WaitForSeconds(0.5f);
      }

        StartCoroutine(Think());
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

        StartCoroutine(Think());
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

        StartCoroutine(Think());
    }


    IEnumerator Dash(Vector3 dashPos) //대쉬
    {
        bState = BossState.Dash;

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

        StartCoroutine(Think());
    }

    IEnumerator BackStep(Vector3 dashPos)
    {
        bState = BossState.Dash;

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

        StartCoroutine(Think());
    }

    IEnumerator Avoid(Vector3 pos)
    {
        bState = BossState.Avoid;
        float startTime = Time.time;
        Vector3 lookPosition = Vector3.zero;


        while (Time.time < startTime + dashTime)
        {
            nav.SetDestination(pos);
            nav.speed = dashSpeed;
            nav.isStopped = false;
            nav.acceleration = dashSpeed;
            lookPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);
            transform.LookAt(lookPosition);
            anim.SetTrigger("Dash");

            yield return null;
        }
    }

    void Run() //타겟으로 이동
    {
        Vector3 lookPosition = Vector3.zero;
        bState = BossState.MoveTarget;
        lookPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);
        nav.isStopped = false;
        nav.SetDestination(lookPosition);
        transform.LookAt(lookPosition);
        isRun = true;
        anim.SetBool("isRun", isRun);

        if (isCollision&& isRun)
        {
            isRun = false;
            nav.isStopped = true;
            nav.velocity = Vector3.zero;
            anim.SetBool("isRun", isRun);
            StartCoroutine(NormalAttack());
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
