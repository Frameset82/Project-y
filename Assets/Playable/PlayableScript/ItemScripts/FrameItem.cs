using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameItem : ActiveItem

{
    private Damage damage; // Damgae 스크립트
    // Start is called before the first frame update
    public override void OnEquip()
    {
        playerInput.moveSpeed += 3f;
        /*        playerInfo.maxHealth += 80f;*/
        /* playerInfo.MoveSpeed += 0.3f; // */
        damage.dValue += 100f;
      /*  playerInfo.CalculateHealthPoint();*/
    }

    public override void UnEquip()
    {
        playerInput.moveSpeed -= 3f;
        /*        playerInfo.maxHealth -= 80f;*/
        /*        playerInfo.MoveSpeed -= 0.3f;*/
        damage.dValue -= 100f;
        /*   playerInfo.CalculateHealthPoint();*/
    }
}
