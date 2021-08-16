using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerAnimation playerAnimation;
    private PlayerEquipmentManager playerEquipmentManager;

    private void Start()
    {
        playerController = this.GetComponentInParent<PlayerController>();
        playerAnimation = this.GetComponentInParent<PlayerAnimation>();
        playerEquipmentManager = this.GetComponentInParent<PlayerEquipmentManager>();
    }

    public void Combo1Check()
    {
        playerController.pState = PlayerController.PlayerState.Idle;
        if (playerController.comboCnt == 1)
        {
            playerAnimation.playerAnimator.SetBool("isAttack", false);
            playerController.comboCnt = 0;
            playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
            playerController.gameObject.GetComponent<PlayerInput>().isBasicAttacking = false;
        }
    }

    public void Combo2Check()
    {
        playerController.pState = PlayerController.PlayerState.Idle;
        if (playerController.comboCnt == 2)
        {
            playerAnimation.playerAnimator.SetBool("isAttack", false);
            playerController.comboCnt = 0;
            playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
            playerController.gameObject.GetComponent<PlayerInput>().isBasicAttacking = false;
        }
    }

    public void Combo3Check()
    {
        playerController.comboCnt = 0;
        playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
        playerController.pState = PlayerController.PlayerState.Idle;
        playerAnimation.playerAnimator.SetBool("isAttack", false);
        playerController.gameObject.GetComponent<PlayerInput>().isBasicAttacking = false;

    }

    public void ComboMoveCheck()
    {
        playerController.ComboMove();
    }

    public void Atk()
    {
        playerEquipmentManager.equipWeapon.OnAttack();
    }
    public void Atk2()
    {
        playerEquipmentManager.equipWeapon.OnActive();
    }
}
