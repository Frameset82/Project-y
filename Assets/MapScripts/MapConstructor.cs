using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapConstructor : MonoBehaviour
{
    public Object tilePrefab;
    GameObject tileParents;
    ArrayList tiles = new ArrayList();
    float sideLength = Mathf.Sqrt(3);
    int fieldRadius = 50;

    void Start() {
        tileParents = GameObject.Find("Tiles");

        for(int x = -fieldRadius; x <= fieldRadius; x ++) {
            for(int y = -fieldRadius; y <= fieldRadius; y++) {
                for(int z = -fieldRadius; z <= fieldRadius; z++) {
                    if(x+y+z==0){
                        HexaCoord hexaCoord = new HexaCoord(new Vector3(x, y, z));
                        GameObject newTile = Instantiate(
                            tilePrefab, hexaCoord.GetPosition(),
                             Quaternion.identity) as GameObject;
                        newTile.transform.parent = tileParents.transform;
                        newTile.GetComponent<Hexagon>().coord = hexaCoord;
                        // newTile
                        tiles.Add(newTile);
                    }
                }
            }
        }
        StartCoroutine(CreateBoxCoroutine());
    }
    
    void Update() {
        
    }

    IEnumerator CreateBoxCoroutine() {
        while(true){
            yield return new WaitForSeconds(3);
        }
    }
}
