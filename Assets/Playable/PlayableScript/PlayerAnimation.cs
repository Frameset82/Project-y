using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class PlayerAnimation : MonoBehaviour
{

    public Animator playerAnimator; // 캐릭터 애니메이터
    public AnimatorController[] playerAnimators;
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

        if(playerEquipmentManager.equipWeapon == null)
        {
            playerAnimator.runtimeAnimatorController = (RuntimeAnimatorController)playerAnimators[0];
        }
        else if(playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isRifle == true)
        {
            playerAnimator.runtimeAnimatorController = (RuntimeAnimatorController)playerAnimators[1];
        }
        else if(playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isGun == true)
        {
            playerAnimator.runtimeAnimatorController = (RuntimeAnimatorController)playerAnimators[2];
        }
        else if(playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isMelee == true)
        {
            playerAnimator.runtimeAnimatorController = (RuntimeAnimatorController)playerAnimators[3];
        }
        else if(playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isSpear == true)
        {
            playerAnimator.runtimeAnimatorController = (RuntimeAnimatorController)playerAnimators[4];
        }
        else if (playerEquipmentManager.equipWeapon.GetComponent<Weapon>().isSword == true)
        {
            playerAnimator.runtimeAnimatorController = (RuntimeAnimatorController)playerAnimators[5];
        }
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
           
        
    }
    public void Combo1Check()
    {
        if (keyboardController.comboCnt == 1)
        {
            playerAnimator.SetBool("isAttack", false);
            keyboardController.comboCnt = 0;
        }
    }

    public bool CompareStateName(string aaa)
    {
        return
        playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(aaa);
    }
}
