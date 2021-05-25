using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // public string gameVersion  = "1.0";          // 게임 버전
    public string networkState = "Network Status";  // 네트워크 상태 메시지
    public string userNickname = "User";            // 유저 닉네임
    public string roomName = "MyRoom";              // 방 이름

    public UnityEvent networkStateUpate;    // 네트워크 상태 갱신 이벤트
    public UnityEvent allBtnInactive;       // 모든 버튼 비활성화 이벤트
    public UnityEvent isConnected;          // 서버 연결 이벤트
    public UnityEvent isDisconnected;       // 서버 연결 끊어짐 이벤트
    public UnityEvent isJoinedRoom;         // 방 입장 이벤트
    public UnityEvent isLeavedRoom;         // 방 나가기 이벤트

    void Start()
    {
        StateUpdate("마스터 서버 접속 대기중");
        networkStateUpate.Invoke();
    }
    // 네트워크 상태 갱신 메서드
    public void StateUpdate(string State){
        networkState = State;
    }

    // 마스터 서버 접속 시도
    public void Connect(){
        PhotonNetwork.ConnectUsingSettings();
        StateUpdate("마스터 서버에 접속 시도 중...");
        networkStateUpate.Invoke();
        allBtnInactive.Invoke();
    }
    // 마스터 서버 접속 성공
    public override void OnConnectedToMaster(){
        // 입력한 닉네임으로 플레이어 닉네임 지정
        PhotonNetwork.LocalPlayer.NickName = userNickname;
        StateUpdate("온라인 : 마스터 서버와 연결됨");
        networkStateUpate.Invoke();
        JoinLobby();
    }
    // 마스터 서버 접속 끊어짐
    public override void OnDisconnected(DisconnectCause cause){
        StateUpdate("마스터 서버 접속 끊김");
        networkStateUpate.Invoke();
        isDisconnected.Invoke();
    }
    
    // 로비 접속 시도
    public void JoinLobby(){
        PhotonNetwork.JoinLobby();
        StateUpdate("로비에 접속 시도 중...");
        networkStateUpate.Invoke();
    }
    // 로비 접속 성공
    public override void OnJoinedLobby(){
        StateUpdate("온라인 : 로비 접속 성공");
        networkStateUpate.Invoke();
        isConnected.Invoke();
    }
    
    // 방 만들기 시도
    public void CreateRoom(){
        PhotonNetwork.CreateRoom(
            roomName, new RoomOptions { MaxPlayers = 2});
        StateUpdate("방 만들기 시도 중...");
        networkStateUpate.Invoke();
        allBtnInactive.Invoke();
    }
    // 방 만들기 성공
    public override void OnCreatedRoom(){
        StateUpdate("방 만들기 완료");
        networkStateUpate.Invoke();
        isJoinedRoom.Invoke();
    }
    // 방 참가하기 시도
    public void JoinRoom(){
        PhotonNetwork.JoinRoom(roomName);
        StateUpdate("방 참가하기 시도 중...");
        networkStateUpate.Invoke();
        allBtnInactive.Invoke();
    }
    // 방 참가하기 성공
    public override void OnJoinedRoom(){
        StateUpdate("방 참가 완료");
        networkStateUpate.Invoke();
        isJoinedRoom.Invoke();
    }
    // 방 랜덤 참가하기
    public void JoinRandomRoom(){
        PhotonNetwork.JoinRandomRoom();
        StateUpdate("랜덤 방 참가하기 시도 중..");
        networkStateUpate.Invoke();
        allBtnInactive.Invoke();
    }
    // 방 나가기 시도
    public void LeaveRoom(){
        PhotonNetwork.LeaveRoom();
        StateUpdate("온라인 : 로비 접속 성공");
        networkStateUpate.Invoke();
        isLeavedRoom.Invoke();
    }

    // 방 만들기 실패
    public override void OnCreateRoomFailed(short returnCode, string message){
        StateUpdate("방 만들기 실패");
        networkStateUpate.Invoke();
        isLeavedRoom.Invoke();
    }
    // 방 참가하기 실패
    public override void OnJoinRoomFailed(short returnCode, string message){
        StateUpdate("방 참가하기 실패");
        networkStateUpate.Invoke();
        isLeavedRoom.Invoke();
    }
    // 방 랜덤 참가하기 실패
    public override void OnJoinRandomFailed(short returnCode, string message){
        StateUpdate("방 랜덤 참가하기 실패");
        networkStateUpate.Invoke();
        isLeavedRoom.Invoke();
    }

}
