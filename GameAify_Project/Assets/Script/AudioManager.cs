using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource BasicBGM;
    public AudioSource BossBGM;
    public AudioSource SyringeSound;
    public AudioSource VaccineSound;
    public AudioSource MessSound;

    private AudioSource[] BgmGroup;
    private AudioSource[] SfxGroup;

    private bool switched = false;      // 중복 실행 방지용

    private void Awake()
    {
        if(instance == null) instance = this;
    }

    void Start()
    {
        // 시작 시 BasicBGM 재생
        BasicBGM.loop = true;
        BasicBGM.Play(); // ★ 이거 나중에 가장 메인화면 사운드로 변경 !!! 아니면 stop처리 해야함

        BossBGM.loop = true;
        // 처음에는 정지
        BossBGM.Stop(); 
        SyringeSound.Stop();
        VaccineSound.Stop();
        MessSound.Stop();
        // 사운드 그룹별 분류
        SoundGroup();
    }


    void Update()
    {
        // 보스 맵 진입 시 배경음 변경
        if (!switched && StageSetting.InbossStage == true)
        {
            SwitchToBossBGM();
        }
    }

    private void SoundGroup() //사운드 음량 조절을 위한 그룹 분류
    {
        var bgmList = new System.Collections.Generic.List<AudioSource>();
        var sfxList = new System.Collections.Generic.List<AudioSource>();

        // BGM
        if (BasicBGM) bgmList.Add(BasicBGM);
        if (BossBGM) bgmList.Add(BossBGM);

        // SFX (Bgm 아닌 것들)
        if (SyringeSound) sfxList.Add(SyringeSound);
        if (VaccineSound) sfxList.Add(VaccineSound);
        if (MessSound) sfxList.Add(MessSound);

        BgmGroup = bgmList.ToArray();
        SfxGroup = sfxList.ToArray();
    }

    void SwitchToBossBGM()
    {
        switched = true;
        BasicBGM.Stop();
        BossBGM.Play();
    }

    public void SetBgmVolume(float volume01) // Bgm 볼륨 설정
    {
        float v = Mathf.Clamp01(volume01);
        if (BgmGroup == null) return;

        foreach (var src in BgmGroup)
            if (src) src.volume = v;
    }

    public void SetSfxVolume(float volume01) // Sfx 볼륨 설정
    {
        float v = Mathf.Clamp01(volume01);
        if (SfxGroup == null) return;

        foreach (var src in SfxGroup)
            if (src) src.volume = v;
    }
}
