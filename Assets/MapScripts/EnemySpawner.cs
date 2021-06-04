using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class EnemySpawner : MonoBehaviour, IPunObservable
{
    public enum enemyType {None, Melee, Rogue, Rifle, End};

    public enemyType[] eTypes;
    private List<LivingEntity> enemis = new List<LivingEntity>();
    private List<Vector3> sPos = new List<Vector3>();

    public Transform boxScale;
    public int waveCount; 
    public GameObject Wall;
    private PhotonView pv;



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }

    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
           
        }

        if(enemis.Count <= 0 && PhotonNetwork.IsMasterClient)
        {
            pv.RPC("AfterWave", RpcTarget.All);
        }
    }

    void AfterWave()
    {

    }


    void SetSpawn()
    {
        for (int i = 0; i < eTypes.Length; i++)
        {
            Vector3 spawnPos;
            spawnPos.x = Random.Range((boxScale.position.x - boxScale.localScale.x / 2), (boxScale.position.x + boxScale.localScale.x / 2)); 
            spawnPos.z = Random.Range((boxScale.position.z - boxScale.localScale.z / 2), (boxScale.position.z + boxScale.localScale.z / 2));
            spawnPos.y = this.transform.position.y;
            sPos.Add(spawnPos);
        }
        pv.RPC("SpawnEnemy", RpcTarget.All, sPos);
    }

    [PunRPC]
    void SpawnEnemy(List<Vector3> spawnPos)
    {
        Wall.SetActive(true);

        for (int i = 0; i < eTypes.Length; i++)
        {
            switch (eTypes[i])
            { 

            case enemyType.Melee:
                MeleeController mcon = ObjectPool.GetMelee();
                mcon.transform.position = spawnPos[i];
                mcon.gameObject.SetActive(true);
                enemis.Add(mcon);
                mcon.onDeath += () => enemis.Remove(mcon);
                break;

            case enemyType.Rifle:
                RogueController rcon = ObjectPool.GetRogue();
                rcon.transform.position = spawnPos[i];
                rcon.gameObject.SetActive(true);
                enemis.Add(rcon);
                rcon.onDeath += () => enemis.Remove(rcon);
                break;

            case enemyType.Rogue:
                RifleController ricon = ObjectPool.GetRifle();
                ricon.transform.position = spawnPos[i];
                ricon.gameObject.SetActive(true);
                enemis.Add(ricon);
                ricon.onDeath += () => enemis.Remove(ricon);
                break;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && PhotonNetwork.IsMasterClient) //플레이어와 충돌하고 마스터 클라이언트일 경우
        {
            SetSpawn();          
        }
    }



}
