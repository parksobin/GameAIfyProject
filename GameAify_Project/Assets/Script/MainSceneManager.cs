using UnityEngine;
using UnityEngine.UI; 
using TMPro;         

public class MainSceneManager : MonoBehaviour
{
    public GameObject MainScreen01;
    public GameObject MainScreen02;
    public GameObject Option;

    private static bool StartKeyDown = false;  // 씬을 다녀와도 유지
    private bool  OptionActive =false;
    public SpriteRenderer[] MainReanderArray;

    public Slider bgmSlider;                // 슬라이더 연결
    public Slider sfxSlider;                // 슬라이더 연결
    public TextMeshProUGUI BgmpercentText;     // 퍼센트 출력 텍스트
    public TextMeshProUGUI SfxpercentText;     // 퍼센트 출력 텍스트
    public Sprite[] SoundOnOff_Img; //사운드 Onoff 이미지 변경  0 켜짐 1꺼짐
    public Image BgmImg;
    public Image SfxImg;

    void Start()
    {
        // StartKeyDown에 맞춰 화면 세팅
        MainScreen01.SetActive(!StartKeyDown);
        MainScreen02.SetActive(StartKeyDown);

        Option.SetActive(OptionActive);

        // 시작 시 현재 값 반영
        UpdateVolumeText(bgmSlider.value, BgmpercentText);
        UpdateVolumeText(sfxSlider.value, SfxpercentText);

    }

    void Update()
    {
        // 아직 키 입력이 없을 때만 체크
        if (!StartKeyDown && Input.anyKeyDown)
        {
            StartKeyDown = true;
            MainScreen01.SetActive(false);
            MainScreen02.SetActive(true);
        }
        // 이미지 & 음량 텍스트 변경
        UpdateVolumeText(bgmSlider.value, BgmpercentText);
        UpdateVolumeText(sfxSlider.value, SfxpercentText);
    }

    public void OptionClick() // 옵션창 닫기 열기
    {
        OptionActive = !OptionActive;
        Option.SetActive(OptionActive);
    }

    void UpdateVolumeText(float value, TextMeshProUGUI txt) // 슬라이더 값애 의한 텍스트 출력과 이미지 변경
    {
        int percent = Mathf.RoundToInt(value * 100); // 0.1 → 10
        txt.text = percent.ToString() + "%";
        if (percent <= 0 && txt == BgmpercentText) BgmImg.sprite = SoundOnOff_Img[1];
        else if (percent > 0 && txt == BgmpercentText) BgmImg.sprite = SoundOnOff_Img[0];
        if (percent <= 0 && txt == SfxpercentText) SfxImg.sprite = SoundOnOff_Img[1];
        else if (percent > 0 && txt == SfxpercentText) SfxImg.sprite = SoundOnOff_Img[0];
    }
}
