using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerController : LivingEntity
{
   
    private Animator animator; //애니메이터
    [Header("체력설정")]
    public float sHealth = 50f; // 초기 체력
    Damage damage;
    Damage damage1;

    //public GameObject target;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>(); //애니메이터 가져오기
        OnEnable(); // 초기 설정하기
    }

    protected override void OnEnable()
    {
        startingHealth = sHealth;     
        base.OnEnable();
        damage.dType = Damage.DamageType.NuckBack;
        damage.dValue = 10f;
        damage.ccTime = 3f;

        damage1.dType = Damage.DamageType.Melee;
        damage1.dValue = 10f;
        damage1.ccTime = 3f;
    }

    private void Update()
    {
        if (health <= 0) // health가 0이하일경우 추가 입력 방지
            return;

        if (Input.GetMouseButtonDown(0)) //마우스클릭시 데미지 입히기(테스트용)
        {
            //OnDamage(10f);
            //LivingEntity enemytarget = target.GetComponent<LivingEntity>();

            //Vector3 hitPoint = target.GetComponent<Collider>().ClosestPoint(transform.position);

            //Vector3 hitNormal = transform.position - target.transform.position;


           // Rigidbody rigid = enemytarget.GetComponent<Rigidbody>();
            //hitNormal = hitNormal.normalized;
            //hitNormal.y = 1;
            //enemytarget.OnDamage(damage1);
            //rigid.AddForce(hitNormal * 20f * -1f, ForceMode.Impulse);
            // Debug.Log(Damage.DamageType.Melee);

        }
    }

    public override void OnDamage(Damage dInfo)  // 데미지를 입는 기능
    {
      
        //base.OnDamage(damage, hitPoint, hitNormal);
        animator.SetTrigger("IsHit");
 
        // 체력이 0 이하 && 아직 죽지 않았다면 사망 처리 실행
      
        
    }

    public override void Die()
    {
        dead = true;
        StartCoroutine(Respone());
        
    }

    // 부활 코루틴
    IEnumerator Respone()
    {
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("IsDead"); 
        yield return new WaitForSeconds(3f);
        OnEnable(); //초기화
        animator.SetTrigger("Restore");
    }

}
