using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어의 조작을 위한 입력감지
// 감지된 입력값을 다른 컴포넌트가 사용할 수 있도록 제공
public class PlayerInput : MonoBehaviour
{
    // 입력 버튼 이름
    private string dodgeButtonName = "Jump";
    
    // 변수값은 내부에서만 할당가능
    public bool move { get; private set; }
    public bool dodge { get; private set; }

    // 이동 딜레이 변수
    private float timeBetMove = 0.15f;
    private float lastMoveTime;

    public PlayerController playerController;
    public Camera mainCamera;
    public Ray ray;
    
    private void Start() {
        playerController = GetComponent<PlayerController>();
    }
    // 매 프레임 입력감지
    private void FixedUpdate()
    {
        InputMove();
        InputDodge();
        InputRifleShoot();
    }

    // 캐릭터 이동 입력
    public void InputMove(){
        if(Input.GetMouseButton(1) && Time.time >= lastMoveTime + timeBetMove){
            lastMoveTime = Time.time;
            RaycastHit hit;
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            playerController.WillMove(); // 회피중 이동 명령예약
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "Player") return; 
                Vector3 destination = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                playerController.Move(destination);
            }
        }
    }
    // 캐릭터 회피 입력
    public void InputDodge(){
        if(Input.GetButton(dodgeButtonName)){
            RaycastHit hit;
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit)){
                Vector3 destination = new Vector3(hit.point.x, 0, hit.point.z);
                playerController.Dodge(destination);
            }
        }
    }
    public void InputRifleShoot()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 destination = new Vector3(hit.point.x, 0, hit.point.z);
                playerController.Attack(destination);
            }
        }
    }
}