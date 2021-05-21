using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // UI 버튼
    public Button serverConnectBtn;
    public Button serverDisconnectBtn;
    public Button lobbyConnectBtn;
    public Button createRoomBtn;
    public Button joinRoomBtn;
    public Button JoinRandomRoomBtn;
    public Button leaveRoomBtn;

    public Text StatusText;
    public InputField roomInput, nicknameInput;

    void Start()
    {
        StatusText.text = "마스터 서버 접속 대기중";
        serverDisconnectBtn.interactable = false;
        lobbyConnectBtn.interactable = false;
        createRoomBtn.interactable = false;
        joinRoomBtn.interactable = false;
        JoinRandomRoomBtn.interactable = false;
        leaveRoomBtn.interactable = false;
    }
    

    // 마스터 서버 접속 시도
    public void Connect(){
        if(nicknameInput.text==""){
            StatusText.text = "닉네임은 공백일 수 없습니다.";
            return;
        }
        PhotonNetwork.ConnectUsingSettings();
        StatusText.text = "마스터 서버에 접속 시도 중...";
    }
    // 마스터 서버 접속 성공
    public override void OnConnectedToMaster(){
        // 입력한 닉네임으로 플레이어 닉네임 지정
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
        StatusText.text = "온라인 : 마스터 서버와 연결됨";
        serverConnectBtn.interactable = false;
        serverDisconnectBtn.interactable = true;
        lobbyConnectBtn.interactable = true;
    }
    // 마스터 서버 접속 끊기 시도
    public void Disconnect(){
        PhotonNetwork.Disconnect();
        StatusText.text = "마스터 서버 접속 해제 중...";
    }
    // 마스터 서버 접속 끊어짐
    public override void OnDisconnected(DisconnectCause cause){
        StatusText.text = "마스터 서버 접속 끊김";
        serverConnectBtn.interactable = true;
        serverDisconnectBtn.interactable = false;
        lobbyConnectBtn.interactable = false;
        createRoomBtn.interactable = false;
        joinRoomBtn.interactable = false;
        JoinRandomRoomBtn.interactable = false;
        leaveRoomBtn.interactable = false;
        
    }
    // 로비 접속 시도
    public void JoinLobby(){
        if(!PhotonNetwork.IsConnected){
            StatusText.text = "서버에 접속 중이 아닙니다.";
            return;
        }
        PhotonNetwork.JoinLobby();
        StatusText.text = "로비에 접속 시도 중...";
    }
    // 로비 접속 성공
    public override void OnJoinedLobby(){
        StatusText.text = "온라인 : 로비 접속 성공";
        createRoomBtn.interactable = true;
        joinRoomBtn.interactable = true;
        JoinRandomRoomBtn.interactable = true;
        leaveRoomBtn.interactable = true;
    }
    // 방 만들기 시도
    public void CreateRoom(){
        if(!PhotonNetwork.IsConnected){
            StatusText.text = "서버에 접속 중이 아닙니다.";
            return;
        }
        if(roomInput.text == ""){
            StatusText.text = "방 이름은 공백일 수 없습니다.";
            return;
        }
        PhotonNetwork.CreateRoom(
            roomInput.text, new RoomOptions { MaxPlayers = 2});
        StatusText.text = "방 만들기 시도 중...";
    }
    // 방 만들기 성공
    public override void OnCreatedRoom(){
        StatusText.text = "방 만들기 완료";
        createRoomBtn.interactable = false;
        joinRoomBtn.interactable = false;
        JoinRandomRoomBtn.interactable = false;
    }
    // 방 참가하기 시도
    public void JoinRoom(){
        if(!PhotonNetwork.IsConnected){
            StatusText.text = "서버에 접속 중이 아닙니다.";
            return;
        }
        if(roomInput.text == ""){
            StatusText.text = "방 이름은 공백일 수 없습니다.";
            return;
        }
        PhotonNetwork.JoinRoom(roomInput.text);
        StatusText.text = "방 참가하기 시도 중...";
    }
    // 방 참가하기 성공
    public override void OnJoinedRoom(){
        StatusText.text = "방 참가 완료";
        createRoomBtn.interactable = false;
        joinRoomBtn.interactable = false;
        JoinRandomRoomBtn.interactable = false;
    }
    // 방 랜덤 참가하기
    public void JoinRandomRoom(){
        if(!PhotonNetwork.IsConnected){
            StatusText.text = "서버에 접속 중이 아닙니다.";
            return;
        }
        PhotonNetwork.JoinRandomRoom();
        StatusText.text = "랜덤 방 참가하기 시도 중..";
    }
    // 방 나가기 시도
    public void LeaveRoom(){
        if(!PhotonNetwork.IsConnected){
            StatusText.text = "서버에 접속 중이 아닙니다.";
            return;
        }
        PhotonNetwork.LeaveRoom();
        StatusText.text = "온라인 : 로비 접속 성공";
        createRoomBtn.interactable = true;
        joinRoomBtn.interactable = true;
        JoinRandomRoomBtn.interactable = true;
        leaveRoomBtn.interactable = true;
    }

    // 방 만들기 실패
    public override void OnCreateRoomFailed(short returnCode, string message){
        StatusText.text = "방 만들기 실패";
    }
    // 방 참가하기 실패
    public override void OnJoinRoomFailed(short returnCode, string message){
        StatusText.text = "방 참가하기 실패";
    }
    // 방 랜덤 참가하기 실패
    public override void OnJoinRandomFailed(short returnCode, string message){
        StatusText.text = "랜덤 방 참가하기 실패";
    }

}
