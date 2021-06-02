using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraSetup : MonoBehaviourPun
{
    GameObject[] players;
    [SerializeField] GameObject target;

    [SerializeField] float offsetX = 7.0f;
    [SerializeField] float offsetY = 9.0f;
    [SerializeField] float offsetZ = 7.0f;

    PhotonView pv;

    public float DelayTime = 5.0f;

    void FixedUpdate()
    {
        if (target == null) return;
        Vector3 FixedPos =
            new Vector3(
            target.transform.position.x + offsetX,
            target.transform.position.y + offsetY,
            target.transform.position.z + offsetZ);
        transform.position = Vector3.Slerp(transform.position, FixedPos, Time.deltaTime * DelayTime);
    }

    public void Targeting()
    {
        if (GameManager.isMulti)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<PhotonView>().IsMine)
                {
                    target = players[i];
                }
            }
        }
        else
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }
    }
}
