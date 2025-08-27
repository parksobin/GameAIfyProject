using System.Collections;
using UnityEngine;

public class MainScreenFade : MonoBehaviour
{
    public CanvasGroup overlay;
    public float duration = 1.0f;
    public MainSceneManager sceneManager;
    public GameObject BlackPannel;

    private IEnumerator Start()
    {
        // AudioManager 준비 보장
        yield return null;

        sceneManager = GetComponent<MainSceneManager>();

        // StartScreenSign만 본다
        BlackPannel.SetActive(!AudioManager.StartScreenSign);
    }

    public void ScreenFade()
    {
        StartCoroutine(FadeSequence());
    }
    public void StartFadeIN() //Start 시 Fade 처리
    {
        BlackPannel.SetActive(true);
        StartCoroutine(NormalScreenFade());
    }
    private IEnumerator NormalScreenFade() //게임 시작 등 검정색으로 변화하는데만 사용
    {
        yield return StartCoroutine(Fade(0f, 1f, duration));
    }
    private IEnumerator FadeSequence() //1초 기준 화면 불투명도 조정 
    {
        yield return StartCoroutine(Fade(0f, 1f, duration));
        sceneManager.ScreenOnoff(); //main 1,2 껐다가 켜짐 -> 자연스러움
        yield return StartCoroutine(Fade(1f, 0f, duration));
        BlackPannel.SetActive(false);
    }

    private IEnumerator Fade(float start, float end, float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            overlay.alpha = Mathf.Lerp(start, end, t / time);
            yield return null;
        }
        overlay.alpha = end;
    }

    // 버튼/터치로 직접 통과 처리할 때 호출 가능
    public void BlackPannel_ActiveFalse()
    {
        AudioManager.MarkStartScreenPassed(); // 상태 true + 저장
        BlackPannel.SetActive(false);
    }
}
