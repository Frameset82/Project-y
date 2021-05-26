using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINetworkManager : UserInterface
{
    [SerializeField]
    NetworkManager networkManager;      // 네트워크 매니저
    public Text StatusText;             // 네트워크 상태 텍스트
    public Text LobbyInfoText;          // 로비 정보 텍스트
    public InputField roomInput;        // 방 이름 인풋필드
    public Button CreateRoomBtn;        // 방 만들기 버튼
    public Button JoinRoomBtn;          // 방 입장 버튼
    public Button JoinRandomRoomBtn;    // 방 랜덤 입장 버튼
    public Button LeaveRoomBtn;         // 방 나가기 버튼

    // 시작 시 모든 버튼 비활성화 후 서버 연결 시도
    void Start() {
        AllBtnInactive();
        networkManager.Connect();
    }

    // 모든 버튼 비활성화
    public void AllBtnInactive(){
        CreateRoomBtn.interactable = false;
        JoinRoomBtn.interactable = false;
        JoinRandomRoomBtn.interactable = false;
        LeaveRoomBtn.interactable = false;
    }
    // 방 나가기 버튼 활성화
    public void LeaveRoomBtnActive(){LeaveRoomBtn.interactable = true;}
    // 방 나가기 버튼 비활성화
    public void LeaveRoomBtnInactive(){LeaveRoomBtn.interactable = false;}
    // 방 입장 버튼 활성화
    public void JoinRoomBtnActive(){
        CreateRoomBtn.interactable = true;
        JoinRoomBtn.interactable = true;
        JoinRandomRoomBtn.interactable = true;
    }
    // 방 입장 버튼 비활성화
    public void JoinRoomBtnInactive(){
         CreateRoomBtn.interactable = false;
        JoinRoomBtn.interactable = false;
        JoinRandomRoomBtn.interactable = false;
    }
    // 네트워크 상태 갱신
    public void StateUpdate(){
        StatusText.text = networkManager.networkState;
    }
    // 로비정보 갱신
    public void LobbyInfoUpdate(){
        LobbyInfoText.text = networkManager.lobbyInfo;
    }

    // 버튼 클릭 메서드
    // 방 만들기 버튼 클릭
    public void ClickCreateRoomBtn(){
        if(roomInput.text==""){
            StatusText.text = "방 이름은 공백일 수 없습니다.";
            return;
        }
        networkManager.roomName = roomInput.text;
        networkManager.CreateRoom();
    }
    // 방 입장 버튼 클릭
    public void ClickJoinRoomBtn(){
        if(roomInput.text==""){
            StatusText.text = "방 이름은 공백일 수 없습니다.";
            return;
        }
        networkManager.roomName = roomInput.text;
        networkManager.JoinRoom();
    }
    // 방 랜덤 입장 버튼 클릭
    public void ClickJoinRandomRoomBtn(){
        networkManager.JoinRandomRoom();
    }
    // 방 나가기 버튼 클릭
    public void ClickLeaveRoomBtn(){
        networkManager.LeaveRoom();
    }
    // ESC 버튼 클릭
    public void ClickEscapeBtn(){
        CloseUI();
    }
    // 로비정보 새로고침 버튼 클릭
    public void ClickRefreshBtn(){
        networkManager.RefreshLobbyInfo();
    }

}
