using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    private PlayerKeyboardController playerKeyboardController;
    private PlayerAnimation playerAnimation;
    private GameObject player;

    public void Start()
    {
        playerKeyboardController = GameObject.Find("Player").GetComponent<PlayerKeyboardController>();
        playerAnimation = GameObject.Find("Player").GetComponent<PlayerAnimation>();
        player = GameObject.Find("Player");
    }

    public void Combo1Check()
    {
        playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Idle;
        if (PlayerKeyboardController.comboCnt == 1)
        {
            playerAnimation.playerAnimator.SetBool("isAttack", false);
            PlayerKeyboardController.comboCnt = 0;
            playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
        }
    }
    public void Combo2Check()
    {
        playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Idle;
        if (PlayerKeyboardController.comboCnt == 2)
        {
            playerAnimation.playerAnimator.SetBool("isAttack", false);
            PlayerKeyboardController.comboCnt = 0;
            playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
        }
    }
    public void Combo3Check()
    {
        playerKeyboardController.pState = PlayerKeyboardController.PlayerState.Idle;
        playerAnimation.playerAnimator.SetBool("isAttack", false);
        PlayerKeyboardController.comboCnt = 0;
        playerAnimation.playerAnimator.SetInteger("ComboCnt", 0);
    }

    public void ComboMoveCheck()
    {
        playerKeyboardController.ComboMove();
    }
}
