using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Fireball : MonoBehaviour {
    public bool pushOnAwake = true;
    public Vector3 startDirection;
    public float startMagnitude;
    public ForceMode forceMode;

    public GameObject fieryEffect;
    public GameObject smokeEffect;
    public GameObject explodeEffect;

    protected Rigidbody rgbd;
    private Damage damage;
    private float timer;

    public void Awake()
    {
        rgbd = GetComponent<Rigidbody>();
        damage.dType = Damage.DamageType.Melee;
        damage.dValue = 10f;

    }

    public void Start()
    {
        timer += Time.deltaTime;

       if(timer > 3f)
        {
            TrapPool.ReturnFireBall(this);
        }
    }

    public void OnEnable()
    {
        if (fieryEffect != null)
            fieryEffect.SetActive(true);
        if (smokeEffect != null)
            smokeEffect.SetActive(true);
        if (explodeEffect != null)
            explodeEffect.SetActive(false);

        rgbd.velocity = this.transform.forward * 10f;
       // rgbd.AddForce(transform.forward * 5f);
        timer = 0f;
    }

    public void Push(Vector3 direction, float magnitude)
    {
        Vector3 dir = direction.normalized;
        rgbd.AddForce(dir * magnitude, forceMode);
    }



    private void OnTriggerEnter(Collider other)
    {
        PlayerInfo playerInfo;
        playerInfo = other.GetComponent<PlayerInfo>();

        if (other.gameObject.CompareTag("Player") && playerInfo.canDamage)
        {
            //LivingEntity livingEntity = col.gameObject.GetComponent<LivingEntity>();
            //livingEntity.OnDamage(damage);

            StartCoroutine(ExplosionRoutine());
        }
    }

    IEnumerator ExplosionRoutine()
    {

        rgbd.Sleep();
        if (fieryEffect != null)
        {
            StopParticleSystem(fieryEffect);
        }
        if (smokeEffect != null)
        {
            StopParticleSystem(smokeEffect);
        }
        if (explodeEffect != null)
            explodeEffect.SetActive(true);

        yield return new WaitForSeconds(1f);

        TrapPool.ReturnFireBall(this);
    }
 

    public void StopParticleSystem(GameObject g)
    {
        ParticleSystem[] par;
        par = g.GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem p in par)
        {
            p.Stop();
        }


    }

  
}


