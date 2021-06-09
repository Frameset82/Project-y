using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    private PlayerKeyboardController playerKeyboardController;
    private PlayerAnimation playerAnimation;

    private void Start()
    {
        playerKeyboardController = this.GetComponentInParent<PlayerKeyboardController>();
        playerAnimation = this.GetComponentInParent<PlayerAnimation>();
    }

    public void Combo1Check()
    {
        playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Idle;
        if (playerKeyboardController.comboCnt == 1)
        {
            playerAnimation.playerAnimator.SetBool("isAttack", false);
            playerKeyboardController.comboCnt = 0;
            playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
            playerKeyboardController.gameObject.GetComponent<PlayerKeyboardInput>().isShoot = false;
        }
    }
    public void Combo2Check()
    {
        playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Idle;
        if (playerKeyboardController.comboCnt == 2)
        {
            playerAnimation.playerAnimator.SetBool("isAttack", false);
            playerKeyboardController.comboCnt = 0;
            playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
            playerKeyboardController.gameObject.GetComponent<PlayerKeyboardInput>().isShoot = false;
        }
    }
    public void Combo3Check()
    {
        playerKeyboardController.comboCnt = 0;
        playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
        playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Idle;
        playerAnimation.playerAnimator.SetBool("isAttack", false);
        playerKeyboardController.gameObject.GetComponent<PlayerKeyboardInput>().isShoot = false;

    }

    public void ComboMoveCheck()
    {
        playerKeyboardController.ComboMove();
    }
}
