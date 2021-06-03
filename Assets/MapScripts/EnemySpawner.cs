using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemySpawner : MonoBehaviour
{
    
    public float fRange; // 수색범위
    public int random;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            MopSpawm();
        }
    }

    void MopSpawm()
    {

        // for(int i = 0; i<3; i++)
        // {

        //     random = Random.Range(0, 3);

        
        //     Vector3 spawnPos = Random.insideUnitCircle * fRange; 
        //     spawnPos.x += this.transform.position.x;
        //     spawnPos.z = spawnPos.y + this.transform.position.z;
        //     spawnPos.y = this.transform.position.y;

        //     switch(random)
        //     {
        //         case 0:
        //             MeleeController mcon = ObjectPool.GetMelee();
        //             mcon.transform.position = spawnPos;
        //             mcon.gameObject.SetActive(true);
        //         break;
        //         case 1:
        //             RogueController rcon = ObjectPool.GetRogue();
        //             rcon.transform.position = spawnPos;
        //             rcon.gameObject.SetActive(true);
        //         break;
        //         case 2:
        //             RifleController ricon = ObjectPool.GetRifle();
        //             ricon.transform.position = spawnPos;
        //             ricon.gameObject.SetActive(true);
        //         break;
        //     }

        // }
      
    }

    private void OnDrawGizmos() // 범위 그리기
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, fRange);
    }
}
