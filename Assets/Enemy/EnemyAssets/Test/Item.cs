﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
 

    public enum Type { Weapon, Grenade, Heart}
    public Type type;
    public int value;


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 80 * Time.deltaTime);
    }
}
