using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    private const string KEY_BGM = "BGM_VOLUME";
    private const string KEY_SFX = "SFX_VOLUME";

    public GameObject MainScreen01; // 첫 진입에서만 보이는 화면
    public GameObject MainScreen02; // 이후 기본 시작화면
    public GameObject Option; //옵션창
    public GameObject PressKey; //presskey 이미지
    public MainScreenFade mainScreenFade; 

    private bool mainScreen = true;
    private bool PressKeyActive = true;
    private float PressTime = 0f;
    private bool OptionActive = false;
    private bool startBtnClick= false; //게임 시작 버튼 사인
    private float startFadeTime = 0f; //게임 시작버튼 후 fade time 1초 여부 확인용

    public AudioManager AudioManager;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public TextMeshProUGUI BgmpercentText;
    public TextMeshProUGUI SfxpercentText;
    public Sprite[] SoundOnOff_Img; // 0 on, 1 off
    public Image BgmImg;
    public Image SfxImg;

    private void Start()
    {
        // 오직 StartScreenSign 하나로 화면 결정
        bool passed = AudioManager.StartScreenSign;
        MainScreen01.SetActive(!passed);
        MainScreen02.SetActive(passed);

        Option.SetActive(OptionActive);

        float bgm = PlayerPrefs.HasKey(KEY_BGM) ? PlayerPrefs.GetFloat(KEY_BGM) : 0.5f;
        float sfx = PlayerPrefs.HasKey(KEY_SFX) ? PlayerPrefs.GetFloat(KEY_SFX) : 0.5f;
        if (!PlayerPrefs.HasKey(KEY_BGM)) PlayerPrefs.SetFloat(KEY_BGM, bgm);
        if (!PlayerPrefs.HasKey(KEY_SFX)) PlayerPrefs.SetFloat(KEY_SFX, sfx);
        PlayerPrefs.Save();

        if (bgmSlider) bgmSlider.value = Mathf.Clamp01(bgm);
        if (sfxSlider) sfxSlider.value = Mathf.Clamp01(sfx);

        // 첫 화면 모드 여부 갱신 (PressKey 깜빡임에 사용)
        mainScreen = !passed;
    }

    private void Update()
    {
        PressKeyUpdate();

        // 첫 화면일 때 아무 키 누르면 통과 & 페이드
        if (mainScreen && Input.anyKeyDown)
        {
            mainScreen = false;
            AudioManager.MarkStartScreenPassed();   // 상태 true + 저장
            mainScreenFade.ScreenFade();            // 페이드 -> ScreenOnoff()에서 UI 전환
        }

        UpdateVolumeText(bgmSlider.value, BgmpercentText);
        UpdateVolumeText(sfxSlider.value, SfxpercentText);

        if (startBtnClick)
        {
            startFadeTime += Time.unscaledDeltaTime;
            if (startFadeTime > 1.0f)
            {
                Time.timeScale = 1f;
                startFadeTime = 0;
                startBtnClick=false;
                SceneManager.LoadScene("InGameScene");
            }
        }
    }

    public void ScreenOnoff() //메인 화면 onoff
    {
        MainScreen01.SetActive(false);
        MainScreen02.SetActive(true);
    }

    public void OptionClick() //옵션 클릭 화면
    {
        OptionActive = !OptionActive;
        Option.SetActive(OptionActive);
        if (!OptionActive) SaveVolumes();
    }

    private void PressKeyUpdate() //메인화면 pressankkey 깜빡임
    {
        if (mainScreen) 
        {
            PressTime += Time.deltaTime;
            if (PressTime > 0.5f)
            {
                PressKeyActive = !PressKeyActive;
                PressKey.SetActive(PressKeyActive);
                PressTime = 0f;
            }
        }
        else
        {
            if (PressKey.activeSelf) PressKey.SetActive(false);
        }
    }

    private void SaveVolumes() // 볼륨 저장
    {
        if (bgmSlider) PlayerPrefs.SetFloat(KEY_BGM, Mathf.Clamp01(bgmSlider.value));
        if (sfxSlider) PlayerPrefs.SetFloat(KEY_SFX, Mathf.Clamp01(sfxSlider.value));
        PlayerPrefs.Save();
    }

    private void UpdateVolumeText(float value, TextMeshProUGUI txt) //볼륨 스프라이트 이미지 변경
    {
        int percent = Mathf.RoundToInt(value * 100);
        txt.text = percent.ToString() + "%";

        if (txt == BgmpercentText)
        {
            AudioManager.SetBgmVolume(value);
            BgmImg.sprite = (percent > 0) ? SoundOnOff_Img[0] : SoundOnOff_Img[1];
        }
        else
        {
            AudioManager.SetSfxVolume(value);
            SfxImg.sprite = (percent > 0) ? SoundOnOff_Img[0] : SoundOnOff_Img[1];
        }
    }

    //public void StartBtn() => SceneManager.LoadScene("InGameScene");

    public void StartBtn()
    {
        startBtnClick=!startBtnClick;
        mainScreenFade.StartFadeIN();
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
