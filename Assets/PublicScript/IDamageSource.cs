using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 데미지를 주는 모든 오브젝트가 가져야할 인터페이스
public interface IDamageSource
{
    void OnAttack();
    //void OnKill();
}