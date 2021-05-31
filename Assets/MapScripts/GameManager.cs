using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    // 싱글톤 접근용 프로퍼티
    public static GameManager instance{
        get{
            if(m_instance){
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance;
        }
    }
    // 싱글톤이 할당될 static 변수
    private static GameManager m_instance;
    // 게임 오버 상태
    public bool isGameover{get; private set;}
    // 플레이어 프리팹
    public GameObject playerPrefab;
    public Vector3 playerSpawn0;
    public Vector3 playerSpawn1;

    // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면 자신을 파괴
    private void Awake() {
        if(instance != this){
            Destroy(gameObject);
        }
    }
    
    private void Start() {
        if(PhotonNetwork.IsMasterClient){
            PhotonNetwork.Instantiate(playerPrefab.name, playerSpawn0, Quaternion.identity);
        } else {
            PhotonNetwork.Instantiate(playerPrefab.name, playerSpawn1, Quaternion.identity);
        }
    }

}
