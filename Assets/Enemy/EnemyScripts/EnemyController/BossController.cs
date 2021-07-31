using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using UnityEngine.UI;
using UnityEngine.AI;
using Photon.Pun;

public class BossController : LivingEntity, IPunObservable
{
    public enum BossState { None = 0, MoveTarget= 1, NormalAttack = 2, SpawnRobot =3, AmimingShot=4, BackDash =5, Dash = 6, Stun =7, Die=8 }; //보스 상태


    public AudioClip[] clips;
    public BossState bState = BossState.None; // 보스 상태 변수
    public float MoveSpeed; // 이동속도
    public LivingEntity[] players;//플레이어들 
    public GameObject[] Players;//플레이어들 
    public LivingEntity target; // 공격대상
    public Vector3 targetPos; // 공격 대상 위치 
    public GameObject StunEffect; //스턴
    public Transform Stuntrans; //스턴 이펙트가 나올 트랜스폼 변수

    public GameObject BossUI;
    public BossHpBar HpUI;
    public BossHpBar DiffUI;

    private float timer; //타이머
    private int randState;

    private NavMeshAgent nav; // NavMesh 컴포넌트
    private PhotonView pv;
    private Rigidbody rigid;
    private Animator anim; // 애니메이터 컴포넌트                         
    public BossGun bGun; //보스 총 컴포넌트


    [Header("전투 속성")]
    private Damage nDamage; //노말 데미지
    private Damage sDamage; // 스나이핑 데미지
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
        pv = GetComponent<PhotonView>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        // bGun = GetComponentInChildren<BossGun>(); 
        nav.updateRotation = false; // 네비의회전 기능 비활성화

        pv.ObservedComponents[0] = this;
        pv.Synchronization = ViewSynchronization.UnreliableOnChange;
    }

    protected override void OnEnable()
    {
        nDamage.dValue = 5f; //초기 데미지값 설정
        nDamage.dType = Damage.DamageType.Melee; //데미지 종류 설정

        sDamage.dValue = 10f;
        sDamage.dType = Damage.DamageType.Stun;
        sDamage.ccTime = 0.5f;

        bState = BossState.None;
        this.startingHealth = 300f; //테스트용 설정
        this.diff = 50f;

        base.OnEnable();
    }

    private void Start()
    {       
        HpUI.maxPoint = this.health;
        HpUI.currentPoint = this.health;

        DiffUI.maxPoint = this.diff;
        DiffUI.currentPoint = this.diff;
    }

    public EffectInfo[] Effects; // 이펙트 들
    
    [System.Serializable]
    public class EffectInfo
    {
        public GameObject Effecta;// 이펙트
        public Transform StartPositionRotation;
        public float DestroyAfter = 10; // 이펙트 지속시간
        public bool UseLocalPosition = true;
    }

    // 대쉬 이펙트 설정
    [PunRPC]
    void InstantiateEffect(int EffectNumber)
    {
        if (Effects == null || Effects.Length <= EffectNumber)
        {
            Debug.LogError("Incorrect effect number or effect is null");
        }

        var instance = Instantiate(Effects[EffectNumber].Effecta, Effects[EffectNumber].StartPositionRotation.position, Effects[EffectNumber].StartPositionRotation.rotation);

        if (Effects[EffectNumber].UseLocalPosition)
        {
            instance.transform.parent = Effects[EffectNumber].StartPositionRotation.transform;
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = new Quaternion();
        }
        Destroy(instance, Effects[EffectNumber].DestroyAfter);
    }


    [PunRPC]
    public void Init(float _nDamage, float _sDamage, float _speed, float _diff, float _startHealth = 50f) //초기 설정 메소드
    {
        nav.speed = _speed; //이동속도 설정
        nDamage.dValue = _nDamage; //초기 데미지값 설정
        nDamage.dType = Damage.DamageType.Melee; //데미지 종류 설정

        sDamage.dValue = _sDamage;
        sDamage.dType = Damage.DamageType.NuckBack;
        sDamage.ccTime = 0.5f;

        diff = _diff; //방어도 설정

        this.startingHealth = _startHealth; //초기 HP값 설정
    }

    void ChangeTarget()
    {
        if (players[0].Equals(target) && !players[1].dead)
        {
            target = players[1];
        }
        else if (players[1].Equals(target) && !players[0].dead)
        {
            target = players[0];
        }
        timer = 0;
    }

    private void FixedUpdate()
    {
        if(target != null)
        {
            targetPos = target.gameObject.transform.position;
            timer += Time.deltaTime;
        }
      
    }

    private void Update()
    {     
        HpUI.RefreshUI(this.health);
        DiffUI.RefreshUI(this.diff);

        if (!PhotonNetwork.IsMasterClient)
        { return; }


        else if (target != null && timer >= 10f && target.dead)
        { ChangeTarget(); }

        if (target != null)
        { sectorCheck(); }

        if (Input.GetKeyDown(KeyCode.Q))
        {

            //StartCoroutine(NormalAttack());
            //anim.SetTrigger("Shoot");
            //StartCoroutine(NormalAttack());
           // StartCoroutine(CreateBomobRobot());

            // StartCoroutine(SnipingShot());
            // StartCoroutine(Dash());
            // StartCoroutine(BackDash());
            //StartCoroutine(Enable());
            // StartCoroutine(Stun());
        }
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    TestPlayers = GameObject.FindGameObjectsWithTag("Player");
        //}
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    players[0] = TestPlayers[0].GetComponent<LivingEntity>();
        //    players[1] = TestPlayers[1].GetComponent<LivingEntity>();
        //    target = players[0];
        //}


        CheckState();
    }

    public void startRoutine()
    {
        players[0] = GameManager.instance.serverP.GetComponent<LivingEntity>();
        // players[1] = GameManager.instance.clientP.GetComponent<LivingEntity>();
        target = players[0];
        StartCoroutine(Enable());
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

        Vector3 lookPosition = Vector3.zero;
        bState = BossState.MoveTarget;
        lookPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);
        nav.isStopped = false;
        nav.SetDestination(lookPosition);
        transform.LookAt(lookPosition);

        float dist = Vector3.Distance(lookPosition, transform.position);

        if (dist <= attackRange)
        {

            nav.isStopped = true;
            nav.velocity = Vector3.zero;

            bState = BossState.NormalAttack;
            StartCoroutine(NormalAttack());
        }
    }

    [PunRPC]
    void ShowAnimation(int state)
    {
        switch(state)
        {
            case (int)BossState.None:
                anim.SetTrigger("Enable");
                break;
            case (int)BossState.MoveTarget:
                break;
            case (int)BossState.NormalAttack:
                anim.SetTrigger("Shoot");
                break;
            case (int)BossState.SpawnRobot:
                anim.SetTrigger("Spawn");
                break;
            case (int)BossState.AmimingShot:
                anim.SetTrigger("Sniping");            
                break;
            case (int)BossState.BackDash:
                anim.SetTrigger("BackDash");
                break;

            case (int)BossState.Dash:
                anim.SetTrigger("Dash");
                break;

            case (int)BossState.Stun:
                anim.SetTrigger("isStun");
                var stun = Instantiate(StunEffect, Stuntrans.position, Stuntrans.rotation);
                Destroy(stun, 3f);
                break;

            case (int)BossState.Die:
                anim.SetTrigger("isDead"); // 트리거 활성화
                break;

            case 9:
                anim.SetTrigger("SnipingShoot");
                break;

            case 10:
                anim.SetTrigger("SnipingEnd");
                break;

        }
    }


    IEnumerator Enable() //처음 실행되는 모션
    {
        yield return new WaitForSeconds(0.5f);


        pv.RPC("ShowAnimation", RpcTarget.All, (int)BossState.None);

        BossUI.SetActive(true);

        yield return new WaitForSeconds(0.3f);
        SoundManager.instance.SFXPlay(clips[1], this.gameObject);

        yield return new WaitForSeconds(6.8f);

        StartCoroutine(Dash());
       
    }

    IEnumerator Think() // 패턴설정
    {     
        yield return new WaitForSeconds(0.2f);
 

        randState = Random.Range(0, 7);

        if(direction.magnitude <= attackRange)
        {
            switch (randState)
            {
                case 0:
                case 1:
                case 3:
                case 4:
                
                case 5:
                    if (!WallCheck())
                    { StartCoroutine(BackDash()); }
                    else
                    { StartCoroutine(NormalAttack()); }
                    break;
                case 2:           
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
        bState = BossState.NormalAttack;
        nav.isStopped = true;

        for (int i = 0; i< 3; i++)
        {
            transform.LookAt(targetPos);
            pv.RPC("ShowAnimation", RpcTarget.All, (int)BossState.NormalAttack);
            SoundManager.instance.SFXPlay(clips[0], this.gameObject);
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.1f);
        StartCoroutine(Think());
    }

    IEnumerator CreateBomobRobot()
    { 
        bState = BossState.SpawnRobot;

        pv.RPC("ShowAnimation", RpcTarget.All, (int)BossState.SpawnRobot);

        for (int i= 0; i< 2; i++)
        {        
            Vector3 spawnPos = Random.insideUnitCircle * 4f; ;
            spawnPos.x += this.transform.position.x;
            spawnPos.z = spawnPos.y + this.transform.position.z;
            spawnPos.y = this.transform.position.y;
            SpawnRobot(spawnPos);
           //pv.RPC("SpawnRobot", RpcTarget., spawnPos);
            SpawnRobot(spawnPos);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Think());
    }
  
    [PunRPC]
    void SpawnRobot(Vector3 spawnPos)//로봇소환
    {
        BombRobotControl BombRobot = PhotonNetwork.Instantiate("BombRobot", spawnPos, Quaternion.identity).GetComponent<BombRobotControl>(); //ObjectPool.GetRobot();
       // BombRobot.transform.position = spawnPos;
        //BombRobot.gameObject.SetActive(true);
        if (PhotonNetwork.IsMasterClient)
        { BombRobot.SetTarget(target.gameObject); }
       
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

        nav.isStopped = true;
        pv.RPC("ShowAnimation", RpcTarget.All, (int)BossState.AmimingShot);
        yield return new WaitForSeconds(2.3f);


        for (int i=0; i < 3; i++)
        {
            pv.RPC("DangerMaskerShoot", RpcTarget.All, targetPos);
          
            yield return new WaitForSeconds(0.1f);   
            transform.LookAt(targetPos);
            pv.RPC("ShowAnimation", RpcTarget.All, 9); //sinping shoot;

            yield return new WaitForSeconds(0.3f); 
            SoundManager.instance.SFXPlay(clips[1], this.gameObject);
            yield return new WaitForSeconds(2f);
        }


        yield return new WaitForSeconds(0.5f);
        pv.RPC("ShowAnimation", RpcTarget.All, 10);//sniping end

        yield return new WaitForSeconds(0.1f);
        StartCoroutine(Think());
    } //스나이핑 샷

    [PunRPC]
    void DangerMaskerShoot(Vector3 endPos)
    {
        DangerLine line = ObjectPool.GetLine();
        Vector3 pos = this.transform.position;
        pos.y += 0.5f;
        line.transform.position = pos;
        line.EndPosition = endPos;      
    }

    void SShot()
    {
        bGun.StartFiring(sDamage);
    }

    void NShot()
    {
        bGun.StartFiring(nDamage);
    }

    IEnumerator Dash() //대쉬
    {
        bState = BossState.Dash;

        float startTime = Time.time;
        Vector3 lookPosition = Vector3.zero;
        pv.RPC("InstantiateEffect", RpcTarget.All, 0); // 이펙트를 전체로 뿌림
        //InstantiateEffect(0);
        //Time.time < startTime + dashTime
        while (!isCollision)
        {
            lookPosition = new Vector3(targetPos.x, this.transform.position.y, targetPos.z);
            nav.SetDestination(lookPosition);
            nav.speed = dashSpeed;
            nav.isStopped = false;
            nav.acceleration = dashSpeed;           
            transform.LookAt(lookPosition);
            pv.RPC("ShowAnimation", RpcTarget.All, (int)BossState.Dash);

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
            pv.RPC("ShowAnimation", RpcTarget.All, (int)BossState.BackDash);
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

        health -= dInfo.dValue; // 체력 감소

        diff -= 6;  /* (int)((dInfo.dValue * dInfo.inCapValue) / 100)*/ //총공격력에서 무력화수치 퍼센트만큼 방어도 감소

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
            Die();
        }
        
    }

    IEnumerator Stun()
    {
        bState = BossState.Stun;

        pv.RPC("ShowAnimation", RpcTarget.All, (int)BossState.Stun);
     
        yield return new WaitForSeconds(3f);

        diff = 50f;

        StartCoroutine(Think());
    }

    public override void Die()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("Die", RpcTarget.Others);
        }

        base.Die();
        StartCoroutine(Death());
    }

    //죽었을때
    public IEnumerator Death()
    {
        rigid.isKinematic = true;
        ShowAnimation((int)BossState.Die);
       

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


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && PhotonNetwork.IsMasterClient)
        { rigid.isKinematic = true; }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && PhotonNetwork.IsMasterClient)
        { rigid.isKinematic = false; }
    }

    private void OnDrawGizmos() // 범위 그리기
    {
        //Handles.color = isCollision ? red : blue;
        //Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angleRange / 2, attackRange);
        //Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angleRange / 2, attackRange);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(diff);
        }
        else
        {
            health = (float)stream.ReceiveNext();
            diff = (float)stream.ReceiveNext();
        }
    }
}
