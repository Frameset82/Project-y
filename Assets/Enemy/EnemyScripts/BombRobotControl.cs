using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BombRobotControl : LivingEntity
{
    public enum BombState { None, Idle, MoveTarget, Exploding, Die };

    public BombState bstate;
    public float Speed = 50.0f;

    public GameObject Explosion;// 폭발 이펙트
    public GameObject target; // 공격 대상

    private bool isEnter = false; // 한번만 실행 시키기 위한 bool 함수
    private bool isWalk;

    MeshRenderer[] mesh;
    NavMeshAgent nav;
    Animator anim;


    void Awake()
    {
        mesh = GetComponentsInChildren<MeshRenderer>(); //메쉬 가져오기
        nav = GetComponent<NavMeshAgent>(); //네비게이션 가져오기
        anim = GetComponentInChildren<Animator>(); //애니메이터 가져오기 
    }

    public void SetTarget(GameObject _target) //타겟 설정
    {
        target = _target;
    }

    protected override void OnEnable()
    {
        Explosion.SetActive(false); //폭발 프리팹 비활성화
        bstate = BombState.Idle; //기본 상태를 유휴 상태로 변경
    }


    void Update()
    {
       
        if (target!= null && !isEnter) //타겟이 있을때만
        {
            nav.SetDestination(target.transform.position); //네비게이션 경로 설정
            bstate = BombState.MoveTarget;
        }
       
        CheckState(); //상태 체크
        anim.SetBool("isWalk", isWalk); //걷기 관련 애니메이션 
       

    }

    void CheckState()
    {
        switch(bstate)
        {
           
            case BombState.MoveTarget:
                isWalk = true;
                break;
            case BombState.Exploding:
            case BombState.Die:
            case BombState.Idle:
                isWalk = false;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")//충돌한 상대가 플레어 일때
        {
            isEnter = true;
            bstate = BombState.Exploding;
            nav.isStopped = true; //네비 멈추기
            nav.velocity = Vector3.zero; //네비 속도 0으로 맞추기
            StartCoroutine(Explode()); //폭발 코루틴 실행
            StartCoroutine(ExplosionEffect()); // 폭발 진행 이펙트 루틴 실행
        }
    }

    private IEnumerator Explode()
    {

        yield return new WaitForSeconds(2.0f); // 2초간 정지후 실행
        Explosion.transform.position = this.transform.position;
        Explosion.transform.rotation = this.transform.rotation;
        Explosion.SetActive(true);
        yield return new WaitForSeconds(1f); // 1초간 정지후 실행       
        Die();

    }

    IEnumerator ExplosionEffect()
    {
        while (true)
        {
            foreach(MeshRenderer mesh in mesh)
            {
                mesh.material.color = Color.red;
            }
            yield return new WaitForSeconds(0.3f);

            foreach (MeshRenderer mesh in mesh)
            {
                mesh.material.color = Color.white;
            }
            yield return new WaitForSeconds(0.1f);
        }     
    }


    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);

        if(health< 0)
        {
            dead = true;
        }
    }

    public override void Die()
    {
        base.Die();
        dead = true;
        bstate = BombState.Die;
        anim.SetBool("isDead", dead); //죽음 관련 애니메이션
        StopAllCoroutines();
        nav.enabled = false;
        Explosion.SetActive(false);
        ObjectPool.ReturnBombRobot(this);
    }
}
    

