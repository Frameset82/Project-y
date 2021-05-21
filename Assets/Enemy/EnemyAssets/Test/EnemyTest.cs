using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTest : LivingEntity
{
    public float Speed = 50.0f;
    private float timer =2.0f;
    private Transform myTransform = null; // 트랜스폼
    //폭발 Object
    public GameObject Explosion = null; // 폭팔
    private bool isEnter = true; // 한번만 실행 시키기 위한 bool 함수
 


    public int maxHealth;
    public int curHealth;
    public Transform target;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    NavMeshAgent nav;
    Animator anim;
    public bool isChase;


    void Start()
    {
        myTransform = GetComponent<Transform>();
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
/*
        Invoke("ChaseStart", 2);*/
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    void FixedUpdate()
    {
        FreezeVelocity();
    }

    /*    void ChaseStart()
        {

            isChase = true;
            anim.SetBool("isRun", true);
        }
    */

    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player" && isEnter)
        {
            
            Debug.Log("Trigger Enter");
            //폭발 Object 생성

            StartCoroutine(Explode());

        }
    }
    private IEnumerator Explode()
    {
            isEnter = false; // 한번만 실행 시키기 위해 isEnter

        anim.SetBool("isWalk", false); 
        yield return new WaitForSeconds(2.0f); // 2초간 정지후 실행
        Instantiate(Explosion, myTransform.position, Quaternion.identity);
        Destroy(gameObject);
        isEnter = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isChase)
        nav.SetDestination(target.position);
    }
}

