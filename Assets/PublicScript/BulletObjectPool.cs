using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObjectPool : MonoBehaviour
{

    private static BulletObjectPool Instance; // 싱글톤을 위한 static 변수
    public GameObject BulletObject; // 총알 원본 프리팹
    private float time;
    private Queue<Bullet> BulletQueue = new Queue<Bullet>();
    // 생성한 총알이 들어갈 큐



    void Start()
    {
        Instance = this; // 자기자신 싱글톤화
        Initialize(50); // 총알 얼마만큼 생성할지 지정
    }

    // 총알 오브젝트 생성후 비활성화 시켜놓기
    private Bullet CreateNewObject()
    {
        var newObj = Instantiate(BulletObject, transform).GetComponent<Bullet>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    // 큐에다가 생성한 총알 넣어주기
    void Initialize(int count)
    {
        for (int i = 0; i < count; i++)
        {
            BulletQueue.Enqueue(CreateNewObject());
        }
      
    }

    // 오브젝트 풀 큐에서 불릿 꺼내오기
    public static Bullet GetBullet()
    {
        // 큐에 있는 총알 갯수가 0보다 클경우 큐에서 총알 꺼내줌
        if(Instance.BulletQueue.Count > 0)
        {
            var obj = Instance.BulletQueue.Dequeue();
            obj.transform.SetParent(null); 
            obj.gameObject.SetActive(true);
            return obj; 
        }
        else //큐에 있는 총알 갯수가 0보다 적으면 새로 생성해서 줌
        {
            var newObj = Instance.CreateNewObject();
            newObj.transform.SetParent(null);
            newObj.gameObject.SetActive(false);
            return newObj;
        }
    }

    //꺼내줬던 총알 다시 받아오기
    public static void ReturnBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(Instance.transform);
        Instance.BulletQueue.Enqueue(bullet);
    }

   

}
