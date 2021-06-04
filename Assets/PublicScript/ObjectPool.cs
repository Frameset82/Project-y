using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
 
    private static ObjectPool Instance;

    [SerializeField]
    private GameObject BombRobot; // 자폭 로봇 프리팹
    [SerializeField]
    private GameObject MeeleEnemy; // 근접적 프리팹
    [SerializeField]
    private GameObject RogueEnemy; // 로그적 프리팹
    [SerializeField]
    private GameObject RifeEnemy; // 라이플적 프리팹
    [SerializeField]
    private GameObject Bullet; //일반 총알
    [SerializeField] 
    private GameObject SBullet; //스나이핑 총알
    [SerializeField]
    private GameObject DangerLine; //공격 범위 라인
    [SerializeField]
    private GameObject Potal; //포탈 범위 라인

    private Queue<BombRobotControl> RobotQueue = new Queue<BombRobotControl>(); //자폭로봇 큐
    private Queue<MeleeController> MeleeQueue = new Queue<MeleeController>();  //근접적 큐
    private Queue<RogueController> RogueQueue = new Queue<RogueController>();  // 로그적 큐
    private Queue<RifleController> RifleQueue = new Queue<RifleController>();  // 라이플적 큐
    private Queue<EnemyBullet> BulletQueue = new Queue<EnemyBullet>();  //일반총알 
    private Queue<EnemyBullet> SBulletQueue = new Queue<EnemyBullet>();  //스나이핑 총알
    private Queue<DangerLine> DLineQueue = new Queue<DangerLine>(); 
    private Queue<GameObject> PotalQueue = new Queue<GameObject>(); 

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
            BulletQueue.Enqueue(CreateNewBullet());
            DLineQueue.Enqueue(CreateNewLine());
            SBulletQueue.Enqueue(CreateNewSBullet());
            MeleeQueue.Enqueue(CreateNewMelee());
            RogueQueue.Enqueue(CreateNewRogue());
            RifleQueue.Enqueue(CreateNewRifle());
            PotalQueue.Enqueue(CreatePotal());
        }
    }

    private GameObject CreatePotal()
    {
        GameObject potal = Instantiate(Potal, transform);
        potal.gameObject.SetActive(false);
        return potal;
    }

    private RifleController CreateNewRifle() //라이플 적 생성
    {
        var newObj = Instantiate(RifeEnemy, transform).GetComponent<RifleController>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    private DangerLine CreateNewLine()
    {

        var newObj = Instantiate(DangerLine, transform).GetComponent<DangerLine>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    private RogueController CreateNewRogue() // 로그적생성 
    {
        var newObj = Instantiate(RogueEnemy, transform).GetComponent<RogueController>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }


    private BombRobotControl CreateNewBomb() // 로봇생성 
    {
        var newObj = Instantiate(BombRobot, transform).GetComponent<BombRobotControl>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    private EnemyBullet CreateNewBullet() //총알 생성
    {
        var newBullet = Instantiate(Bullet, transform).GetComponent<EnemyBullet>();
        newBullet.gameObject.SetActive(false);
        return newBullet;
    }

    private EnemyBullet CreateNewSBullet() //총알 생성
    {
        var newBullet = Instantiate(SBullet, transform).GetComponent<EnemyBullet>();
        newBullet.gameObject.SetActive(false);
        return newBullet;
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


    public static DangerLine GetLine() //공격범위가져가기
    {

        if (Instance.DLineQueue.Count > 0) //0보다 많으면 큐에서 꺼내주기
        {
            var obj = Instance.DLineQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }

        else // 0보다 적으면 새로 생성하기
        {
            var newObj = Instance.CreateNewLine();
            newObj.transform.SetParent(null);
            newObj.gameObject.SetActive(false);
            return newObj;
        }
    }

    public static void ReturnLine(DangerLine line) //공격범위 반환
    {
        line.gameObject.SetActive(false);
        line.transform.SetParent(Instance.transform);
        Instance.DLineQueue.Enqueue(line);
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

    public static RogueController GetRogue() //로그적 가져가기
    {
        if (Instance.RogueQueue.Count > 0) //0보다 많으면 큐에서 꺼내주기
        {
            var obj = Instance.RogueQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }

        else // 0보다 적으면 새로 생성하기
        {
            var newObj = Instance.CreateNewRogue();
            newObj.transform.SetParent(null);
            newObj.gameObject.SetActive(false);
            return newObj;
        }
    }

    public static void ReturnRogue(RogueController mbr) //로그적 반환
    {
        mbr.gameObject.SetActive(false); //오브젝트 비활성화
        mbr.transform.SetParent(Instance.transform); //오브젝트 풀의 자식으로 설정
        Instance.RogueQueue.Enqueue(mbr); //다시 큐에 넣기
    }

    public static RifleController GetRifle() //라이플적 가져가기
    {
        if (Instance.RifleQueue.Count > 0) //0보다 많으면 큐에서 꺼내주기
        {
            var obj = Instance.RifleQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }

        else // 0보다 적으면 새로 생성하기
        {
            var newObj = Instance.CreateNewRifle();
            newObj.transform.SetParent(null);
            newObj.gameObject.SetActive(false);
            return newObj;
        }
    }

    public static void ReturnRifle(RifleController mbr) //라이플적 반환
    {
        mbr.gameObject.SetActive(false); //오브젝트 비활성화
        mbr.transform.SetParent(Instance.transform); //오브젝트 풀의 자식으로 설정
        Instance.RifleQueue.Enqueue(mbr); //다시 큐에 넣기
    }


    public static EnemyBullet GetBullet() //총알 가져가기
    {
        if (Instance.BulletQueue.Count > 0) //0보다 많으면 큐에서 꺼내주기
        {
            var obj = Instance.BulletQueue.Dequeue();
            obj.transform.SetParent(null);
            //obj.gameObject.SetActive(true);
            return obj;
        }

        else // 0보다 적으면 새로 생성하기
        {
            var newObj = Instance.CreateNewBullet();
            newObj.transform.SetParent(null);
            //newObj.gameObject.SetActive(false);
            return newObj;
        }
    }

    public static EnemyBullet GetSBullet() //총알 가져가기
    {
        if (Instance.SBulletQueue.Count > 0) //0보다 많으면 큐에서 꺼내주기
        {
            var obj = Instance.SBulletQueue.Dequeue();
            obj.transform.SetParent(null);
            //obj.gameObject.SetActive(true);
            return obj;
        }

        else // 0보다 적으면 새로 생성하기
        {
            var newObj = Instance.CreateNewSBullet();
            newObj.transform.SetParent(null);
            //newObj.gameObject.SetActive(false);
            return newObj;
        }
    }

    public static void ReturnBullet(EnemyBullet bl) //총알 반환
    {
        bl.gameObject.SetActive(false); //오브젝트 비활성화
        bl.transform.SetParent(Instance.transform); //오브젝트 풀의 자식으로 설정
        Instance.BulletQueue.Enqueue(bl); //다시 큐에 넣기
    }

    public static void ReturnSBullet(EnemyBullet bl) //총알 반환
    {
        bl.gameObject.SetActive(false); //오브젝트 비활성화
        bl.transform.SetParent(Instance.transform); //오브젝트 풀의 자식으로 설정
        Instance.SBulletQueue.Enqueue(bl); //다시 큐에 넣기
    }

    
}
