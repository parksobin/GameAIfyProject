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

    private bool switched = false;      // �ߺ� ���� ������

    private void Awake()
    {
        if(instance == null) instance = this;
    }

    void Start()
    {
        // ���� �� BasicBGM ���
        BasicBGM.loop = true;
        BasicBGM.Play();

        BossBGM.loop = true;
        BossBGM.Stop(); // ó������ ����
    }

    void Update()
    {
        // ���� �� ���� �� ����� ����
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
