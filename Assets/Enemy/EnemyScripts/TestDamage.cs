using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TestDamage : MonoBehaviour
{

    Damage damage;
    Damage ndamage;
    Damage sdamage;
    public GameObject[] target;


    private void OnEnable()
    {
        damage.dType = Damage.DamageType.Melee;
        damage.dValue = 10f;
        damage.ccTime = 1f;

        ndamage.dType = Damage.DamageType.NuckBack;
        ndamage.dValue = 1f;
        ndamage.ccTime = 1f;


        sdamage.dType = Damage.DamageType.Stun;
        sdamage.dValue = 1f;
        sdamage.ccTime = 10f;
    }

    private void Start()
    {
        target = GameObject.FindGameObjectsWithTag("Enemy");
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            for (int i = 0; i < target.Length; i++)
            {
                LivingEntity enemytarget = target[i].GetComponent<LivingEntity>();
                enemytarget.OnDamage(damage);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            for (int i = 0; i < target.Length; i++)
            {
                LivingEntity enemytarget = target[i].GetComponent<LivingEntity>();
                enemytarget.OnDamage(ndamage);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            for (int i = 0; i < target.Length; i++)
            {
                LivingEntity enemytarget = target[i].GetComponent<LivingEntity>();
                enemytarget.OnDamage(sdamage);
            }
        }
    }

}
