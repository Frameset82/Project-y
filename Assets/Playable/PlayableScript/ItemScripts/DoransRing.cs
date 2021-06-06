using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoransRing : ActiveItem
{
    public override void OnEquip()
    {
        playerInfo.defaultDamage += 8f;
        playerInfo.maxHealth += 20f;
        playerKeyboardInput.animSpeed += 0.1f;
        playerAnimation.playerAnimator.SetFloat("AttackSpeed", playerKeyboardInput.animSpeed);
    }

    public override void UnEquip()
    {
        playerInfo.defaultDamage -= 8f;
        playerInfo.maxHealth -= 20f;
        playerKeyboardInput.animSpeed -= 0.1f;
        playerAnimation.playerAnimator.SetFloat("AttackSpeed", playerKeyboardInput.animSpeed);
    }
}
