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

    private bool switched = false;      // �ߺ� ���� ������

    private void Awake()
    {
        if(instance == null) instance = this;
    }

    void Start()
    {
        // ���� �� BasicBGM ���
        BasicBGM.loop = true;
        BasicBGM.Play(); // �� �̰� ���߿� ���� ����ȭ�� ����� ���� !!! �ƴϸ� stopó�� �ؾ���

        BossBGM.loop = true;
        // ó������ ����
        BossBGM.Stop(); 
        SyringeSound.Stop();
        VaccineSound.Stop();
        MessSound.Stop();
        // ���� �׷캰 �з�
        SoundGroup();
    }

    void Update()
    {
        // ���� �� ���� �� ����� ����
        if (!switched && StageSetting.InbossStage == true)
        {
            SwitchToBossBGM();
        }
    }

    private void SoundGroup() //���� ���� ������ ���� �׷� �з�
    {
        var bgmList = new System.Collections.Generic.List<AudioSource>();
        var sfxList = new System.Collections.Generic.List<AudioSource>();

        // BGM
        if (BasicBGM) bgmList.Add(BasicBGM);
        if (BossBGM) bgmList.Add(BossBGM);

        // SFX (Bgm �ƴ� �͵�)
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

    public void SetBgmVolume(float volume01) // Bgm ���� ����
    {
        float v = Mathf.Clamp01(volume01);
        if (BgmGroup == null) return;

        foreach (var src in BgmGroup)
            if (src) src.volume = v;
    }

    public void SetSfxVolume(float volume01) // Sfx ���� ����
    {
        float v = Mathf.Clamp01(volume01);
        if (SfxGroup == null) return;

        foreach (var src in SfxGroup)
            if (src) src.volume = v;
    }
}
