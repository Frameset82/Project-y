using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    public HexaCoord coord;
    GameObject player;
    Material tileRenderer;

    public void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        tileRenderer = GetComponent<Renderer>().material;
    }

    public void Update() {

        Color newColor = new Color(tileRenderer.color.r, tileRenderer.color.g, tileRenderer.color.b, 0);
        tileRenderer.SetColor(Shader.PropertyToID("_Color"), newColor);
        tileRenderer.SetFloat("_Metallic", Mathf.Max(.222f, Mathf.Min(.777f, 1f-Vector3.Distance(player.transform.position, transform.position)/4)));
    }

    public void SetPosition() {
        transform.position = coord.GetPosition();
    }
}
