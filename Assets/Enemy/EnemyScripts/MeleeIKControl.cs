using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeIKControl : MonoBehaviour
{
    public Transform leftHandMount;

    private Animator EnemyAnimator;


    void Start()
    {
        EnemyAnimator = GetComponent<Animator>();
    }

    
    void Update()
    {
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        EnemyAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        EnemyAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        EnemyAnimator.SetIKPosition(AvatarIKGoal.RightHand, leftHandMount.position);
        EnemyAnimator.SetIKRotation(AvatarIKGoal.RightHand, leftHandMount.rotation);
    }
}
