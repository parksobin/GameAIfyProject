using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource BasicBGM;
    public AudioSource BossBGM;
    public AudioSource SyringeSound;
    public AudioSource VaccineSound;
    public AudioSource MessSound;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        BasicBGM.Play();   
    }
}
