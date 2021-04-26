using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKControl : MonoBehaviour
{
    //public Transform gunPivot; // 총 배치의 기준점
    public Transform leftHandMount; // 총의 왼쪽 손잡이, 왼손이 위치할 지점
    public Transform rightHandMount; // 오른손으로 총 잡는 지점
    public Animator playerAnimator; // 애니메이터 컴포넌트

 /*   public Transform swordPivot;
    public Transform sleftHandMount;
    public Transform srightHandMount;*/

    void Start () 
    {
        playerAnimator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        
    }

    void OnAnimatorIK(int layerIndex) {
        // IK를 사용하여 왼손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);
        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMount.rotation);

        /*playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, sleftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, sleftHandMount.rotation);
        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, srightHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, srightHandMount.rotation);*/
    }
}
