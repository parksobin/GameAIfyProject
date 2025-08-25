using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    private const string KEY_BGM = "BGM_VOLUME";
    private const string KEY_SFX = "SFX_VOLUME";

    public GameObject MainScreen01; // 메인 화면 (시작 한 번만)
    public GameObject MainScreen02; // 시작화면
    public GameObject Option; // 옵션창
    public GameObject PressKey; //pressKey 글자 오브젝트
    private bool mainScreen =true;
    private bool PressKeyActive =true;
    private float PressTime = 0f; //깜빡임 간격 1초
    public MainScreenFade mainScreenFade;

    private static bool StartKeyDown = false;  // 씬을 다녀와도 유지
    private bool  OptionActive =false; //옵션 활성화용
    public SpriteRenderer[] MainReanderArray; 

    public AudioManager AudioManager;
    public Slider bgmSlider;                // 슬라이더 연결
    public Slider sfxSlider;                // 슬라이더 연결
    public TextMeshProUGUI BgmpercentText;     // 퍼센트 출력 텍스트
    public TextMeshProUGUI SfxpercentText;     // 퍼센트 출력 텍스트
    public Sprite[] SoundOnOff_Img; //사운드 Onoff 이미지 변경  0 켜짐 1꺼짐
    public Image BgmImg; //Bgm 껐다 켜짐 이미지
    public Image SfxImg;//Sfx 껐다 켜짐 이미지

    void Start()
    {
        // StartKeyDown에 맞춰 화면 세팅
        MainScreen01.SetActive(!StartKeyDown);
        MainScreen02.SetActive(StartKeyDown);

        Option.SetActive(OptionActive);

        // 시작 시 현재 음량 값 반영 , 없으면 기본 50%
        float bgm = PlayerPrefs.HasKey(KEY_BGM) ? PlayerPrefs.GetFloat(KEY_BGM) : 0.5f;
        float sfx = PlayerPrefs.HasKey(KEY_SFX) ? PlayerPrefs.GetFloat(KEY_SFX) : 0.5f;
        if (!PlayerPrefs.HasKey(KEY_BGM)) PlayerPrefs.SetFloat(KEY_BGM, bgm);
        if (!PlayerPrefs.HasKey(KEY_SFX)) PlayerPrefs.SetFloat(KEY_SFX, sfx);
        PlayerPrefs.Save();
        if (bgmSlider) bgmSlider.value = Mathf.Clamp01(bgm);
        if (sfxSlider) sfxSlider.value = Mathf.Clamp01(sfx);
    }

    void Update()
    {
        PressKeyUpdate();
        // 아직 키 입력이 없을 때만 체크
        if (!StartKeyDown && Input.anyKeyDown)
        {
            StartKeyDown = true;
            mainScreenFade.ScreenFade();

            mainScreen = false;
        }
        // 이미지 & 음량 텍스트 변경
        UpdateVolumeText(bgmSlider.value, BgmpercentText);
        UpdateVolumeText(sfxSlider.value, SfxpercentText);
    }

    public void ScreenOnoff()
    {
        MainScreen01.SetActive(false);
        MainScreen02.SetActive(true);
    }

    public void OptionClick() // 옵션창 닫기 열기
    {
        OptionActive = !OptionActive;
        Option.SetActive(OptionActive);
        // 닫힐 때 음량 설정 저장
        if (!OptionActive)
        {
            SaveVolumes();
        }
    }

    private void PressKeyUpdate()
    {
        if(mainScreen)
        {
            PressTime += Time.deltaTime;
            if (PressTime > 0.5f)
            {
                PressKeyActive = !PressKeyActive;
                PressKey.SetActive(PressKeyActive);
                PressTime = 0f;
            }
        }
    }

    private void SaveVolumes()
    {
        if (bgmSlider) PlayerPrefs.SetFloat(KEY_BGM, Mathf.Clamp01(bgmSlider.value));
        if (sfxSlider) PlayerPrefs.SetFloat(KEY_SFX, Mathf.Clamp01(sfxSlider.value));
        PlayerPrefs.Save(); 
    }

    void UpdateVolumeText(float value, TextMeshProUGUI txt) // 슬라이더 값애 의한 텍스트 출력과 이미지 변경
    {
        int percent = Mathf.RoundToInt(value * 100); // 0.1 → 10
        txt.text = percent.ToString() + "%";

        //음량값과 이미지, 텍스트 바로 변경
        if(txt == BgmpercentText)
        {
            AudioManager.SetBgmVolume(value);
            if(percent > 0) BgmImg.sprite = SoundOnOff_Img[0];
            else BgmImg.sprite = SoundOnOff_Img[1];
        }
        else
        {
            AudioManager.SetSfxVolume(value);
            if (percent > 0) SfxImg.sprite = SoundOnOff_Img[0];
            else SfxImg.sprite = SoundOnOff_Img[1];
        }
    }

    public void StartBtn() //게임 시작버튼
    {
        SceneManager.LoadScene("InGameScene");
    }

    public void QuitGame() //게임종료 버튼
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

}
