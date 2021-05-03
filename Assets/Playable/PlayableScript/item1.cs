using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item1 : MonoBehaviour
{
    public enum Type { Ammo, Weapon, Coin, Heart, Granade };
    public Type type;
    public int value;

    void Update()
    {
        transform.Rotate(Vector3.up * 100 * Time.deltaTime);
    }
}
