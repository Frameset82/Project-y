using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGun : MonoBehaviour
{
    public Transform fireTransform; // 총알 생성 위치
    public ParticleSystem muzzleFlash; // 총구 화염 효과
    public GameObject Flash;
    public bool isFiring = false;
    EnemyBullet bullet; 

    public void StartFiring(Damage _damage)
    {
        isFiring = true;
       
        StartCoroutine(EffectRoutine());
        switch (_damage.dType)
        {
            case Damage.DamageType.Melee:
                bullet = ObjectPool.GetBullet();
                break;
            case Damage.DamageType.Stun:
                bullet = ObjectPool.GetSBullet();
                break;
        }

        bullet.damage = _damage;
       
        bullet.transform.position = fireTransform.position;
        bullet.transform.rotation = Quaternion.Euler( new Vector3(0f, fireTransform.eulerAngles.y, fireTransform.eulerAngles.z));
        bullet.gameObject.SetActive(true);
       
    }

    IEnumerator EffectRoutine()
    {
        Flash.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Flash.SetActive(false);
    }



 
}
