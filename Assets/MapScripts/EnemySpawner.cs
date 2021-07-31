using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;



public class EnemySpawner : MonoBehaviour, IPunObservable
{
    public enum enemyType {None, Melee, Rogue, Rifle, End};

    public enemyType[] eTypes;
    private static List<LivingEntity> enemis = new List<LivingEntity>();
    private  List<Vector3> sPos = new List<Vector3>();

    public int wave;
    private bool isEnter;
    public Transform boxScale;
    public GameObject[] Walls;
    private PhotonView pv;
    private int count = 0;



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(wave);  //웨이브 동기화
            stream.SendNext(count);
        }
        else
        {
            wave = (int)stream.ReceiveNext();
            count = (int)stream.ReceiveNext();          
        }
    }

    void Start()
    {
        isEnter = false;
        pv = GetComponent<PhotonView>();
        wave = 0;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
           
        }

       /* if(enemis.Count <= 0 && PhotonNetwork.IsMasterClient)
        {
            pv.RPC("AfterWave", RpcTarget.All);
        }*/
    }

    [PunRPC]
    void AfterWave()
    {
        Walls[wave].SetActive(false); //현재 웨이브 벽 비활성화
        wave++; //웨이브 카운트
    }


    void SetSpawn() // 몹 스폰할 지점 설정
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPos;
            spawnPos.x = Random.Range((boxScale.position.x - boxScale.localScale.x / 2), (boxScale.position.x + boxScale.localScale.x / 2)); 
            spawnPos.z = Random.Range((boxScale.position.z - boxScale.localScale.z / 2), (boxScale.position.z + boxScale.localScale.z / 2));
            spawnPos.y = boxScale.position.y;
            sPos.Add(spawnPos);

            // pv.RPC("SpawnEnemy", RpcTarget.All, spawnPos, count);
          //  SpawnEnemy(spawnPos, count);
            pv.RPC("enemyActive", RpcTarget.Others, spawnPos, count);
            //SpawnEnemy(spawnPos);
            count++;
        }
      
    }

    //[PunRPC]
    //void SpawnEnemy(Vector3 spawnPos, int _count) //몹 스폰
    //{
    //   StartCoroutine(ShowRoutine(spawnPos));

    //    switch (eTypes[_count])
    //    { 
    //        case enemyType.Melee:
    //            MeleeController mcon = ObjectPool.GetMelee();       
    //            mcon.gameObject.transform.position = spawnPos; 
    //            enemis.Add(mcon);
    //            mcon.onDeath += () => enemis.Remove(mcon);
    //            break;

    //        case enemyType.Rifle:
    //            RogueController rcon = ObjectPool.GetRogue();
    //            rcon.gameObject.transform.position = spawnPos;          
    //            enemis.Add(rcon);
    //            rcon.onDeath += () => enemis.Remove(rcon);
    //            break;

    //        case enemyType.Rogue:
    //            RifleController ricon = ObjectPool.GetRifle();     
    //            ricon.gameObject.transform.position = spawnPos;
    //            enemis.Add(ricon);
    //            ricon.onDeath += () => enemis.Remove(ricon);
    //            break;            
    //    }
    
    //}


    [PunRPC]
    void enemyActive(Vector3 spawnPos, int _count)
    {
        enemis[_count].gameObject.SetActive(false);
    }




    IEnumerator ShowRoutine(Vector3 spawnPos)
    {    
          GameObject  potal = ObjectPool.GetPotal();
          potal.transform.position = spawnPos;
          potal.gameObject.SetActive(true);
        
          yield return new WaitForSeconds(6f);
      
          ObjectPool.ReturnPotal(potal);
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && PhotonNetwork.IsMasterClient && !isEnter) //플레이어와 충돌하고 마스터 클라이언트일 경우
        {         
            SetSpawn();
            isEnter = true;
        }
    }



}
