using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{

    public void Combo1Check()
    {
        PlayerKeyboardInput.playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Idle;
        if (PlayerKeyboardController.comboCnt == 1)
        {
            PlayerKeyboardInput.playerAnimation.playerAnimator.SetBool("isAttack", false);
            PlayerKeyboardController.comboCnt = 0;
            PlayerKeyboardInput.playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
        }
    }
    public void Combo2Check()
    {
        PlayerKeyboardInput.playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Idle;
        if (PlayerKeyboardController.comboCnt == 2)
        {
            PlayerKeyboardInput.playerAnimation.playerAnimator.SetBool("isAttack", false);
            PlayerKeyboardController.comboCnt = 0;
            PlayerKeyboardInput.playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
        }
    }
    public void Combo3Check()
    {
        PlayerKeyboardInput.playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Idle;
        PlayerKeyboardInput.playerAnimation.playerAnimator.SetBool("isAttack", false);
        PlayerKeyboardController.comboCnt = 0;
        PlayerKeyboardInput.playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
    }

    public void ComboMoveCheck()
    {
        PlayerKeyboardInput.playerKeyboardController.ComboMove();
    }
}
