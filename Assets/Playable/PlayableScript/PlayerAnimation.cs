using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    public Animator playerAnimator; // 캐릭터 애니메이터
    private PlayerEquipmentManager playerEquipmentManager;
    private keyboardController keyboardController;

    private void Start()
    {
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        keyboardController = GetComponent<keyboardController>();
    }
    void Update()
    {
        playerAnimator = gameObject.GetComponentInChildren<Animator>();
    }

    // 캐릭터 선자세 애니메이션
    public void IdleAni()
    {

    }
    // 캐릭터 이동 애니메이션
    public void MoveAni(bool isMove)
    {
        playerAnimator.SetBool("isMove", isMove);
    }
    // 캐릭터 회피 애니메이션
    public void DodgeAni()
    {
        playerAnimator.SetTrigger("dodge");
    }
    // 캐릭터 라이플 사격 애니메이션
    public void Attack()
    {
        playerAnimator.SetTrigger("Attack");
        playerAnimator.SetBool("isMove", false);
    }

    public void Dead()
    {
        playerAnimator.SetTrigger("isDeath");
    }
    public void Swap()
    {
        playerAnimator.SetTrigger("Swap");
        keyboardController.isSwap = true;
    }

    public bool CompareStateName(string aaa)
    {
        return
        playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(aaa);
    }
}
