using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCheck : MonoBehaviour
{
    public LayerMask targetLayer; // 공격 대상 레이어
    public float fRange; // 수색범위

    private void Update()
    {
        FindNearEnemy(targetLayer);
    }

    void FindNearEnemy(LayerMask tlayer) // 가장 가까운 대상 찾기
    {

        Collider[] colliders = Physics.OverlapSphere(this.transform.position, fRange, tlayer);//콜라이더 설정하기
        Collider colliderMin = null; // 가장가까운 대상의 콜라이더
        float fPreDist = 99999999.0f; // 가장가까운 대상 거리 float값

        //찾은대상중 가장 가까운 대상을 찾는다.
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            float fDist = Vector3.Distance(collider.transform.position, this.transform.position);
            //콜라이더를 통해 찾은 타겟과의 거리를 float값으로 계산

            if (colliderMin == null || fPreDist > fDist) // 조건문으로 가장 가까운 대상 찾기
                colliderMin = collider;
            fPreDist = fDist;

        }

        if (colliderMin != null) //콜라이더가 비어있지 않으면
        {
            LivingEntity livingEntity = colliderMin.GetComponent<LivingEntity>();


            if (livingEntity != null && !livingEntity.dead) //찾은 리빙엔티티가 죽지않고 null값이 아닐때
            {
                gameObject.SendMessageUpwards("OnSetTarget", livingEntity, SendMessageOptions.DontRequireReceiver);
            }
        }


    }

    private void OnDrawGizmos() // 범위 그리기
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, fRange);
    }
}
