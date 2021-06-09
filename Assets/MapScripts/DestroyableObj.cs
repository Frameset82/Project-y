using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class DestroyableObj : LivingEntity
{
    public delegate void BreakDelegate(float power);
    public BreakDelegate breakDelegate;
    PhotonView pv;

    bool isBroken = false;

    void Start() {
        pv = GetComponent<PhotonView>();
    }

    public void OnDamage(Damage damage) {
        if(isBroken) return; // 이미 파괴되어 있다면 반환

        if(GameManager.isMulti) // 멀티플레이 중일 경우 RPC 호출
            pv.RPC("OnBreak", RpcTarget.All, damage.dValue);
        else // 솔로플레이 중일 경우 메소드 호출
            OnBreak(damage.dValue);
    }
    [PunRPC]
    public void OnBreak(float power) {
        isBroken = true;
        breakDelegate(power);
        Destroy(gameObject, 3f); // 오브젝트 제거
        GetComponent<BoxCollider>().enabled = false;
    }
    void Update() {
        if(Input.GetKey(KeyCode.B)){
            OnDamage(new Damage());
        }
    }
}

