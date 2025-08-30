using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    public GameObject VideoScreen; //비디오 로우이미지
    public GameObject VideoPlayer; // 비디오 플레이어 
    public GameObject MainScreen01; // 메인화면
    public GameObject MainScreen02; //서브 메뉴 화면
    public GameObject Option;
    public GameObject PressKey;
    public MainScreenFade mainScreenFade;
    public AudioManager AudioManager;
    // 새로 추가: 옵션 UI 컴포넌트 참조
    [SerializeField] private AudioOptionsUI audioOptionsUI;

    private float videoTimer = 0;
    private bool videosign ;
    private bool mainScreen = true;
    private bool pressKeyActive = true;
    private float pressTime = 0f;
    private bool optionActive = false;
    private bool startBtnClick = false;
    private float startFadeTime = 0f;

    private void Start()
    {
        bool passed = AudioManager.StartScreenSign;
        MainScreen01.SetActive(!passed); 
        VideoScreen.SetActive(!passed);
        VideoPlayer.SetActive(!passed);
        MainScreen02.SetActive(passed);
        Option.SetActive(optionActive);
        mainScreen = !passed;
        videosign = !passed;

        // 옵션 패널에 AudioOptionsUI가 없다면 자동으로 찾아보기 (선택)
        if (!audioOptionsUI && Option)
            audioOptionsUI = Option.GetComponentInChildren<AudioOptionsUI>(true);
        AudioManager.instance.BasicBgmStart();

        if (!passed)
        { AudioManager.instance.BasicBGM.Stop(); }
        else 
        { AudioManager.instance.BasicBGM.Play(); }
    }

    private void Update()
    {
        PressKeyUpdate();
         
        //첫시작 메인 화면은 게임 실행 처음만 실행
        if (mainScreen && videosign)
        {
            videoTimer += Time.deltaTime;
            if(videoTimer >1.5f)
            {
                mainScreenFade.FadeinOparcity();
                VideoScreen.SetActive(false);
                VideoPlayer.SetActive(false);
                videosign =false;
                AudioManager.instance.BasicBGM.Play();
            }
        }

        //비디오 종료 후 메인화면 키 입력 가능
        if (mainScreen && Input.anyKeyDown&&!videosign)
        {
            mainScreen = false;
            AudioManager.MarkStartScreenPassed();
            mainScreenFade.ScreenFade();
        }

        if (startBtnClick)
        {
            startFadeTime += Time.unscaledDeltaTime;
            if (startFadeTime > 1.0f)
            {
                Time.timeScale = 1f;
                startFadeTime = 0f;
                startBtnClick = false;
                SceneManager.LoadScene("InGameScene");
            }
        }
    }

    public void ScreenOnoff()
    {
        MainScreen01.SetActive(false);
        MainScreen02.SetActive(true);
    }

    public void OptionClick()
    {
        optionActive = !optionActive;
        Option.SetActive(optionActive);

        /*/
        // 패널 닫힐 때만 명시 저장
        if (!optionActive && audioOptionsUI)
            audioOptionsUI.SaveToPrefs();
        */
    }

    private void PressKeyUpdate()
    {
        if (mainScreen)
        {
            pressTime += Time.deltaTime;
            if (pressTime > 0.5f)
            {
                pressKeyActive = !pressKeyActive;
                if (PressKey) PressKey.SetActive(pressKeyActive);
                pressTime = 0f;
            }
        }
        else
        {
            if (PressKey && PressKey.activeSelf) PressKey.SetActive(false);
        }
    }

    public void StartBtn()
    {
        startBtnClick = !startBtnClick;
        mainScreenFade.StartFadeIN();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
