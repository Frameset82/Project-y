using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITypingText : MonoBehaviour
{
    public Text mainTitle;
    public Text firstTypingText; 
    public Text secondTypingText; 

    public string firstMessage;     
    public string secondMessage;
    public float m_Speed = 0.5f; 

    // Start is called before the first frame update 
    void Start() 
    { 
        mainTitle.text = "서부 마을";
        firstMessage = "18xx년 x월 x일 오전.";
        secondMessage = "시간 수호석 첫 번째 강탈지점";

        StartCoroutine(FadeTextToFullAlpha(mainTitle));
        
    } 

    void FadeOutAllText(){
        StartCoroutine(FadeTextToZero(mainTitle)); 
        StartCoroutine(FadeTextToZero(firstTypingText)); 
        StartCoroutine(FadeTextToZero(secondTypingText)); 
    }


    IEnumerator Typing(Text typingText, string message, float speed) 
    { 
        for (int i = 0; i < message.Length; i++) 
        { 
            typingText.text = message.Substring(0, i + 1); 
            yield return new WaitForSeconds(speed); 
        }
    } 


    public IEnumerator FadeTextToFullAlpha(Text text) // 알파값 0에서 1로 전환
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (text.color.a < 1.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / 2.0f));
            yield return null;
        }
        StartCoroutine(Typing(firstTypingText, firstMessage, m_Speed)); 
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(Typing(secondTypingText, secondMessage, m_Speed));
        yield return new WaitForSeconds(2.0f);
        FadeOutAllText();
    }

    public IEnumerator FadeTextToZero(Text text)  // 알파값 1에서 0으로 전환
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / 2.0f));
            yield return null;
        }
    }
}
