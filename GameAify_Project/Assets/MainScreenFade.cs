using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MainScreenFade : MonoBehaviour
{
    public CanvasGroup overlay;   // 검은 이미지 CanvasGroup
    public float duration = 1.0f; // 각 페이드 구간 시간
    public MainSceneManager sceneManager;

    void Start()
    {
       
    }

    public void ScreenFade()
    {
        StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        // 0 → 1 (0.5초)
        yield return StartCoroutine(Fade(0f, 1f, duration));
        sceneManager.ScreenOnoff();
        // 1 → 0 (0.5초)
        yield return StartCoroutine(Fade(1f, 0f, duration));
    }

    IEnumerator Fade(float start, float end, float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime; // Time.timeScale 영향 안 받음
            float p = t / time;
            overlay.alpha = Mathf.Lerp(start, end, p);
            yield return null;
        }
        overlay.alpha = end;
    }
}
