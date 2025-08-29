using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DefaultExecutionOrder(-100)] // 항상 가장 먼저 깨어나도록(옵션이지만 추천)
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    // 메인 화면 진입 전 블랙패널/첫 화면을 통과했는지의 “유일한” 기준
    public static bool StartScreenSign = false;  // default: 첫 진입 전

    [Header("BGM / SFX Sources")]
    public AudioSource BasicBGM;
    public AudioSource BossBGM;
    public AudioSource SyringeSound;
    public AudioSource VaccineSound;
    public AudioSource MessSound;

    // 내부 그룹
    private AudioSource[] BgmGroup;
    private AudioSource[] SfxGroup;

    // 보스 BGM 전환 플래그
    private bool switched = false;

    // 볼륨 상태 + 키
    private const string KEY_BGM = "BGM_VOLUME";
    private const string KEY_SFX = "SFX_VOLUME";
    [Range(0f, 1f)] private float _bgm = 0.5f;
    [Range(0f, 1f)] private float _sfx = 0.5f;
    private bool _initialized = false; // Prefs를 최초 1회만 로드

    public float CurrentBgm => _bgm;
    public float CurrentSfx => _sfx;

    private void Awake()
    {
        // 싱글톤 보장 + 영속화
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // 최초 1회만 Prefs 로드해서 내부 상태 세팅
        if (!_initialized)
        {
            _bgm = PlayerPrefs.GetFloat(KEY_BGM, 0.5f);
            _sfx = PlayerPrefs.GetFloat(KEY_SFX, 0.5f);
            _initialized = true;
        }

        // 씬 로드 콜백 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // 소스 세팅
        SoundGroup();

        // BGM 기본 재생 상태
        if (BasicBGM)
        {
            BasicBGM.loop = true;
            if (!BasicBGM.isPlaying) BasicBGM.Play();
        }
        if (BossBGM)
        {
            BossBGM.loop = true;
            BossBGM.Stop();
        }

        if (SyringeSound) SyringeSound.Stop();
        if (VaccineSound) VaccineSound.Stop();
        if (MessSound) MessSound.Stop();

        // 현재 내부 볼륨 값을 일괄 적용(부팅 안정화)
        ApplyVolumesToAllSources();
    }

    private void Update()
    {
        // 보스 페이즈 진입 시 1회 전환
        if (!switched && StageSetting.InbossStage == true)
            SwitchToBossBGM();

        // (원래 코드 유지) 아무 키 입력 시 시작 화면 통과 플래그
        if (Input.anyKey)
            MarkStartScreenPassed();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 재로드되면 새로 활성화되는 소스들이 있을 수 있음 → 다음 프레임에 재적용
        StartCoroutine(ApplyVolumesNextFrame());
    }

    private IEnumerator ApplyVolumesNextFrame()
    {
        yield return null; // 한 프레임 대기
        // 혹시 참조가 바뀌었다면 그룹 재구성
        SoundGroup();
        ApplyVolumesToAllSources();
    }

    private void SoundGroup()
    {
        var bgmList = new System.Collections.Generic.List<AudioSource>();
        var sfxList = new System.Collections.Generic.List<AudioSource>();

        if (BasicBGM) bgmList.Add(BasicBGM);
        if (BossBGM) bgmList.Add(BossBGM);

        if (SyringeSound) sfxList.Add(SyringeSound);
        if (VaccineSound) sfxList.Add(VaccineSound);
        if (MessSound) sfxList.Add(MessSound);

        BgmGroup = bgmList.ToArray();
        SfxGroup = sfxList.ToArray();
    }

    private void SwitchToBossBGM()
    {
        switched = true;
        if (BasicBGM) BasicBGM.Stop();
        if (BossBGM && !BossBGM.isPlaying) BossBGM.Play();
    }

    // BossBGM에서 BasicBGM으로 전환하는 전역 함수
    public void SwitchToBasicBGM()
    {
        if (BossBGM && BossBGM.isPlaying)
        {
            BossBGM.Stop();
            switched = false; // 보스 BGM 전환 플래그 리셋
        }
        
        if (BasicBGM && !BasicBGM.isPlaying)
        {
            BasicBGM.Play();
        }
    }


    public void SetBgmVolume(float v01)
    {
        _bgm = Mathf.Clamp01(v01);
        PlayerPrefs.SetFloat(KEY_BGM, _bgm);
        PlayerPrefs.Save();

        if (BgmGroup == null) return;
        foreach (var src in BgmGroup)
            if (src) src.volume = _bgm;
    }

    public void SetSfxVolume(float v01)
    {
        _sfx = Mathf.Clamp01(v01);
        PlayerPrefs.SetFloat(KEY_SFX, _sfx);
        PlayerPrefs.Save();

        if (SfxGroup == null) return;
        foreach (var src in SfxGroup)
            if (src) src.volume = _sfx;
    }

    public void ApplyVolumesToAllSources()
    {
        // 내부 상태값(_bgm/_sfx)을 그대로 소스에 반영
        if (BgmGroup != null)
            foreach (var src in BgmGroup)
                if (src) src.volume = _bgm;

        if (SfxGroup != null)
            foreach (var src in SfxGroup)
                if (src) src.volume = _sfx;
    }

    public static void MarkStartScreenPassed()
    {
        if (StartScreenSign) return;
        StartScreenSign = true;
        // 필요하다면 PlayerPrefs로 영속화도 가능:
        // PlayerPrefs.SetInt("START_SCREEN_PASSED", 1);
        // PlayerPrefs.Save();
    }
}
