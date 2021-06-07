using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedPiece : MonoBehaviour
{
    DestroyableObj parent;
    public float power = 100f;
    void Start() {
        parent = transform.parent.GetComponent<DestroyableObj>();
        parent.breakDelegate += BreakObject;
    }

    void BreakObject(float power) {
        GetComponent<Rigidbody>().isKinematic = false;
        DeleteObj();
    }

    void DeleteObj(){
        StartCoroutine(FadeOut(1.5f));
    }
    IEnumerator FadeOut(float transition) {
        yield return new WaitForSeconds(1.5f);
        float coef = 0;

        Material material = GetComponent<Renderer>().material;
        GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        material.SetInt("_Outline_Width", 0);
        material.SetInt("_TransparentEnabled", 1);
        material.renderQueue = 3000;
        
        while(coef < transition) {
            coef += Time.unscaledDeltaTime;
            material.SetFloat("_Tweak_transparency", Mathf.Max(-1, Mathf.Lerp(0, -1, coef / transition)));
            yield return null;
        }
    }
}
