using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneCtrl : MonoBehaviour
{
    // 싱클톤 패턴
    private static LoadingSceneCtrl instance;
    public static LoadingSceneCtrl Instance {
        get{
            if(instance == null){
                var obj = FindObjectOfType<LoadingSceneCtrl>();
                if(obj != null){
                    instance = obj;
                } else {
                    instance = Create();
                }
            }
            return instance;
        }
    }
    
    // 로딩 UI 인스턴스화
    private static LoadingSceneCtrl Create(){
        return Instantiate(Resources.Load<LoadingSceneCtrl>("LoadingUI"));
    }

    // 인스턴스 검사
    void Awake() {
        if(instance != this){
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Slider progressBar;
    string loadSceneName;

    // 씬 로딩 메서드
    public void LoadScene(string sceneName){
        gameObject.SetActive(true);                     // UI 활성화
        SceneManager.sceneLoaded += OnSceneLoaded;      // 씬 로딩이 끝나는 순간
        loadSceneName = sceneName;                      // 씬 이름 할당
        StartCoroutine(LoadSceneProcess());             // 씬 로딩 프로세스 코루틴
    }

    // 씬 로딩 프로세스 코루틴
    IEnumerator LoadSceneProcess(){
        progressBar.value = 0f;
        yield return StartCoroutine(Fade(true));

        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName);
        op.allowSceneActivation = false;    // 자동 씬 전환 off

        float timer = 0f;
        while(!op.isDone){
            yield return null;
            if(op.progress < 0.9f){
                progressBar.value = op.progress;
            } else {
                timer += Time.unscaledDeltaTime;
                progressBar.value = Mathf.Lerp(0.9f, 1f, timer);
                if(progressBar.value >= 1f){
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    void OnSceneLoaded(Scene arg0, LoadSceneMode arg1){
        if(arg0.name == loadSceneName){
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    
    // 로딩 UI Fade
    IEnumerator Fade(bool isFadeIn){
        float fadeTimer = 0f;
        while(fadeTimer <= 1f){
            yield return null;
            fadeTimer += Time.unscaledDeltaTime * 1.5f;
            canvasGroup.alpha = isFadeIn ? Mathf.Lerp(0f, 1f, fadeTimer) : Mathf.Lerp(1f, 0f, fadeTimer);
        }
        if(!isFadeIn){
            gameObject.SetActive(false);
        }
    }

}
