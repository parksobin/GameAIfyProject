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

    private IEnumerator FadeSequence()
    {
        yield return StartCoroutine(Fade(0f, 1f, duration));
        sceneManager.ScreenOnoff();
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
