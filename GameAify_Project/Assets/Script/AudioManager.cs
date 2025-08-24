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

    private bool switched = false;      // 중복 실행 방지용

    private void Awake()
    {
        if(instance == null) instance = this;
    }

    void Start()
    {
        // 시작 시 BasicBGM 재생
        BasicBGM.loop = true;
        BasicBGM.Play();

        BossBGM.loop = true;
        BossBGM.Stop(); // 처음에는 정지
    }

    void Update()
    {
        // 보스 맵 진입 시 배경음 변경
        if (!switched && StageSetting.InbossStage == true)
        {
            SwitchToBossBGM();
        }
    }

    void SwitchToBossBGM()
    {
        switched = true;
        BasicBGM.Stop();
        BossBGM.Play();
    }

    public void SFXVolumeControl(bool onoff)
    {
        SyringeSound.volume = onoff ? 1 : 0;
        MessSound.volume = onoff ? 1 : 0;
        VaccineSound.volume = onoff ? 1 : 0;
    }
}
