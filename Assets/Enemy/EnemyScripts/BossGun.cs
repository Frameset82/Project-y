using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGun : EnemyGun
{
    private void Awake()
    {
        muzzleFlashEffect.SetActive(false);
        bulletLineRenderer = GetComponent<LineRenderer>();
        bulletLineRenderer.positionCount = 2;
        bulletLineRenderer.enabled = false;
    }

    public void Fire(Damage damage, float fireDistance)
    {
        base.Fire(damage, fireDistance);
    }
}
