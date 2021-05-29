using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveItem : MonoBehaviour, IDamageSource
{
    private GameObject player; 
    public Sprite ItemSprite; // 아이템 스프라이트

    public void OnAttack()
    {
        throw new System.NotImplementedException();
    }

    void Awake()
    {
        player = GameObject.Find("Player");
    }
}
