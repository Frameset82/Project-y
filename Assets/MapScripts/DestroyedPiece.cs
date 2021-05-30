using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedPiece : MonoBehaviour
{
    DestroyableObj parent;
    public float power = 100f;
    void Start() {
        parent = transform.parent.GetComponent<DestroyableObj>();
        parent.breakDelegate += BreakObject;
    }

    void BreakObject(Damage dType) {
        GetComponent<Rigidbody>().isKinematic = false;
        // GetComponent<Rigidbody>().AddForce((transform.position - attackerPosition).normalized * power);
        Invoke("DeleteObj", 3f);
    }
    
    void DeleteObj(){
        Destroy(gameObject);
    }
}
