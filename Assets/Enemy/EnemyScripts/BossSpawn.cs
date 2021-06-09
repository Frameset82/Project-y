using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class BossSpawn : MonoBehaviour
{
    public BossController Boss;
    public CameraSetup cam;
    private bool first;

    void Start()
    {

        first = true;
    }

    IEnumerator StartBoss()
    {
        Boss.startRoutine();
        cam.BossTargeting();

        yield return new WaitForSeconds(3f);

        cam.Targeting();
        SoundManager.instance.BgmPlay();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && PhotonNetwork.IsMasterClient  && first)
        {
            StartCoroutine(StartBoss());
            SoundManager.instance.BgmPlay();
            first = false;
        }
    }
}
