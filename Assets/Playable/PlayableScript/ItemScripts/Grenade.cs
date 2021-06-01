using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    
    private Rigidbody rigid;
    public GameObject effectObj;
    private Damage damage;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        damage.dType = Damage.DamageType.NuckBack;
        damage.ccTime = 1f;
        damage.dValue = 20f;
    }

    private void Start()
    {
        StartCoroutine(Explosion());
    }



    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;


        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
            15f, Vector3.up, 0f, LayerMask.GetMask("Enemy"));

        foreach (RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<LivingEntity>().OnDamage(damage);
        }

        Instantiate(effectObj, this.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
