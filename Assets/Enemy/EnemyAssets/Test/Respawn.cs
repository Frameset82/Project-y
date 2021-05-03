using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{

    [Header("적생성")] //굵은 글씨체로 인스펙터(Inspector) 창에 보이게 된다.
    [SerializeField] GameObject enemy; // private 변수를 inspector 로 접근가능하는게 [SerializeField], enemy 지정
    [SerializeField] Transform[] createnemy; // enemy가 나올 장소를 배열로 지정
    [SerializeField] float creat_time; // 젠 시간
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("creat", 0, creat_time);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void creat()
    {
        int i = Random.Range(0, 3); // 배열 값 지정(리스폰 되는 장소 갯수)
        Instantiate(enemy, createnemy[i].position, createnemy[i].rotation); // enemy 생성 코드
    }



}