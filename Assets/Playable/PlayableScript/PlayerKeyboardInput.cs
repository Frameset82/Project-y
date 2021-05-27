﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyboardInput : MonoBehaviour
{
    [Header("옵션창 활성화 여부")]
    [SerializeField] UISetting uiSetting;

    // 입력 버튼 이름
    private string dodgeButtonName = "Jump";

    public float speed = 3.0f;
    float hAxis;
    float vAxis;

    Vector3 moveVec; // 움직임 벡터
    public static Vector3 moveVec1; // 상태 초기화용 벡터
    public Vector3 moveVec2; // 구르기용 벡터
    private Rigidbody rigi;
    private PlayerKeyboardController playerKeyboardController;
    private PlayerEquipmentManager playerEquipmentManager;
    private PlayerAnimation playerAnimation;
    
    public Ray ray;
    public Camera mainCamera;
    public Animator avater;

    public static bool isShoot = false; // 공격중
    public static bool isRight = false; // 우클릭 공격중
    public static bool isSwap = false; // 스왑중
    public static bool isDodge = false; // 회피중
    public static bool onHit = false; // 맞는중
    public static bool onNuckBack = false; // 넉백(다운)중
    public static bool onStun = false; // 스턴중

    public static float maxCcTime = 0f; // 시간 저장용
    private float ccTime = 0f;
    //스턴 파티클
    public GameObject Stunps;
    void Start()
    {
        rigi = GetComponent<Rigidbody>();
        playerKeyboardController = GetComponent<PlayerKeyboardController>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerAnimation = GetComponent<PlayerAnimation>();
        moveVec2 = transform.forward;
    }
    void FixedUpdate()
    {
        if(PlayerKeyboardController.isInteraction){return;}
        InputMove();
        InputDodge();
    }

    private void Update()
    {
        InputEscape();
        Attack();
        Interation();
        RightAttack();
        SwapInput();
        SwapCheck();
        CcCheck();
    }

    public void InputMove()
    {
        if (playerKeyboardController.pState == PlayerKeyboardController.PlayerState.Dodge || playerKeyboardController.pState == PlayerKeyboardController.PlayerState.Death || playerKeyboardController.pState == PlayerKeyboardController.PlayerState.Attack || isSwap == true || onHit == true || playerKeyboardController.pState == PlayerKeyboardController.PlayerState.onCC || isRight)
            return;
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            playerKeyboardController.Move();
            avater.SetBool("isMove", true);
        }
        else
        {
            playerKeyboardController.unMove();
            avater.SetBool("isMove", false);
        }

        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        Vector3 heading = mainCamera.transform.localRotation * Vector3.forward;
        heading = Vector3.Scale(heading, new Vector3(1, 0, 1)).normalized;
        moveVec = heading * Time.fixedDeltaTime * Input.GetAxisRaw("Vertical") * speed;
        moveVec += Quaternion.Euler(0, 90, 0) * heading * Time.fixedDeltaTime * Input.GetAxisRaw("Horizontal") * speed;

        rigi.MovePosition(rigi.position + moveVec);
        moveVec1 = moveVec;
        moveVec1.y = 0;

        transform.LookAt(transform.position + moveVec1);

        avater.SetFloat("Up", vAxis);
        avater.SetFloat("Speed", hAxis);
    }

    public void InputDodge()
    {
        if (Input.GetButton(dodgeButtonName))
        {
            hAxis = Input.GetAxisRaw("Horizontal");
            vAxis = Input.GetAxisRaw("Vertical");

            if (hAxis != 0 || vAxis != 0)
            {
                Vector3 heading = mainCamera.transform.localRotation * Vector3.forward;
                heading = Vector3.Scale(heading, new Vector3(1, 0, 1)).normalized;
                moveVec2 = heading * Time.fixedDeltaTime * Input.GetAxisRaw("Vertical") * speed;
                moveVec2 += Quaternion.Euler(0, 90, 0) * heading * Time.fixedDeltaTime * Input.GetAxisRaw("Horizontal") * speed;
            }
            else if (hAxis == 0 && vAxis == 0)
            {
                moveVec2 = transform.forward;
            }

            playerKeyboardController.Dodge(moveVec2);
            transform.LookAt(transform.position + moveVec2);
        }
    }

    public void Attack()
    {
        if (Input.GetMouseButtonDown(0) && !isShoot && playerKeyboardController.pState != PlayerKeyboardController.PlayerState.RIghtAttack && playerEquipmentManager.equipWeapon != null && isSwap == false)
        {
            print("공격");
            RaycastHit hit;
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 destination = new Vector3(hit.point.x, 0, hit.point.z);
                playerKeyboardController.Attack(destination);
            }
        }
    }

    public void RightAttack()
    {
        if (Input.GetMouseButtonDown(1) && !isRight && playerKeyboardController.pState != PlayerKeyboardController.PlayerState.Attack && playerEquipmentManager.equipWeapon != null && isSwap == false)
        {
            print("공격");
            RaycastHit hit;
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 destination = new Vector3(hit.point.x, 0, hit.point.z);
                playerKeyboardController.RightAttack(destination);
            }
        }
    }

    // 상호작용 키 입력
    public void Interation()
    {
        if(Input.GetButtonDown("Interation")){
            if(playerEquipmentManager.nearObject != null && isSwap == false){
                playerEquipmentManager.Interation();
            }
            if(playerKeyboardController.targetInterObj != null){
                playerKeyboardController.targetInterObj.ActiveUI();
            }
        }


        // if ((Input.GetButtonDown("Interation") && playerEquipmentManager.nearObject != null) && isSwap == false) // g키로 획득
        // {
        //     playerEquipmentManager.Interation();
        // }
    }

    public void SwapInput()
    {
        if (Input.GetButtonDown("Swap1") && (playerEquipmentManager.mainWeapon == null || PlayerEquipmentManager.equipCount == 1))
            return;
        if (Input.GetButtonDown("Swap2") && (playerEquipmentManager.subWeapon == null || PlayerEquipmentManager.equipCount == 2))
            return;

        if (PlayerKeyboardInput.isShoot == false && !PlayerKeyboardInput.isDodge && !PlayerKeyboardInput.isSwap)
        {
            if (Input.GetButtonDown("Swap1"))
            {
                playerEquipmentManager.Swap(1);
                print("1입력");
            }
            else if (Input.GetButtonDown("Swap2"))
            {
                playerEquipmentManager.Swap(2);
                print("2입력");
            }
        }
    }

    public void SwapCheck()
    {
        if (PlayerKeyboardInput.isSwap == true)
        {
            playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Swap;
        }
        else if (playerKeyboardController.pState == PlayerKeyboardController.PlayerState.Swap && PlayerKeyboardInput.isSwap == false)
        {
            playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Idle;
        }
    }

    public void CcCheck()
    {
        if (onNuckBack || onStun)
        {
            playerAnimation.playerAnimator.SetBool("OnCC", true);
            playerAnimation.playerAnimator.SetBool("isMove", false);
            playerAnimation.playerAnimator.SetFloat("Up", 0.0f);
            playerAnimation.playerAnimator.SetFloat("Speed", 0.0f);
            ccTime += Time.deltaTime;

            Stunps.SetActive(true);

            if (ccTime >= maxCcTime)
            {
                Stunps.SetActive(false);
                ccTime = 0f;
                onNuckBack = false;
                onStun = false;
                playerAnimation.playerAnimator.SetBool("OnCC", false);
                playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Idle;
            }
        }
    }

    // ESC 키 입력
    void InputEscape(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(PlayerKeyboardController.isInteraction){
                if(playerKeyboardController.targetInterObj != null){
                    playerKeyboardController.targetInterObj.InactiveUI();
                }
                uiSetting.CloseUI();
            } else {
                uiSetting.OpenUI();
            }
        }
    }
}
