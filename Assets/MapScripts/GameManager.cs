using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.Events;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;           // 싱글톤 접근용 프로퍼티
    public bool isGameover{get; private set;}     // 게임 오버 상태
    public GameObject playerPrefab;               // 플레이어 프리팹
    public GameObject playerEffect;               // 플레이어 스폰 효과
    public Transform playerSpawn0;                // 플레이어 스폰 위치 0번
    public Transform playerSpawn1;                // 플레이어 스폰 위치 1번
    public UnityEvent playerSpawn;                // 플레이어 스폰 이벤트
    public AudioClip playerSpawnClip;             // 플레이어 스폰 클립

    public static bool isMulti{get; private set;} // 멀티플레이 환경 체크

    // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면 자신을 파괴
    void Awake() {
        if(instance == null){
            instance = this;
        } else {
            if(instance!=this){
                Destroy(gameObject);
            }
        }
    }
    
    void Start() {
        if(PhotonNetwork.IsConnected){
            isMulti = true;
        } else {
            isMulti = false;
        }

        if(isMulti){
            if(PhotonNetwork.IsMasterClient){
                PhotonNetwork.Instantiate(playerEffect.name, playerSpawn0.position, Quaternion.identity);
                PhotonNetwork.Instantiate(playerPrefab.name, playerSpawn0.position, Quaternion.identity);
                SoundManager.instance.SFXPlay(playerSpawnClip, gameObject);
            } else {
                PhotonNetwork.Instantiate(playerEffect.name, playerSpawn1.position, Quaternion.identity);
                PhotonNetwork.Instantiate(playerPrefab.name, playerSpawn1.position, Quaternion.identity);
                SoundManager.instance.SFXPlay(playerSpawnClip, gameObject);
            }
        } else {
            Instantiate(playerEffect, playerSpawn0.position, Quaternion.identity);
            Instantiate(playerPrefab, playerSpawn0.position, Quaternion.identity);
            SoundManager.instance.SFXPlay(playerSpawnClip, gameObject);
        }
        PlayerKeyboardController.isInteraction=false;
        playerSpawn.Invoke();
        SoundManager.instance.BgmPlay();
    }


}
