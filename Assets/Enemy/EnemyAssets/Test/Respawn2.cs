using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn2 : MonoBehaviour
{
    public GameObject theEnemy;
    public GameObject explosion;
    public int xPos; // x값
    public int zPos; // z값
    public int enemycount; // 적 추가
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnemmyDrop()); // 코루틴 실행
    }
    IEnumerator EnemmyDrop() //적 생성 코드
    {
        while( enemycount <50) // 몇 마리 생성 할껀지
        {
            xPos = Random.Range(1, 35); // x값(1 ~35에 랜덤으로 리스폰)
            zPos = Random.Range(1, 31); //z값 (1~ 38에 랜덤으로 리스폰) (맵에 따라 따로 지정 필요)
            Instantiate(explosion, new Vector3(xPos, 2, zPos), Quaternion.identity);
            Instantiate(theEnemy, new Vector3(xPos, 2, zPos), Quaternion.identity); // enemy 생성 코드
            yield return new WaitForSeconds(0.1f);
            enemycount += 1; // 적을 한명 추가

        }
    }
}
