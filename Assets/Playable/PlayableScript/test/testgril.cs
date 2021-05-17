using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testgril : LivingEntity
{

    // Start is called before the first frame update
    void Start()
    {
        health = 100f;
        startingHealth = 100f;
        
    }

    // Update is called once per frame
    void Update()
    {
        print(gameObject.name + "의 HP" +
            health);
    }

    public override void OnDamage(Damage dInfo)
    {
        health -= dInfo.dValue; //체력 감소
    }
}
