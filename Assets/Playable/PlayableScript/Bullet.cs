using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float time;
    public static GameObject rangeHitTarget;
    public PlayerEquipmentManager playerEquipmentManager;

    private void OnEnable() // 활성화시  초기화
    {
        time = 0f; // 시간을 0으로 초기화
    }

    void FixedUpdate()
    {
        transform.Translate(this.transform.forward * -1f);

        if(time > 0.5f)
        {
            BulletObjectPool.ReturnBullet(this);
        }

        time += Time.deltaTime;
        //Destroy(gameObject, 1.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            rangeHitTarget = other.gameObject;
            playerEquipmentManager.equipWeapon.GetComponent<Weapon>().OnAttack();
        }
    }
}