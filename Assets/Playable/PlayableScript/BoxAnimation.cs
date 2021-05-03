using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAnimation : MonoBehaviour
{
    public Animator BoxAnimator; // 박스 애니메이터


    public void boxopen()
    {
        BoxAnimator.SetTrigger("Open");
    }
}
