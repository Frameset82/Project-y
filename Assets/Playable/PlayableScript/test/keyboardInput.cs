using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyboardInput : MonoBehaviour
{
    // 입력 버튼 이름
    private string dodgeButtonName = "Jump";

    public float speed = 3.0f;
    float hAxis;
    float vAxis;

    Vector3 moveVec; // 움직임 벡터
    public static Vector3 moveVec1; // 상태 초기화용 벡터
    private Rigidbody rigi;
    private keyboardController keyboardController;
    public Ray ray;
    public Camera mainCamera;
    public Animator avater;
    private PlayerEquipmentManager playerEquipmentManager;

    public static bool isShoot = false;
    void Start()
    {
        rigi = GetComponent<Rigidbody>();
        keyboardController = GetComponent<keyboardController>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
    }
    void FixedUpdate()
    {
        InputMove();
        InputDodge();
    }

    private void Update()
    {
        InputRifleShoot();
    }

    public void InputMove()
    {
        if (keyboardController.pState == keyboardController.PlayerState.Dodge || keyboardController.pState == keyboardController.PlayerState.Death || keyboardController.pState == keyboardController.PlayerState.Attack)
            return;
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            keyboardController.Move();
            avater.SetBool("isMove", true);
        }
        else
        {
            keyboardController.unMove();
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
            keyboardController.Dodge();
            transform.LookAt(transform.position + moveVec1);
        }
    }

    public void InputRifleShoot()
    {
        if (Input.GetMouseButtonDown(0) && !isShoot && playerEquipmentManager.equipWeapon != null)
        {
            RaycastHit hit;
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "Player") return;
                Vector3 destination = new Vector3(hit.point.x, 0, hit.point.z);
                keyboardController.Attack(destination);
            }

        }
    }
}
