using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boomTimer : MonoBehaviour
{
        public float Time = 0.5f;

        // Start is called before the first frame update
        void Start()
        {
            //Time 후 Object 삭제
            Destroy(gameObject, 1.0f);
        }
    

}
