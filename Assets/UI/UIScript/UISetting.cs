using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UISetting : UserInterface
{
    public AudioMixer audioMixer;                           // 오디오 믹서
    public Slider masterSlider;                             // 마스터 볼륨
    public Slider bgmSlider;                                // 배경음 볼륨
    public Slider enviroSlider;                             // 환경음 볼륨
    public Slider effectSlider;                             // 효과음 볼륨

    FullScreenMode screenMode;                              // 드롭 다운
    public Dropdown resolutionDropdown;                     // 전체 화면 토글 버튼
    public Toggle fullscreenBtn;                            // 해상도 배열
    List<Resolution> resolutions = new List<Resolution>();  // 드롭다운 항목 값
    [SerializeField] int resolutionNum;

    void Start(){
        InitUI();
    }

    void InitUI(){
        // 모니터가 지원하는 해상도 정보를 리스트 resolutions에 할당
        // 화면 재생 빈도가 144인 값만 할당
        // for(int i = 0; i<Screen.resolutions.Length; i++){
        //     if(Screen.resolutions[i].refreshRate == 144)
        //         resolutions.Add(Screen.resolutions[i]);
        // }
        resolutions.AddRange(Screen.resolutions);
        resolutions.Reverse();

        // 드롭다운의 옵션 정리
        resolutionDropdown.options.Clear();

        int optionNum = 0;
        // 가져온 해상도의 갯수만큼 반복하면서 옵션을 추가
        // Options List 형식이 OptionData 클래스로 되어있으니 OptionData 객체 생성후
        // text 변수에 해상도 값을 넣어준 후 Options List에 추가
        foreach(Resolution item in resolutions){
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.width + "x" + item.height + " " + item.refreshRate + "hz";
            resolutionDropdown.options.Add(option);

            // 현재 해상도 항목 값을 드롭다운 선택값으로 변경
            // 처음 시작시 드롭다운에 선택된 값이 초기화 되어 있지 않기때문
            if(item.width == Screen.width && item.height == Screen.height)
                resolutionDropdown.value = optionNum;
            optionNum++;
        }
        // 드롭 다운 새로고침
        resolutionDropdown.RefreshShownValue();
        // 현재 전체 화면상태인지 판단후 토글 버튼상태를 초기화
        fullscreenBtn.isOn = Screen.fullScreenMode.Equals(
            FullScreenMode.FullScreenWindow) ? true : false;
    }

    // 드롭다운 항목 값 변경 감지
    public void DropBoxOptionChange(int x){
        resolutionNum = x;
    }

    // 전체 화면 토글 버튼
    public void FullScreenBtn(bool isFull){
        screenMode = isFull 
        ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    // 오디오 볼륨 컨트롤
    public void AudioControl(){
        float masterSound = masterSlider.value;
        float bgmSound = bgmSlider.value;
        float enviroSound = enviroSlider.value;
        float effectSound = effectSlider.value;

        if(masterSound == -40f) audioMixer.SetFloat("Master", -80);
        else audioMixer.SetFloat("Master", masterSound);
        if(bgmSound == -40f) audioMixer.SetFloat("BGM", -80);
        else audioMixer.SetFloat("BGM", bgmSound);
        if(enviroSound == -40f) audioMixer.SetFloat("Enviro", -80);
        else audioMixer.SetFloat("Enviro", enviroSound);
        if(effectSound == -40f) audioMixer.SetFloat("SFX", -80);
        else audioMixer.SetFloat("SFX", effectSound);
    }

    public void ToggleAudioVolume(){
        
    }

    // 확인 버튼
    public void OkBtnClick(){
        Screen.SetResolution(
            resolutions[resolutionNum].width,
            resolutions[resolutionNum].height,
            screenMode);
        // SoundManager.instance.SetVolume(bgmVol.value, enviroVol.value, effectVol.value);
    }

    // x 버튼
    public void EscapeBtnClick(){
        CloseUI();
    }
} 
