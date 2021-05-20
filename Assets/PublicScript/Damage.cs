using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Damage
{
    public enum DamageType { None = 0, Melee = 1, Stun = 2, NuckBack = 3, End } //공격타입
    public DamageType dType; 
    public float dValue; //데미지 수치
    public float ccTime; //cc기 지속시간
    public float inCapValue; //무력화 수치
    public Vector3 hitPoint; //타격지점
    public Vector3 hitNormal; // 타격 방향

}
