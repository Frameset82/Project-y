using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolDownScript : MonoBehaviour
{
    [SerializeField]
    private Image imageCoolDown; //쿨타임시 보일 이미지

    [SerializeField]
    private Text textCoolDown; //쿨타임 텍스트
  

    private bool isCooldown = false; //쿨타임 중인지
    private float coolDownTime = 10f; //쿨타임 변수
    private float timer = 0f; //타이머

    void Start()
    {
   
        textCoolDown.gameObject.SetActive(false);
        imageCoolDown.fillAmount = 0.0f;
    }


    void Update()
    {
        if (isCooldown)
        {
            ApplyCoolDown();
        }
    }

    void ApplyCoolDown()
    {
        timer -= Time.deltaTime; //쿨타임 계산하기

        if (timer < 0.0f) //0보다 작으면
        {
            isCooldown = false; //쿨타임 false
            textCoolDown.gameObject.SetActive(false); // 쿨타임 텍스트 비활성화
            imageCoolDown.fillAmount = 0.0f; //쿨타임 이미지 0
           
        }
        else
        {
            textCoolDown.text = Mathf.RoundToInt(timer).ToString(); //쿨타임 텍스트 설정
            imageCoolDown.fillAmount = timer / coolDownTime; //쿨타임 이미지 설정
           
        }
    }

    public void UseSpell(float coolTime) //스킬사용시 호출
    {
        if (!isCooldown) //쿨타임이 아닐때
        {
            isCooldown = true; //쿨타임 true로 설정
            coolDownTime = coolTime; //쿨타임 설정
            timer = coolDownTime; // 타이머 설정
            textCoolDown.gameObject.SetActive(true); //쿨타임 텍스트 설정
        }
    }
}
