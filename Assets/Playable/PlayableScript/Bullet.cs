using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float time;

    private void OnEnable() // 활성화시  초기화
    {
        time = 0f; // 시간을 0으로 초기화
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * 1f);

        if(time > 1f)
        {
            BulletObjectPool.ReturnBullet(this);
        }

        time += Time.deltaTime;
        //Destroy(gameObject, 1.0f);
    }
}