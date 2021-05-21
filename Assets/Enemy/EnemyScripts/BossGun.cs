using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGun : MonoBehaviour
{
    public Transform fireTransform; // 총알 생성 위치
    public ParticleSystem muzzleFlash; // 총구 화염 효과
    public bool isFiring = false;
    
    public void StartFiring(Damage _damage)
    {
        isFiring = true;
        muzzleFlash.Emit(1);
        var Bullet = ObjectPool.GetBullet();
        Bullet.damage = _damage;

        Bullet.transform.position = fireTransform.position;
        Bullet.transform.rotation = fireTransform.rotation;
    }


 
}
