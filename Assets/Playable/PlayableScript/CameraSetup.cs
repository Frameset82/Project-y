using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraSetup : MonoBehaviourPun
{
    public GameObject target;
    [SerializeField] float offsetX = 7.0f;
    [SerializeField] float offsetY = 9.0f;
    [SerializeField] float offsetZ = 7.0f;

    public float DelayTime = 5.0f;

    void FixedUpdate()
    {
        Vector3 FixedPos =
            new Vector3(
            target.transform.position.x + offsetX,
            target.transform.position.y + offsetY,
            target.transform.position.z + offsetZ);
        transform.position = Vector3.Slerp(transform.position, FixedPos, Time.deltaTime * DelayTime);
    }
}
