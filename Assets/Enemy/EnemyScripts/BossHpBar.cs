using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHpBar : MonoBehaviour
{
    public float maxPoint;
    public float currentPoint;

    public GameObject barObject;
    public Material material;

    void Start()
    {
        barObject.GetComponent<Image>().material = new Material(material);
        RefreshUI(currentPoint);
    }
    public void RefreshUI(float _currPoint)
    {
        barObject.GetComponent<Image>().material.SetFloat("Percent", (_currPoint / maxPoint) * 100);
        
    }

    void Update()
    {
     
    }
}
