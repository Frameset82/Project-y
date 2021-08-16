using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInput : MonoBehaviourPun
{
    [Header("옵션창 활성화 여부")]
    [SerializeField] UISetting uiSetting;

    // 입력 버튼 이름
    private string dodgeButtonName = "Jump";

    public Vector3 moveVec2; // 구르기용 벡터
    public static Camera mainCamera;

    private PlayerController playerController;
    private PlayerEquipmentManager playerEquipmentManager;
    private PlayerAnimation playerAnimation;
    private PlayerInfo playerInfo;
    private PlayerInput playerKeyboardInput;

    public Ray ray;
    public float moveSpeed = 7.0f;

    public bool isBasicAttacking = false; // 공격중
    public bool isSpecialAttacking = false; // 우클릭 공격중
    public bool isSwap = false; // 스왑중
    public bool isDodge = false; // 회피중
    public bool onHit = false; // 맞는중
    public bool onNuckBack = false; // 넉백(다운)중
    public bool onStun = false; // 스턴중
    public bool isChange = false; // 무기 교체중

    public float maxCcTime = 0f; // 시간 저장용
    private float ccTime = 0f;
    public float delay = 0.4f; // 어택코루틴 딜레이
    public float animSpeed = 1.0f; // 애니메이터 스피드
    //스턴 파티클
    public GameObject Stunps;

    Vector3 inputDirection;
    
    private void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();
        playerEquipmentManager = gameObject.GetComponent<PlayerEquipmentManager>();
        playerInfo = gameObject.GetComponent<PlayerInfo>();
        playerAnimation = gameObject.GetComponent<PlayerAnimation>();
        playerKeyboardInput = gameObject.GetComponent<PlayerInput>();
        playerAnimation.ChangeAnimator();
        moveVec2 = transform.forward;
        mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        if(PlayerController.isInteraction) return;
        if(GameManager.isMulti && !photonView.IsMine) return;
        InputMove();
        InputDodge();
    }

    private void Update()
    {
        if (GameManager.isMulti && !photonView.IsMine) return;
        InputEscape();
        BasicAttack();
        Interation();
        SpecialAttack();
        SwapInput();
        StateCheck();
        CcCheck();
        //Grenade();
    }

    public void InputMove() {
        if (!playerController.canMove) return;
        inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        playerController.Move();
        print(playerController);
    }

    public void InputDodge() {
        if (Input.GetButton(dodgeButtonName) && playerController.canDodge && !playerInfo.playerUIEnable)
        {
            if (inputDirection != Vector3.zero) {
                Vector3 heading = mainCamera.transform.localRotation * Vector3.forward;
                heading = Vector3.Scale(heading, new Vector3(1, 0, 1)).normalized;
                moveVec2 = heading * Time.fixedDeltaTime * Input.GetAxisRaw("Vertical") * moveSpeed;
                moveVec2 += Quaternion.Euler(0, 90, 0) * heading * Time.fixedDeltaTime * Input.GetAxisRaw("Horizontal") * moveSpeed;
            } else {
                moveVec2 = transform.forward;
            }
            transform.LookAt(transform.position + moveVec2);
            playerController.Dodge(moveVec2);
        }
    }

    public void BasicAttack() {
        if (Input.GetMouseButtonDown(0) && playerController.canBasicAttack) {
            RaycastHit hit;
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            int layerMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Enemy"));  // 땅, 적만 raycast

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                Vector3 destination = new Vector3(hit.point.x, gameObject.transform.position.y, hit.point.z);
                playerController.BasicAttack(destination, delay, animSpeed);
            }
        }
    }

    public void SpecialAttack()
    {
        if (Input.GetMouseButtonDown(1) && !isSpecialAttacking && playerController.pState != PlayerController.PlayerState.BasicAttack && playerEquipmentManager.equipWeapon != null && isSwap == false)
        {
            RaycastHit hit;
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            int layerMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Enemy"));  // 땅, 적만 raycast

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Vector3 destination = new Vector3(hit.point.x, gameObject.transform.position.y, hit.point.z);
                playerController.SpecialAttack(destination);
            }
        }
    }

    // 상호작용 키 입력
    public void Interation()
    {
        if(Input.GetButtonDown("Interation")){
            if(playerEquipmentManager.nearObject != null && isSwap == false && isDodge == false){
                playerEquipmentManager.Interation();
            }
            if(playerController.targetInterObj != null){
                playerController.targetInterObj.ActiveUI();
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

        if (playerController.pState != PlayerController.PlayerState.BasicAttack && !playerKeyboardInput.isDodge && !playerKeyboardInput.isSwap)
        {
            if (Input.GetButtonDown("Swap1"))
            {
                playerEquipmentManager.Swap(1);
            }
            else if (Input.GetButtonDown("Swap2"))
            {
                playerEquipmentManager.Swap(2);
            }
        }
    }

    public void StateCheck()
    {
        if (playerKeyboardInput.isSwap == true)
            playerController.pState = PlayerController.PlayerState.Swap;
        else if (playerController.pState == PlayerController.PlayerState.Swap && playerKeyboardInput.isSwap == false)
            playerController.pState = PlayerController.PlayerState.Idle;

        if (isDodge == true)
        {
            playerController.pState = PlayerController.PlayerState.Dodge;
            playerInfo.canDamage = false;
        }
        else if (playerController.pState == PlayerController.PlayerState.Dodge && playerKeyboardInput.isDodge == false)
        {
            playerController.pState = PlayerController.PlayerState.Idle;
            playerInfo.canDamage = true;
        }

        if (isSpecialAttacking == true)
            playerController.pState = PlayerController.PlayerState.SpecialAttack;
        else if (playerController.pState == PlayerController.PlayerState.SpecialAttack && playerKeyboardInput.isSpecialAttacking == false)
            playerController.pState = PlayerController.PlayerState.Idle;
     
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
                playerController.pState = PlayerController.PlayerState.Idle;
            }
        }
        if(onHit == true)
        {
            playerController.pState = PlayerController.PlayerState.Hit;
        }
        else if(playerController.pState == PlayerController.PlayerState.Hit && onHit == false)
        {
            playerController.pState = PlayerController.PlayerState.Idle;
        }
    }

    // ESC 키 입력
    void InputEscape(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(PlayerController.isInteraction){
                if(playerController.targetInterObj != null){
                    playerController.targetInterObj.InactiveUI();
                }
                uiSetting.CloseUI();
            } else {
                uiSetting.OpenUI();
            }
        }
    }

   /* public void Grenade()
    {
        if (Input.GetButtonDown("Grenade") && !isRight && playerKeyboardController.pState != PlayerController.PlayerState.Attack && isSwap == false)
        {
            RaycastHit hit;
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 destination = new Vector3(hit.point.x, 0, hit.point.z);
                playerKeyboardController.Grenade(destination);
            }
        }
    }*/
}
