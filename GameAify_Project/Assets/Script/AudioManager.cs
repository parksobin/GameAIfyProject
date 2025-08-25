using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    // 메인 화면 진입 전 블랙패널/첫 화면을 통과했는지의 “유일한” 기준
    public static bool StartScreenSign = false;  // default: 첫 진입 전

    public AudioSource BasicBGM;
    public AudioSource BossBGM;
    public AudioSource SyringeSound;
    public AudioSource VaccineSound;
    public AudioSource MessSound;

    private AudioSource[] BgmGroup;
    private AudioSource[] SfxGroup;
    private bool switched = false;

    const string KEY_START = "StartScreenSign";

    private void Awake()
    {
        // 싱글턴 보장 + 영속화
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // 저장된 값 복원
        StartScreenSign = PlayerPrefs.GetInt(KEY_START, 0) == 1;
    }

    private void Start()
    {
        BasicBGM.loop = true;
        if (!BasicBGM.isPlaying) BasicBGM.Play();

        BossBGM.loop = true;
        BossBGM.Stop();
        SyringeSound.Stop();
        VaccineSound.Stop();
        MessSound.Stop();
        SoundGroup();
    }

    private void Update()
    {
        if (!switched && StageSetting.InbossStage == true)
            SwitchToBossBGM();
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
        BasicBGM.Stop();
        if (!BossBGM.isPlaying) BossBGM.Play();
    }

    public void SetBgmVolume(float v01)
    {
        float v = Mathf.Clamp01(v01);
        if (BgmGroup == null) return;
        foreach (var src in BgmGroup) if (src) src.volume = v;
    }

    public void SetSfxVolume(float v01)
    {
        float v = Mathf.Clamp01(v01);
        if (SfxGroup == null) return;
        foreach (var src in SfxGroup) if (src) src.volume = v;
    }

    // 외부에서 “통과 완료” 처리할 때 이걸 호출하면 저장까지 한번에
    public static void MarkStartScreenPassed()
    {
        StartScreenSign = true;
        PlayerPrefs.SetInt(KEY_START, 1);
        PlayerPrefs.Save();
    }
}
