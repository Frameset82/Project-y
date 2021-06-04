using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoRotation : MonoBehaviour
{
    [SerializeField] Transform logoBase;
    [SerializeField] float rotateSpeed = 100.0f;

    void Update()
    {
        logoBase.Rotate(new Vector3(0,0, rotateSpeed * Time.deltaTime));
    }
}
