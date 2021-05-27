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
    public Text RoomName;               // 방 이름 텍스트
    public Text HostUser;               // 호스트 유저 닉네임 텍스트
    public Text RoomUser;               // 참가자 유저 닉네임 텍스트
    public InputField roomInput;        // 방 이름 인풋필드
    public GameObject roomListPnl;      // 방 목록 패널
    public GameObject roomPnl;          // 방 정보 패널
    public Button CreateRoomBtn;        // 방 만들기 버튼
    public Button JoinRoomBtn;          // 방 입장 버튼
    public Button JoinRandomRoomBtn;    // 방 랜덤 입장 버튼
    public Button LeaveRoomBtn;         // 방 나가기 버튼
    public Button PreviousBtn;          // 이전 버튼
    public Button NextBtn;              // 다음 버튼
    public List<Button> RoomListBtn;    // 방 목록 버튼 리스트

    // 시작 시 모든 버튼 비활성화 후 서버 연결 시도
    void Start() {
        AllBtnInactive();
        RoomPnlInactive();
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
    // 방 리스트 패널 활성화
    public void RoomListPnlActive(){roomListPnl.SetActive(true);}
    // 방 리스트 패널 비활성화
    public void RoomListPnlInactive(){roomListPnl.SetActive(false);}
    // 방 정보 패널 활성화
    public void RoomPnlActive(){roomPnl.SetActive(true);}
    // 방 정보 패널 비활성화
    public void RoomPnlInactive(){roomPnl.SetActive(false);}
    // 이전, 다음 버튼 활성화 여부 갱신
    public void MoveListBtnUpdate(){
        PreviousBtn.interactable = networkManager.isProviousBtnActive;
        NextBtn.interactable = networkManager.isNextBtnActive;
    }
    // 방 목록 버튼 활성화 여부, 정보 갱신
    public void RoomListBtnUpdate(){
        for(int i=0; i < RoomListBtn.Count; i++){
            if(i < networkManager.roomListBtn.Count){
                RoomListBtn[i].interactable = networkManager.roomListBtn[i];
                RoomListBtn[i].transform.GetChild(0).GetComponent<Text>().text = networkManager.roomName[i];
                RoomListBtn[i].transform.GetChild(1).GetComponent<Text>().text = networkManager.roomMax[i];
            }
        }
    }
    // 방 정보 갱신
    public void RoomInfoUpdate(){
        RoomName.text = networkManager.inputRoomName;
        HostUser.text = networkManager.roomUser[0];
        if(networkManager.roomUser.Count>1) RoomUser.text = networkManager.roomUser[1];
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
        networkManager.inputRoomName = roomInput.text;
        networkManager.CreateRoom();
    }
    // 방 입장 버튼 클릭
    public void ClickJoinRoomBtn(){
        if(roomInput.text==""){
            StatusText.text = "방 이름은 공백일 수 없습니다.";
            return;
        }
        networkManager.inputRoomName = roomInput.text;
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
    // 이전 버튼 클릭
    public void ClickPreviousBtn(){
        networkManager.ServerRoomListClick(-2);
    }
    // 다음 버튼 클릭
    public void ClickNextBtn(){
        networkManager.ServerRoomListClick(-1);
    }
    

}
