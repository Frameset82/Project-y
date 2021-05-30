using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyableObj : MonoBehaviour, IDamageable
{
    public delegate void BreakDelegate(Damage dType);
    public BreakDelegate breakDelegate;
    public void OnDamage(Damage dType) {
        OnBreak(dType);
    }
    public void OnBreak(Damage dType) {
        breakDelegate(dType);
        GetComponent<BoxCollider>().enabled = false;
    }
}

