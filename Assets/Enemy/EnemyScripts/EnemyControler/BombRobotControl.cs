using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class BombRobotControl : LivingEntity, IPunObservable
{
    public enum BombState { None, Idle, MoveTarget, Exploding, KnockBack, Stun, Die };

    public BombState bstate;
    public float Speed = 50.0f;

    public GameObject Explosion;// 폭발 이펙트
    public GameObject target; // 공격 대상
    public GameObject DangerRange;

    private Damage damage;
    private bool isWalk;
  
    private MeshRenderer[] mesh;
    private MeshRenderer[] Defaultmesh;

    private NavMeshAgent nav;
    private Animator anim;
    private Rigidbody rigid;
    private PhotonView pv;
    private Vector3 targetPos;

    [SerializeField]
    private Healthbar healthbar;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        mesh = this.transform.GetChild(2).GetComponentsInChildren<MeshRenderer>();  
        Defaultmesh = this.transform.GetChild(2).GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>(); //네비게이션 가져오기
        anim = GetComponentInChildren<Animator>(); //애니메이터 가져오기 
        rigid = GetComponent<Rigidbody>(); //리지드 바디

        damage.dType = Damage.DamageType.NuckBack;
        damage.ccTime = 1f;
        damage.dValue = 50f;
        startingHealth = 30f;

        pv.ObservedComponents[0] = this;
        pv.Synchronization = ViewSynchronization.UnreliableOnChange;
    }

    protected override void OnEnable()
    {
        DangerRange.SetActive(false);
       
        Explosion.SetActive(false); //폭발 프리팹 비활성화
        bstate = BombState.Idle; //기본 상태를 유휴 상태로 변경
        this.startingHealth = 100f; //테스트용 설정
        health = startingHealth;
        healthbar.SetMaxHealth((int)startingHealth);
    }


    public void SetTarget(GameObject _target) //타겟 설정
    {
        if (!PhotonNetwork.IsMasterClient) { return; }
        target = _target;
         
         bstate = BombState.MoveTarget;
    }

    void Update()
    {
        healthbar.SetHealth((int)health);

        if (!PhotonNetwork.IsMasterClient) { return; }

        if (target!= null && bstate != BombState.Exploding && gameObject.activeSelf) //타겟이 있을때만
        { 
            targetPos = target.transform.position;
        } 

        if(target.GetComponent<LivingEntity>().dead)
        { Die();  }
     
        CheckState(); //상태 체크
        anim.SetBool("isWalk", isWalk); //걷기 관련 애니메이션 
       

    }

    void CheckState()
    {
        switch(bstate)
        {
            case BombState.MoveTarget:
                Move();
                isWalk = true;
                break;
            case BombState.Exploding:
            case BombState.Die:
            case BombState.Idle:
                isWalk = false;
                break;
        }
    }

    void Move()
    {
        if(!dead)
        nav.SetDestination(targetPos); //네비게이션 경로 설정
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) { return; }
        else if (other.gameObject.layer == 8 && bstate != BombState.Exploding)//충돌한 상대가 플레어 일때
        {           
            bstate = BombState.Exploding;
            pv.RPC("StartExplosion", RpcTarget.All);
        }

    }

    [PunRPC]
    void StartExplosion()
    {
        StartCoroutine(Explode()); //폭발 코루틴 실행
        StartCoroutine(ExplosionEffect()); // 폭발 진행 이펙트 루틴 실행
    }


    private IEnumerator Explode()
    {
        nav.isStopped = true; //네비 멈추기
        nav.velocity = Vector3.zero; //네비 속도 0으로 맞추기
        DangerRange.SetActive(true);

        yield return new WaitForSeconds(2.0f); // 2초간 정지후 실행

        if(PhotonNetwork.IsMasterClient)
        {
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
              2f, Vector3.up, 0f, LayerMask.GetMask("Player"));

            foreach (RaycastHit hitObj in rayHits)
            {
                hitObj.transform.GetComponent<LivingEntity>().OnDamage(damage);
            }
        }


        Explosion.transform.position = this.transform.position;
        Explosion.transform.rotation = this.transform.rotation;
        Explosion.SetActive(true);
     
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

        else if(PhotonNetwork.IsMasterClient)
        {
            health -= dInfo.dValue;

            if (health <= 0 && this.gameObject.activeInHierarchy && !dead)
            {
                dead = true;
                Die();
            }
            else
            {
                StopAllCoroutines();
                DamageEvent((int)dInfo.dType, dInfo.ccTime);
            }
        }  
    }

    void DamageEvent(int dType, float ccTime)
    {
        switch (dType)
        {
            case 1:
                StartCoroutine(NormalDamageRoutine()); //일반 공격일시
                break;

            case 2:
                bstate = BombState.Stun;
                StartCoroutine(StunRoutine(ccTime));     
                break;

            case 3:
                bstate = BombState.KnockBack;
                StartCoroutine(NuckBackDamageRoutine(ccTime));
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
                anim.SetTrigger("isDead");
                break;
        }
    }


    IEnumerator NormalDamageRoutine()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.KnockBack"))
        { pv.RPC("ShowAnimation", RpcTarget.All, 4); }// 트리거 실행


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
        { pv.RPC("ShowAnimation", RpcTarget.All, 3); } // 트리거 실행

        float startTime = Time.time;

        while (Time.time < startTime + nuckTime)
        {
            nav.isStopped = true;
            rigid.angularVelocity = Vector3.zero;
            yield return null;
        }

        pv.RPC("ShowAnimation", RpcTarget.All, 2);
        yield return new WaitForSeconds(0.1f);

        nav.isStopped = false;
        bstate = BombState.MoveTarget;
        
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

        while (Time.time < startTime + 2f)
        {
            rigid.angularVelocity = Vector3.zero;
            yield return null;
        }
        nav.isStopped = false;
        bstate = BombState.MoveTarget;
    }

    
    public override void Die()
    {
        base.Die();
        StopAllCoroutines();
        StartCoroutine(Death());
    }


    public IEnumerator Death()
    {
        ShowAnimation(5);

        bstate = BombState.Die;
        dead = true; 
        nav.enabled = false;
        yield return new WaitForSeconds(1f);

        Explosion.SetActive(false);

        for(int i = 0; i< Defaultmesh.Length; i++)
        {
            mesh[i].material.color = Defaultmesh[i].material.color;
        }

     

        if (PhotonNetwork.IsMasterClient)
        {
            ObjectPool.ReturnBombRobot(this);
        }
       
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
}


