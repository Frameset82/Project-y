using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBarScript : MonoBehaviour
{
    [SerializeField] GameObject m_goPrefab = null; //SerializeField  는 public 이나 익스펙터 창에는 나타나지 않을려는 용도로 사용


    List<Transform> m_objectList = new List<Transform>(); // 몬스터 위치를 담을 리스트 선언
    List<GameObject> m_hpBarList = new List<GameObject>(); //Hp바 리스트 선언

    Camera m_cam = null;
    // Start is called before the first frame update
    //https://www.youtube.com/watch?v=jY49bG8Z7KQ
    void Start()
    {

        m_cam = Camera.main;

        GameObject[] t_objects = GameObject.FindGameObjectsWithTag("Enemy"); // 특정 태그가 "Enemy"인 경우에 배열에 저장함
        for (int i =0; i< t_objects.Length; i++) // 
        {
            m_objectList.Add(t_objects[i].transform);  // 배열의 갯수만큼 몬스터 위치 리스트에 추가한다.
            GameObject t_hpbar =  Instantiate(m_goPrefab, t_objects[i].transform.position,Quaternion.identity, transform); // 몬스터 위치에 HP 프리팹 생성
            m_hpBarList.Add(t_hpbar); //생성된 hb바를 hb 리스트에 추가

        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0; i<m_objectList.Count; i++)
        {
            m_hpBarList[i].transform.position = m_cam.WorldToScreenPoint(m_objectList[i].position + new Vector3(0, 2.0f,0)); // hp바가 몬스터 머리위로 따라다니게 위치 표현
        }
    }
}
