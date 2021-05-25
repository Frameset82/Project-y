using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    public Animator playerAnimator; // 캐릭터 애니메이터
    private PlayerEquipmentManager playerEquipmentManager;
    private PlayerKeyboardController playerKeyboardController;

    private void Start()
    {
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerKeyboardController = GetComponent<PlayerKeyboardController>();
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
    // 캐릭터 공격 애니메이션
    public void Attack()
    {
        playerAnimator.SetTrigger("Attack");
        playerAnimator.SetBool("isMove", false);
    }

    public void RightAttack()
    {
        playerAnimator.SetTrigger("RightAttack");
        playerAnimator.SetBool("isMove", false);
    }

    public void Dead()
    {
        print("사망");
        playerAnimator.SetTrigger("isDeath");
    }
    public void Swap()
    {
        playerAnimator.SetTrigger("Swap");
        PlayerKeyboardInput.isSwap = true;
    }

    public void OnHit()
    {
        playerAnimator.SetTrigger("OnHit");
    }

    public void OnNuckBack()
    {
        playerAnimator.SetTrigger("OnNuckBack");
    }

    public void OnStun()
    {
        playerAnimator.SetTrigger("OnStun");
    }

    public bool CompareStateName(string aaa)
    {
        return
        playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(aaa);
    }
}
