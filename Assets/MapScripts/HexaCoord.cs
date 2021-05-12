using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HexaCoord
{
    Vector3 coord;
    public HexaCoord(Vector3 coord) {
        this.coord = coord;
    }
    public Vector3 GetHexaCoord(){
        return coord;
    }
    public Vector3 GetPosition(){
        Vector3 pos = new Vector3(
            /* pos.x */ (-coord.x * 0.5f) + (coord.y * 0.5f),
            /* pos.y */ 0,
            /* pos.z */ (-coord.x / (Mathf.Sqrt(3)*2)) + (-coord.y / (Mathf.Sqrt(3)*2)) + (coord.z / Mathf.Sqrt(3))
        );
        return pos;
    }
}
