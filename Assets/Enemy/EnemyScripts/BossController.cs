using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController : LivingEntity
{
   public enum BossState { None, MoveTarget, NormalAttack, SpawnRobot, AmimingShot, Avoid, Die}; //보스 상태

    public BossState bState = BossState.None; // 보스 상태 변수
    public float MoveSpeed; // 이동속도
    public GameObject target; // 공격대상
    public Vector3 targetPos; // 공격 대상 위치 

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
        nav.speed = MoveSpeed;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(NormalAttack());
        }
        targetPos = target.transform.position;
    }

    void CheckAnimaitonState()
    {
        switch(bState)
        {
            case BossState.NormalAttack:
                anim.SetTrigger("Shoot");
                break;
        }
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

  
}
