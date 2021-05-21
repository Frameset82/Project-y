using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
 
    private static ObjectPool Instance;

    [SerializeField]
    private GameObject BombRobot; // 자폭 로봇 프리팹
    private GameObject MeeleEnemy; // 근접적 프리팹
    private GameObject RobueEnemy; // 로그적 프리팹
    private GameObject RifeEnemy; // 라이플적 프리팹

    private Queue<BombRobotControl> RobotQueue = new Queue<BombRobotControl>(); //자폭로봇 큐
    private Queue<MeleeController> MeleeQueue = new Queue<MeleeController>();  //근접적 큐
    private Queue<RogueController> RogueQueue = new Queue<RogueController>();  // 로그적 큐
    private Queue<RifleController> RifleQueue = new Queue<RifleController>();  // 라이플적 큐

    void Start()
    {
        Instance = this;
        Initialize(20);
    }

    void Initialize(int count) // 초기 설정
    {
        for (int i = 0; i < count; i++)
        {
            RobotQueue.Enqueue(CreateNewBomb());
           // MeleeQueue.Enqueue(CreateNewMelee());
            //RogueQueue.Enqueue(CreateNewRogue());
            //RifleQueue.Enqueue(CreateNewRifle());
        }
    }

    private RifleController CreateNewRifle() //라이플 적 생성
    {
        var newObj = Instantiate(RifeEnemy, transform).GetComponent<RifleController>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }


    private RogueController CreateNewRogue() // 로그적생성 
    {
        var newObj = Instantiate(RobueEnemy, transform).GetComponent<RogueController>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }


    private BombRobotControl CreateNewBomb() // 로봇생성 
    {
        var newObj = Instantiate(BombRobot, transform).GetComponent<BombRobotControl>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    private MeleeController CreateNewMelee() //근접적생성 
    {
        var newObj = Instantiate(MeeleEnemy, transform).GetComponent<MeleeController>();
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
            var newObj = Instance.CreateNewBomb(); 
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

    public static MeleeController GetMelee() //근접적 가져가기
    {
        if (Instance.MeleeQueue.Count > 0) //0보다 많으면 큐에서 꺼내주기
        {
            var obj = Instance.MeleeQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }

        else // 0보다 적으면 새로 생성하기
        {
            var newObj = Instance.CreateNewMelee();
            newObj.transform.SetParent(null);
            newObj.gameObject.SetActive(false);
            return newObj;
        }
    }

    public static void ReturnMeleeEnemy(MeleeController mbr) //근접적 반환
    {
        mbr.gameObject.SetActive(false); //오브젝트 비활성화
        mbr.transform.SetParent(Instance.transform); //오브젝트 풀의 자식으로 설정
        Instance.MeleeQueue.Enqueue(mbr); //다시 큐에 넣기
    }
}
