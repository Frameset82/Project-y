using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    public Animator playerAnimator; // 캐릭터 애니메이터
    private PlayerInput playerKeyboardInput;
    public RuntimeAnimatorController[] anim = new RuntimeAnimatorController[4]; // 0 검 1 밀리 2 창 3 라이플

    private void Start()
    {
        playerKeyboardInput = gameObject.GetComponent<PlayerInput>();
        playerAnimator = gameObject.GetComponentInChildren<Animator>();
    }

    public void ChangeAnimator()
    {
        playerAnimator = gameObject.GetComponentInChildren<Animator>();
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

    public void Grenade()
    {
        playerAnimator.SetTrigger("Grenade");
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
        playerKeyboardInput.isSwap = true;
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
