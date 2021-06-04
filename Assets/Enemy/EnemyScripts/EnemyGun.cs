using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyGun : MonoBehaviour
{
    public Transform fireTransform; // 총알 생성 위치
    public GameObject muzzleFlashEffect; // 총구 화염 효과
    EnemyBullet bullet;

  

    private void Awake()
    {
        muzzleFlashEffect.SetActive(false);
        //bulletLineRenderer = GetComponent<LineRenderer>();
        //bulletLineRenderer.positionCount = 2;
        //bulletLineRenderer.enabled = false;
    }


    //public void Fire(Damage damage, float fireDistance)
    //{
    //    RaycastHit hit; //레이캐스트 
    //    Vector3 hitPosition = Vector3.zero; //레이를 쏠 방향

    //    if(Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
    //    {
    //        LivingEntity attackTarget = hit.collider.GetComponent<LivingEntity>();

    //        damage.hitPoint = hit.point;
    //        damage.hitNormal = hit.normal;

    //        if(attackTarget != null)
    //        {
    //            attackTarget.OnDamage(damage);
    //            hitPosition = hit.point;
    //        }
    //        else
    //        {
    //            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
    //        }

    //        StartCoroutine(ShotEffect(hitPosition));
    //    }
    //}

    public void Fire(Damage damage, float fireDistance)
    {
        StartCoroutine(ShotEffect());
       
        bullet = ObjectPool.GetBullet();
      
        bullet.damage = damage;

        bullet.transform.position = fireTransform.position;
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0f, fireTransform.eulerAngles.y, fireTransform.eulerAngles.z));
        bullet.gameObject.SetActive(true);
    }

    protected IEnumerator ShotEffect()
    {
        muzzleFlashEffect.SetActive(true);
        //bulletLineRenderer.SetPosition(0, fireTransform.position);
        //bulletLineRenderer.SetPosition(1, hitPoint);
        //bulletLineRenderer.enabled = true;

        yield return new WaitForSeconds(0.1f);
        muzzleFlashEffect.SetActive(false);
        //bulletLineRenderer.enabled = false;
    }
}
