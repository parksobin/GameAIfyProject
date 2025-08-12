using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    public GameObject MainScreen01;
    public GameObject MainScreen02;
    private static bool StartKeyDown =false;
    public SpriteRenderer[] MainReanderArray;
    void Start()
    {
        MainScreen01.SetActive(true);
        MainScreen02.SetActive(false);
    }
    void Update()
    {
        if(!StartKeyDown&& Input.anyKeyDown)
        {
            StartKeyDown = !StartKeyDown;
            MainScreen01.SetActive(false);
            MainScreen02.SetActive(true);
        }


    }
}
