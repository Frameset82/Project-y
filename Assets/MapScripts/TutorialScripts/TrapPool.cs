using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPool : MonoBehaviour
{
    private static TrapPool Instance;

    public GameObject[] Traps;
    public GameObject FireBall; //파이어볼 프리팹
    private  Queue<Fireball> FireBallQueue = new Queue<Fireball>();

    private bool isActive;
    private float timer;

    void Start()
    {
        Instance = this;
        isActive = false;
        Initialize();
        timer = 2f;
    }


    void Update()
    {
        if(isActive)
        {
           SpawnFireBall();
       
        }
      
    }

    void SpawnFireBall()
    {

        timer += Time.deltaTime;

        if (timer >= 2f)
        {
            for (int i = 0; i < Traps.Length; i++)
            {
                Fireball fire = GetFireBall();
                fire.transform.position = Traps[i].transform.position;
                fire.transform.rotation = Quaternion.Euler(new Vector3(0f, Traps[i].transform.eulerAngles.y, Traps[i].transform.eulerAngles.z));
                fire.gameObject.SetActive(true);

            }
            timer = 0f;
        }
    }


    void Initialize() // 초기 설정
    {
        for (int i = 0; i < 30; i++)
        {
            FireBallQueue.Enqueue(CreateNewFireBall());
        }
    }


    private  Fireball CreateNewFireBall() // 화염구 생성
    {
        var newObj = Instantiate(FireBall, transform).GetComponent<Fireball>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }


    public static  Fireball GetFireBall() //화염구 가져가기
    {
        if (Instance.FireBallQueue.Count > 0) //0보다 많으면 큐에서 꺼내주기
        {
            var obj = Instance.FireBallQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }

        else // 0보다 적으면 새로 생성하기
        {
            var newObj = Instance.CreateNewFireBall();
            newObj.transform.SetParent(null);
            newObj.gameObject.SetActive(false);
            return newObj;
        }
    }

    public static void ReturnFireBall(Fireball fire) //총알 반환
    {
        fire.gameObject.SetActive(false); //오브젝트 비활성화
        fire.transform.SetParent(Instance.transform); //오브젝트 풀의 자식으로 설정
        Instance.FireBallQueue.Enqueue(fire); //다시 큐에 넣기
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && !isActive)
        {
            isActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("Player") && isActive)
        {
            isActive = false;
        }
    }
}
