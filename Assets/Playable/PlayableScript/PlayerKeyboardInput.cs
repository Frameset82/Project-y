using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerKeyboardInput : MonoBehaviourPun
{
    [Header("옵션창 활성화 여부")]
    [SerializeField] UISetting uiSetting;

    // 입력 버튼 이름
    private string dodgeButtonName = "Jump";

    public Vector3 moveVec2; // 구르기용 벡터
    public static Camera mainCamera;

    private PlayerKeyboardController playerKeyboardController;
    private PlayerEquipmentManager playerEquipmentManager;
    private PlayerAnimation playerAnimation;
    private PlayerInfo playerInfo;
    private PlayerKeyboardInput playerKeyboardInput;

    public Ray ray;
    public float moveSpeed = 7.0f;

    public bool isShoot = false; // 공격중
    public bool isRight = false; // 우클릭 공격중
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
    
    private void Start()
    {
        playerKeyboardController = gameObject.GetComponent<PlayerKeyboardController>();
        playerEquipmentManager = gameObject.GetComponent<PlayerEquipmentManager>();
        playerInfo = gameObject.GetComponent<PlayerInfo>();
        playerAnimation = gameObject.GetComponent<PlayerAnimation>();
        playerKeyboardInput = gameObject.GetComponent<PlayerKeyboardInput>();
        playerAnimation.ChangeAnimator();
        moveVec2 = transform.forward;
        mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        if(PlayerKeyboardController.isInteraction) return;
        if(GameManager.isMulti && !photonView.IsMine) return;
        InputDodge();
        InputMove();
    }

    private void Update()
    {
        if (GameManager.isMulti && !photonView.IsMine) return;
        InputEscape();
        Attack();
        Interation();
        RightAttack();
        SwapInput();
        StateCheck();
        CcCheck();
        //Grenade();
    }

    public void InputMove()
    {
        if (playerKeyboardController.pState == PlayerKeyboardController.PlayerState.Dodge || playerKeyboardController.pState == PlayerKeyboardController.PlayerState.Death || playerKeyboardController.pState == PlayerKeyboardController.PlayerState.Attack || isSwap == true || onHit == true || playerKeyboardController.pState == PlayerKeyboardController.PlayerState.onCC || isRight || isChange)
            return;
        playerKeyboardController.hAxis = Input.GetAxisRaw("Horizontal");
        playerKeyboardController.vAxis = Input.GetAxisRaw("Vertical");

        playerKeyboardController.Move();
    }

    public void InputDodge()
    {
        if (Input.GetButton(dodgeButtonName) && playerKeyboardController.pState != PlayerKeyboardController.PlayerState.Dodge)
        {
            if (playerKeyboardController.hAxis != 0 || playerKeyboardController.vAxis != 0)
            {
                Vector3 heading = mainCamera.transform.localRotation * Vector3.forward;
                heading = Vector3.Scale(heading, new Vector3(1, 0, 1)).normalized;
                moveVec2 = heading * Time.fixedDeltaTime * Input.GetAxisRaw("Vertical") * moveSpeed;
                moveVec2 += Quaternion.Euler(0, 90, 0) * heading * Time.fixedDeltaTime * Input.GetAxisRaw("Horizontal") * moveSpeed;
            }
            else if (playerKeyboardController.hAxis == 0 && playerKeyboardController.vAxis == 0)
            {
                moveVec2 = transform.forward;
            }
            transform.LookAt(transform.position + moveVec2);
            playerKeyboardController.Dodge(moveVec2);
        }
    }

    public void Attack()
    {
        if (Input.GetMouseButtonDown(0) && !isShoot && playerKeyboardController.pState != PlayerKeyboardController.PlayerState.RIghtAttack && playerEquipmentManager.equipWeapon != null && isSwap == false)
        {
            RaycastHit hit;
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << LayerMask.NameToLayer("Ground");
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Vector3 destination = new Vector3(hit.point.x, gameObject.transform.position.y, hit.point.z);
                playerKeyboardController.Attack(destination, delay, animSpeed);
            }
        }
    }

    public void RightAttack()
    {
        if (Input.GetMouseButtonDown(1) && !isRight && playerKeyboardController.pState != PlayerKeyboardController.PlayerState.Attack && playerEquipmentManager.equipWeapon != null && isSwap == false)
        {
            RaycastHit hit;
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 destination = new Vector3(hit.point.x, gameObject.transform.position.y, hit.point.z);
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

        if (playerKeyboardController.pState != PlayerKeyboardController.PlayerState.Attack && !playerKeyboardInput.isDodge && !playerKeyboardInput.isSwap)
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
            playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Swap;
        else if (playerKeyboardController.pState == PlayerKeyboardController.PlayerState.Swap && playerKeyboardInput.isSwap == false)
            playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Idle;

        if (isDodge == true)
        {
            playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Dodge;
            playerInfo.canDamage = false;
        }
        else if (playerKeyboardController.pState == PlayerKeyboardController.PlayerState.Dodge && playerKeyboardInput.isDodge == false)
        {
            playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Idle;
            playerInfo.canDamage = true;
        }

        if (isRight == true)
            playerKeyboardController.pState = PlayerKeyboardController.PlayerState.RIghtAttack;
        else if (playerKeyboardController.pState == PlayerKeyboardController.PlayerState.RIghtAttack && playerKeyboardInput.isRight == false)
            playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Idle;
     
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
        if(onHit == true)
        {
            playerKeyboardController.pState = PlayerKeyboardController.PlayerState.onHit;
        }
        else if(playerKeyboardController.pState == PlayerKeyboardController.PlayerState.onHit && onHit == false)
        {
            playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Idle;
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

   /* public void Grenade()
    {
        if (Input.GetButtonDown("Grenade") && !isRight && playerKeyboardController.pState != PlayerKeyboardController.PlayerState.Attack && isSwap == false)
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
