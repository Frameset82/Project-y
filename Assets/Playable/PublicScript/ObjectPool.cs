using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private static ObjectPool Instance;

    [SerializeField]
    private GameObject BombRobot; // 자폭 로봇 프리팹

    private Queue<BombRobotControl> RobotQueue = new Queue<BombRobotControl>();


    void Start()
    {
        Instance = this;
        Initialize(20);


    }

    void Initialize(int count) // 초기 설정
    {
        for (int i = 0; i < count; i++)
        {
            RobotQueue.Enqueue(CreateNewObject());
        }
    }

    private BombRobotControl CreateNewObject() // 로봇생성 스크립트
    {
        var newObj = Instantiate(BombRobot, transform).GetComponent<BombRobotControl>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    public static BombRobotControl GetRobot() //로봇 가져가기
    {
        if(Instance.RobotQueue.Count > 0) //0보다 많으면 큐에서 꺼내주기
        {
            var obj = Instance.RobotQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }

        else // 0보다 적으면 새로 생성하기
        {
            var newObj = Instance.CreateNewObject(); 
            newObj.transform.SetParent(null);
            newObj.gameObject.SetActive(false);
            return newObj;
        }
    }


    public static void ReturnBombRobot(BombRobotControl bombr) //로봇 반환
    {
        bombr.gameObject.SetActive(false); //오브젝트 비활성화
        bombr.transform.SetParent(Instance.transform); //오브젝트 풀의 자식으로 설정
        Instance.RobotQueue.Enqueue(bombr); //다시 큐에 넣기
    }
}
