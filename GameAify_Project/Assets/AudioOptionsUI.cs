using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioOptionsUI : MonoBehaviour
{
    private const string KEY_BGM = "BGM_VOLUME";
    private const string KEY_SFX = "SFX_VOLUME";

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI bgmPercentText;
    [SerializeField] private TextMeshProUGUI sfxPercentText;
    [SerializeField] private Sprite[] soundOnOffImg; // [0]=On, [1]=Off
    [SerializeField] private Image bgmIcon;
    [SerializeField] private Image sfxIcon;

    private float _lastBgm = -1f;
    private float _lastSfx = -1f;

    private AudioManager AM => AudioManager.instance;

    private void OnEnable()
    {
        // Prefs -> 슬라이더
        float bgm = PlayerPrefs.GetFloat(KEY_BGM, 0.5f);
        float sfx = PlayerPrefs.GetFloat(KEY_SFX, 0.5f);
        if (bgmSlider) bgmSlider.SetValueWithoutNotify(Mathf.Clamp01(bgm));
        if (sfxSlider) sfxSlider.SetValueWithoutNotify(Mathf.Clamp01(sfx));

        // 바로 1회 반영
        ForceApply();
    }

    // “드래그 중”에도 바로바로 반영되도록
    private void Update()
    {
        if (!bgmSlider || !sfxSlider) return;

        if (!Mathf.Approximately(_lastBgm, bgmSlider.value))
        {
            ApplyBgm(bgmSlider.value);
            _lastBgm = bgmSlider.value;
        }
        if (!Mathf.Approximately(_lastSfx, sfxSlider.value))
        {
            ApplySfx(sfxSlider.value);
            _lastSfx = sfxSlider.value;
        }
    }

    private void ForceApply()
    {
        if (bgmSlider) { _lastBgm = -1f; ApplyBgm(bgmSlider.value); _lastBgm = bgmSlider.value; }
        if (sfxSlider) { _lastSfx = -1f; ApplySfx(sfxSlider.value); _lastSfx = sfxSlider.value; }
    }

    private void ApplyBgm(float v)
    {
        if (AM) AM.SetBgmVolume(v);
        PlayerPrefs.SetFloat(KEY_BGM, Mathf.Clamp01(v));
        PlayerPrefs.Save();
        UpdateUI(v, bgmPercentText, bgmIcon);
    }

    private void ApplySfx(float v)
    {
        if (AM) AM.SetSfxVolume(v);
        PlayerPrefs.SetFloat(KEY_SFX, Mathf.Clamp01(v));
        PlayerPrefs.Save();
        UpdateUI(v, sfxPercentText, sfxIcon);
    }

    private void UpdateUI(float v, TextMeshProUGUI txt, Image icon)
    {
        if (txt) txt.text = Mathf.RoundToInt(v * 100f) + "%";
        if (icon && soundOnOffImg != null && soundOnOffImg.Length >= 2)
            icon.sprite = (v > 0f) ? soundOnOffImg[0] : soundOnOffImg[1];
    }
}
