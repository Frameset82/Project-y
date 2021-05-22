using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGun : MonoBehaviour
{
    public Transform fireTransform; // 총알 생성 위치
    public ParticleSystem muzzleFlash; // 총구 화염 효과
    public bool isFiring = false;
    EnemyBullet bullet; 

    public void StartFiring(Damage _damage)
    {
        isFiring = true;
        muzzleFlash.Emit(1);
       
        switch(_damage.dType)
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
        bullet.transform.rotation = fireTransform.rotation;
    }




 
}
