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
    public string inputRoomName;           // 방 이름
    public List<string> roomUser;           // 방 참가자 리스트
    public List<string> roomList;           // 방 목록
    public List<string> roomName;           // 방 이름 리스트
    public List<string> roomMax;            // 방 인원 리스트
    List<RoomInfo> serverRoomList = new List<RoomInfo>();          // 방 목록 리스트
    
    int currentPage = 1, maxPage, multiple; // 페이지 관리 변수
    public bool isProviousBtnActive;        // 이전 버튼 활성화 여부
    public bool isNextBtnActive;            // 다음 버튼 활성화 여부
    public List<bool> roomListBtn;              // 방 목록 버튼 활성화 여부

    public UnityEvent networkStateUpate;    // 네트워크 상태 갱신 이벤트
    public UnityEvent lobbyInfoRefresh;     // 로비 정보 새로고침 이벤트
    public UnityEvent roomInfoRefresh;      // 방 정보 새로고침 이벤트
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
    // 마스터 서버 접속 끊기 시도
    public void DisConnect(){
        PhotonNetwork.Disconnect();
    }
    // 마스터 서버 접속 끊어짐
    public override void OnDisconnected(DisconnectCause cause){
        StateUpdate("서버 접속 끊어짐");
        networkStateUpate.Invoke();
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
        RefreshLobbyInfo();
        networkStateUpate.Invoke();
        isConnected.Invoke();
    }
    // 방 만들기 시도
    public void CreateRoom(){
        StateUpdate("방 만들기 시도 중...");
        PhotonNetwork.CreateRoom(
            inputRoomName, new RoomOptions { MaxPlayers = 2});
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
        PhotonNetwork.JoinRoom(inputRoomName);
        networkStateUpate.Invoke();
        allBtnInactive.Invoke();
    }
    // 방 참가하기 성공
    public override void OnJoinedRoom(){
        StateUpdate("방 참가 완료");
        RefreshRoomInfo();
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
        ServerRoomListRefresh();
        lobbyInfoRefresh.Invoke();
    }
    // 방 정보 새로 고침
    public void RefreshRoomInfo(){
        inputRoomName = PhotonNetwork.CurrentRoom.Name;
        for(int i=0; i < PhotonNetwork.PlayerList.Length; i++){
            roomUser.Add(PhotonNetwork.PlayerList[i].NickName);
        }
        roomInfoRefresh.Invoke();
    }
    // 방에 유저가 참가했을 때
    public override void OnPlayerEnteredRoom(Player newPlayer){
        RefreshRoomInfo();
    }
    // 방 목록이 갱신 될 때
    public override void OnRoomListUpdate(List<RoomInfo> roomList){
        int roomCount = roomList.Count;
        for(int i = 0; i < roomCount; i++){
            if(!roomList[i].RemovedFromList){
                if(!serverRoomList.Contains(roomList[i])) serverRoomList.Add(roomList[i]);
                else serverRoomList[serverRoomList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (serverRoomList.IndexOf(roomList[i]) != -1)
                serverRoomList.RemoveAt(serverRoomList.IndexOf(roomList[i]));
        }
        ServerRoomListRefresh();
    }
    // 방 목록 패널 버튼 클릭시
    public void ServerRoomListClick(int num){
        if(num == -2) --currentPage;
        else if(num == -1) ++currentPage;
        RefreshLobbyInfo();
    }
    // 방 목록 패널 새로고침
    void ServerRoomListRefresh(){
        if(serverRoomList.Count!=0 && roomList.Count !=0){
            maxPage = (serverRoomList.Count % roomList.Count == 0)
            ? serverRoomList.Count / roomList.Count : serverRoomList.Count / roomList.Count + 1;
        }
        isProviousBtnActive = (currentPage <= 1) ? false : true;
        isNextBtnActive = (currentPage >= maxPage) ? false : true;

        multiple = (currentPage - 1) * roomList.Count;
        if(roomList.Count==0) return;
        for(int i = 0; i < roomList.Count; i++){
            // roomListBtn[i] = (multiple + i < serverRoomList.Count)
            //  ? true : false;
            roomListBtn.Add((multiple + i < serverRoomList.Count) ? true : false);
            // roomName[i] = (multiple + i < serverRoomList.Count)
            //  ? serverRoomList[multiple + i].Name : "";
            roomName.Add((multiple + i < serverRoomList.Count) ? serverRoomList[multiple + i].Name : "");
            // roomMax[i] = (multiple + i < serverRoomList.Count)
            //  ? serverRoomList[multiple + i].PlayerCount + "/" + serverRoomList[multiple + i].MaxPlayers : "";
            roomMax.Add((multiple + i < serverRoomList.Count) ? serverRoomList[multiple + i].PlayerCount + "/" + serverRoomList[multiple + i].MaxPlayers : "");

        }
    }
}
