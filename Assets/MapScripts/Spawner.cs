using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviour, IPunObservable
{
    public enum enemyType { None, Melee, Rogue, Rifle, End };

    public enemyType[] eTypes;
    private List<LivingEntity> enemis = new List<LivingEntity>();
    private List<Vector3> sPos = new List<Vector3>();

    public int wave;
    private bool isEnter;
    private bool isSpanw;
    public Transform boxScale;
    public GameObject[] Walls;
    private PhotonView pv;
    


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(wave);  //웨이브 동기화
        }

        else
        {
            wave = (int)stream.ReceiveNext();            
        }
    }


    void Start()
    {
        isSpanw = false;
        isEnter = false;
        pv = GetComponent<PhotonView>();
        wave = 0;
    }

    private void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if(enemis.Count <= 0 && isSpanw)
            {
                pv.RPC("AfterWave", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void AfterWave()
    {
        Walls[0].SetActive(false); //현재 웨이브 벽 비활성화    
    }

    void SetSpawn() // 몹 스폰할 지점 설정
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 spawnPos;
            spawnPos.x = Random.Range((boxScale.position.x - boxScale.localScale.x / 2), (boxScale.position.x + boxScale.localScale.x / 2));
            spawnPos.z = Random.Range((boxScale.position.z - boxScale.localScale.z / 2), (boxScale.position.z + boxScale.localScale.z / 2));
            spawnPos.y = boxScale.position.y;
            sPos.Add(spawnPos);
        
            SpawnEnemy(spawnPos, i);
                      
        }
        isSpanw = true;
    }

    void SpawnEnemy(Vector3 spawnPos, int count) //몹 스폰
    {
        StartCoroutine(ShowRoutine(spawnPos));

        switch (eTypes[count])
        {
            case enemyType.Melee:
                MeleeController mcon = PhotonNetwork.Instantiate("MeleeEnemy", spawnPos, Quaternion.identity).GetComponent<MeleeController>();              
                enemis.Add(mcon);
                mcon.onDeath += () => enemis.Remove(mcon);
                mcon.onDeath += () => StartCoroutine(DestroyAfter(mcon.gameObject,10f));
                break;

            case enemyType.Rifle:
                RogueController rcon = PhotonNetwork.Instantiate("RogueEnemy", spawnPos, Quaternion.identity).GetComponent<RogueController>();
                enemis.Add(rcon);
                rcon.onDeath += () => enemis.Remove(rcon);
                rcon.onDeath += () => StartCoroutine(DestroyAfter(rcon.gameObject, 10f));
                break;

            case enemyType.Rogue:
                RifleController ricon = PhotonNetwork.Instantiate("RifleEnemy", spawnPos, Quaternion.identity).GetComponent<RifleController>();            
                enemis.Add(ricon);
                ricon.onDeath += () => enemis.Remove(ricon);
                ricon.onDeath += () => StartCoroutine(DestroyAfter(ricon.gameObject, 10f));
                break;
        }
  
    }

    IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        if(target != null)
        {
            PhotonNetwork.Destroy(target);
        }
    }

    IEnumerator ShowRoutine(Vector3 spawnPos)
    {
        GameObject potal = ObjectPool.GetPotal();
        potal.transform.position = spawnPos;
        potal.gameObject.SetActive(true);

        yield return new WaitForSeconds(4f);

        ObjectPool.ReturnPotal(potal);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && PhotonNetwork.IsMasterClient && !isEnter) //플레이어와 충돌하고 마스터 클라이언트일 경우
        {
            SetSpawn();
            isEnter = true;
        }
    }

}
