using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // string _gameVersion  = "1";          // 게임 버전
    public string networkState;             // 네트워크 상태
    public string lobbyInfo;                // 로비 정보
    public string userNickname="Player";    // 유저 닉네임
    public string roomName;                 // 방 이름

    public UnityEvent networkStateUpate;    // 네트워크 상태 갱신 이벤트
    public UnityEvent allBtnInactive;       // 모든 버튼 비활성화 이벤트
    public UnityEvent isConnected;          // 서버 연결 이벤트
    public UnityEvent isJoinedRoom;         // 방 입장 이벤트

    // 네트워크 상태 갱신 메서드
    public void StateUpdate(string State){
        networkState = State;
    }

    // 마스터 서버 접속 시도
    // 유저 닉네임이 null인 경우 return
    public void Connect(){
        if(userNickname==null) return; 
        StateUpdate("마스터 서버에 접속 시도 중...");
        PhotonNetwork.ConnectUsingSettings();
        networkStateUpate.Invoke();
        allBtnInactive.Invoke();
    }
    // 마스터 서버 접속 성공
    // 로비 접속 시도
    public override void OnConnectedToMaster(){
        // 입력한 닉네임으로 플레이어 닉네임 지정
        PhotonNetwork.LocalPlayer.NickName = userNickname;
        StateUpdate("온라인 : 마스터 서버와 연결됨");
        networkStateUpate.Invoke();
        JoinLobby();
    }
    // 마스터 서버 접속 끊어짐
    // 서버 접속 재시도
    public override void OnDisconnected(DisconnectCause cause){
        StateUpdate("서버 접속 끊어짐, 재접속 시도 중...");
        Connect();
        networkStateUpate.Invoke();
        allBtnInactive.Invoke();
    }
    // 로비 접속 시도
    public void JoinLobby(){
        StateUpdate("로비에 접속 시도 중...");
        PhotonNetwork.JoinLobby();
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
        StateUpdate("방 만들기 시도 중...");
        PhotonNetwork.CreateRoom(
            roomName, new RoomOptions { MaxPlayers = 2});
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
        StateUpdate("방 참가하기 시도 중...");
        PhotonNetwork.JoinRoom(roomName);
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
        StateUpdate("랜덤 방 참가하기 시도 중..");
        PhotonNetwork.JoinRandomRoom();
        networkStateUpate.Invoke();
        allBtnInactive.Invoke();
    }
    // 방 나가기 시도
    public void LeaveRoom(){
        PhotonNetwork.LeaveRoom();
        StateUpdate("온라인 : 로비 접속 성공");
        networkStateUpate.Invoke();
        isConnected.Invoke();
    }

    // 방 만들기 실패
    public override void OnCreateRoomFailed(short returnCode, string message){
        StateUpdate("방 만들기 실패");
        networkStateUpate.Invoke();
        isConnected.Invoke();
    }
    // 방 참가하기 실패
    public override void OnJoinRoomFailed(short returnCode, string message){
        StateUpdate("방 참가하기 실패");
        networkStateUpate.Invoke();
        isConnected.Invoke();
    }
    // 방 랜덤 참가하기 실패
    public override void OnJoinRandomFailed(short returnCode, string message){
        StateUpdate("방 랜덤 참가하기 실패");
        networkStateUpate.Invoke();
        isConnected.Invoke();
    }
    // 로비 정보 새로 고침
    public void RefreshLobbyInfo(){
        lobbyInfo = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms)
         + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";
    }

}
