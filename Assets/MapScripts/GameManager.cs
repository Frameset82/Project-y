using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.Events;

public class GameManager : MonoBehaviourPunCallbacks
{
    // 싱글톤 접근용 프로퍼티
    public static GameManager instance;

    // 게임 오버 상태
    public bool isGameover{get; private set;}
    // 플레이어 프리팹
    public GameObject playerPrefab;
    public Transform playerSpawn0;
    public Transform playerSpawn1;
    public UnityEvent playerSpwan;
    public static bool isMulti{get; private set;} // 멀티플레이 환경 체크

    // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면 자신을 파괴
    private void Awake() {
        if(instance == null){
            instance = this;
        } else {
            if(instance!=this){
                Destroy(gameObject);
            }
        }
    }
    
    private void Start() {
        if(PhotonNetwork.IsConnected){
            isMulti = true;
        } else {
            isMulti = false;
        }

        if(isMulti){
            if(PhotonNetwork.IsMasterClient){
                PhotonNetwork.Instantiate(playerPrefab.name, playerSpawn0.position, Quaternion.identity);
            } else {
                PhotonNetwork.Instantiate(playerPrefab.name, playerSpawn1.position, Quaternion.identity);
            }
        } else {
            Instantiate(playerPrefab, playerSpawn0.position, Quaternion.identity);
        }
        PlayerKeyboardController.isInteraction=false;
        playerSpwan.Invoke();
    }
}
