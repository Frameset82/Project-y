using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    private keyboardController keyboardController;
    private PlayerAnimation playerAnimation;

    public void Start()
    {
        keyboardController = GameObject.Find("Player").GetComponent<keyboardController>();
        playerAnimation = GameObject.Find("Player").GetComponent<PlayerAnimation>();
    }

    public void Combo1Check()
    {
        keyboardController.pState = keyboardController.PlayerState.Idle;
        if (keyboardController.comboCnt == 1)
        {
            playerAnimation.playerAnimator.SetBool("isAttack", false);
            keyboardController.comboCnt = 0;
            playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
        }
    }
    public void Combo2Check()
    {
        keyboardController.pState = keyboardController.PlayerState.Idle;
        if (keyboardController.comboCnt == 2)
        {
            playerAnimation.playerAnimator.SetBool("isAttack", false);
            keyboardController.comboCnt = 0;
            playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
        }
    }
    public void Combo3Check()
    {
        keyboardController.pState = keyboardController.PlayerState.Idle;
        playerAnimation.playerAnimator.SetBool("isAttack", false);
        keyboardController.comboCnt = 0;
        playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
    }
}
