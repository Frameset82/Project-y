using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boom : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;
    private float time;

    void Start()
    {
        StartCoroutine(Explosion());
    }



    IEnumerator Explosion()
    { 
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero; //rigidbody 속도 
        rigid.angularVelocity = Vector3.zero; //오브젝트가 회전하는 속도
        meshObj.SetActive(false);
        effectObj.SetActive(true);
      
    }

   
}
