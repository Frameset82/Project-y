using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityItem : ActiveItem
{
    public override void OnEquip()
    {
        playerInfo.maxHealth += 80f;
        playerKeyboardInput.animSpeed += 0.1f; // 애니메이션 스피드 0.1 증가
        playerAnimation.playerAnimator.SetFloat("AttackSpeed", playerKeyboardInput.animSpeed);
        playerInfo.CalculateHealthPoint();
    }

    public override void UnEquip()
    {
        playerInfo.maxHealth -= 100f;
        playerKeyboardInput.animSpeed -= 0.1f;
        playerAnimation.playerAnimator.SetFloat("AttackSpeed", playerKeyboardInput.animSpeed);
        playerInfo.CalculateHealthPoint();
    }
}
