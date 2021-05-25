using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINetworkManager : UserInterface
{
    [SerializeField] NetworkManager networkManager; // 네트워크 매니저
    public Text StatusText;                         // 네트워크 상태 텍스트
    public InputField roomInput, nicknameInput;     // 닉네임, 방 이름 인풋필드

    public Button ServerConnetBtn;      // 서버 연결 버튼
    public Button CreateRoomBtn;        // 방 만들기 버튼
    public Button JoinRoomBtn;          // 방 입장 버튼
    public Button JoinRandomRoomBtn;    // 방 랜덤 입장 버튼
    public Button LeaveRoomBtn;         // 방 나가기 버튼

    void Start() {
        CreateRoomBtn.interactable = false;
        JoinRoomBtn.interactable = false;
        JoinRandomRoomBtn.interactable = false;
        LeaveRoomBtn.interactable = false;
    }
    // 모든 버튼 비활성화
    public void AllBtnInactive(){
        ServerConnetBtn.interactable = false;
        CreateRoomBtn.interactable = false;
        JoinRoomBtn.interactable = false;
        JoinRandomRoomBtn.interactable = false;
        LeaveRoomBtn.interactable = false;
    }
    // 서버 연결 버튼 활성화
    public void ServerConnetBtnActive(){ServerConnetBtn.interactable = true;}
    // 서버 연결 버튼 비활성화
    public void ServerConnetBtnInactive(){ServerConnetBtn.interactable = false;}
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

    // 버튼 클릭 메서드
    // 서버 연결 버튼 클릭
    public void ClickServerConnetBtn(){
        if(nicknameInput.text==""){
            StatusText.text = "닉네임은 공백일 수 없습니다.";
            return;
        }
        networkManager.userNickname = nicknameInput.text;
        networkManager.Connect();
    }
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

}
