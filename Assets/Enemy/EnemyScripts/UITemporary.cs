using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITemporary : MonoBehaviour {
    public float maxPoint;
    public float currentPoint;
    
    public GameObject barObject;
    public Material material;

    void Start() {
        barObject.GetComponent<Image>().material = new Material(material);
        RefreshUI();
    }
    void RefreshUI() {
        barObject.GetComponent<Image>().material.SetFloat("Percent", (currentPoint/maxPoint) * 100);
        print((currentPoint/maxPoint) * 100);
    }

    void Update() {
        if(Input.GetKey(KeyCode.UpArrow)) {
            currentPoint = Mathf.Min(currentPoint + 10, 100);
            RefreshUI();
        }
        if(Input.GetKey(KeyCode.DownArrow)) {
            currentPoint = Mathf.Max(currentPoint - 10, 0);
            RefreshUI();
        }
    }
}
