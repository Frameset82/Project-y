using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed; //총알 속도
    public GameObject hitPrefab; //타격 프리팹
    public List<GameObject> trails; //트레일 
    private Rigidbody rigid;
    private float time;
    public Damage damage; //데미지

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();

    }

    private void OnEnable()
    {
        time = 0f; // 시간을 0으로 초기화
        rigid.velocity = this.transform.forward * speed;
    }


    void Update()
    {
        if (time > 4f)
        {
            ReturnToPool();
        }

        time += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                LivingEntity enemytarget = other.gameObject.GetComponent<LivingEntity>();
                enemytarget.OnDamage(damage);
            }

            if (trails.Count > 0)
            {
                for (int i = 0; i < trails.Count; i++)
                {
                    trails[i].transform.parent = null;
                    var ps = trails[i].GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        ps.Stop();
                        Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                    }
                }
            }

         
            
            Vector3 pos = other.GetComponent<Collider>().bounds.ClosestPoint(transform.position);
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, pos.normalized);

            if (hitPrefab != null)
            {
                var hitVFX = Instantiate(hitPrefab, pos, rot);
                var ps = hitVFX.GetComponent<ParticleSystem>();
                if (ps == null)
                {
                    var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                    Destroy(hitVFX, psChild.main.duration);
                }
                else
                    Destroy(hitVFX, ps.main.duration);
            }

            StartCoroutine(DestroyParticle(0f));
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.layer == 8)
    //    {
    //        LivingEntity enemytarget = collision.gameObject.GetComponent<LivingEntity>();
    //        enemytarget.OnDamage(damage);
     

    //        if (trails.Count > 0)
    //        {
    //            for (int i = 0; i < trails.Count; i++)
    //            {
    //                trails[i].transform.parent = null;
    //                var ps = trails[i].GetComponent<ParticleSystem>();
    //                if (ps != null)
    //                {
    //                    ps.Stop();
    //                    Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
    //                }
    //            }
    //        }

    //        ContactPoint contact = collision.contacts[0];
    //        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
    //        Vector3 pos = contact.point;

    //        if (hitPrefab != null)
    //        {
    //            var hitVFX = Instantiate(hitPrefab, pos, rot);
    //            var ps = hitVFX.GetComponent<ParticleSystem>();
    //            if (ps == null)
    //            {
    //                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
    //                Destroy(hitVFX, psChild.main.duration);
    //            }
    //            else
    //                Destroy(hitVFX, ps.main.duration);
    //        }

    //        StartCoroutine(DestroyParticle(0f));
    //    }
    //}

    public IEnumerator DestroyParticle(float waitTime)
    {

        if (transform.childCount > 0 && waitTime != 0)
        {
            List<Transform> tList = new List<Transform>();

            foreach (Transform t in transform.GetChild(0).transform)
            {
                tList.Add(t);
            }

            while (transform.GetChild(0).localScale.x > 0)
            {
                yield return new WaitForSeconds(0.01f);
                transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                for (int i = 0; i < tList.Count; i++)
                {
                    tList[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }

        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }

    private void ReturnToPool() //탄환 반납
    {
        switch (damage.dType)
        {
            case Damage.DamageType.Melee:
                ObjectPool.ReturnBullet(this);
                break;
            case Damage.DamageType.Stun:
                ObjectPool.ReturnSBullet(this);
                break;
        }
    }
}
