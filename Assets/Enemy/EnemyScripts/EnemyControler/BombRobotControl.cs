using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BombRobotControl : LivingEntity
{
    public enum BombState { None, Idle, MoveTarget, Exploding, KnockBack, Stun, Die };

    public BombState bstate;
    public float Speed = 50.0f;

    public GameObject Explosion;// 폭발 이펙트
    public GameObject target; // 공격 대상

    private Damage damage;
    private bool isEnter = false; // 플레이어가 폭발 범위 내로 들어왔는지
    private bool isWalk;
    private float timer;
    private LivingEntity living;

    private MeshRenderer[] mesh;
    private NavMeshAgent nav;
    private Animator anim;
    private Rigidbody rigid;

    [SerializeField]
    private Healthbar healthbar;

    void Awake()
    {
        mesh = GetComponentsInChildren<MeshRenderer>(); //메쉬 가져오기
        nav = GetComponent<NavMeshAgent>(); //네비게이션 가져오기
        anim = GetComponentInChildren<Animator>(); //애니메이터 가져오기 
        rigid = GetComponent<Rigidbody>(); //리지드 바디

        damage.dType = Damage.DamageType.NuckBack;
        damage.ccTime = 1f;
        damage.dValue = 50f;
        startingHealth = 30f;
    }

    public void SetTarget(GameObject _target) //타겟 설정
    {
        target = _target;
        living = target.GetComponent<LivingEntity>();
    }

    protected override void OnEnable()
    {
        timer = 0;
        Explosion.SetActive(false); //폭발 프리팹 비활성화
        bstate = BombState.Idle; //기본 상태를 유휴 상태로 변경
        this.startingHealth = 100f; //테스트용 설정
        health = startingHealth;
        healthbar.SetMaxHealth((int)startingHealth);
    }


    void Update()
    {
        timer += Time.deltaTime;
        if (target!= null && bstate != BombState.Exploding) //타겟이 있을때만
        {
            nav.SetDestination(target.transform.position); //네비게이션 경로 설정
            bstate = BombState.MoveTarget;
        } 
        /*else if(timer >= 15f && bstate != BombState.Exploding)
        {
            isEnter = true;
            bstate = BombState.Exploding;
            StartCoroutine(Explode()); //폭발 코루틴 실행
            StartCoroutine(ExplosionEffect()); // 폭발 진행 이펙트 루틴 실행
        }*/

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
        if (other.gameObject.layer == 8 && bstate != BombState.Exploding)//충돌한 상대가 플레어 일때
        {
           
            isEnter = true;
            bstate = BombState.Exploding;
            StartCoroutine(Explode()); //폭발 코루틴 실행
            StartCoroutine(ExplosionEffect()); // 폭발 진행 이펙트 루틴 실행
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            isEnter = false;
        }
    }


    private IEnumerator Explode()
    {
        nav.isStopped = true; //네비 멈추기
        nav.velocity = Vector3.zero; //네비 속도 0으로 맞추기
        yield return new WaitForSeconds(2.0f); // 2초간 정지후 실행

        Explosion.transform.position = this.transform.position;
        Explosion.transform.rotation = this.transform.rotation;
        Explosion.SetActive(true);
       
        if(isEnter) //타겟이 공격범위 안이면
        {
            living.OnDamage(damage);
        }

        yield return new WaitForSeconds(0.8f); // 1초간 정지후 실행       
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


    public override void OnDamage(Damage dInfo)
    {
        if (dead) return;
        health -= dInfo.dValue;

        if(health <= 0 && this.gameObject.activeInHierarchy && !dead)
        {
            dead = true;
            StartCoroutine(Die());
        }
        else
        {
            switch (dInfo.dType)
            {
                case Damage.DamageType.Melee:    
                   StartCoroutine(NormalDamageRoutine()); //일반 공격일시
                    break;

                case Damage.DamageType.NuckBack:
                    bstate = BombState.KnockBack;
                    StartCoroutine(NuckBackDamageRoutine(dInfo.ccTime));
                    break;
                case Damage.DamageType.Stun:
                    bstate = BombState.Stun;
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

        while (Time.time < startTime + 0.7f)
        {
            nav.velocity = Vector3.zero;
            yield return null;
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
        yield return new WaitForSeconds(0.1f);

        nav.isStopped = false;
        bstate = BombState.MoveTarget;
        
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

        while (Time.time < startTime + 2f)
        {
            rigid.angularVelocity = Vector3.zero;

            yield return null;
        }
        nav.isStopped = false;
        bstate = BombState.MoveTarget;

    }

    public new IEnumerator Die()
    {

        anim.SetBool("isDead", dead); //죽음 관련 애니메이션
        base.Die();
        dead = true;
        StopAllCoroutines();
        bstate = BombState.Die;
        nav.enabled = false;
        yield return new WaitForSeconds(1f);
        Explosion.SetActive(false);
        ObjectPool.ReturnBombRobot(this);
    }
}
    

