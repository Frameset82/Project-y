using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunChase : MonoBehaviour
{
    public Transform gunPivot; // 총 배치의 기준점
    public Transform leftHandMount; // 총의 왼쪽 손잡이, 왼손이 위치할 지점

    private Animator playerAnimator; // 애니메이터 컴포넌트

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        // IK를 사용하여 왼손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);
    }
}
