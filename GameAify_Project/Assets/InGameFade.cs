using UnityEngine;
using System.Collections;

public class InGameFade : MonoBehaviour
{
    public CanvasGroup overlay;
    public float duration = 1.0f;
    public GameObject BlackPannel;

    private void Start()
    {
        InGameFadeOUt(); // 시작 시 1 -> 0
    }

    public void ReplayBtn()
    {
        // 검정으로 올린 뒤 리로드
        StartCoroutine(ReplayRoutine());
    }

    public void MainBtn()
    {
        // 버튼에서 호출: 화면만 까맣게(리로드 없음)
        StartCoroutine(FadeToBlackOnly());
    }

    // Start 시 Fade 처리(호출부 유지)
    public void FadeIN()
    {
        // 호환용: 화면만 까맣게 (리로드 X)
        StartCoroutine(FadeToBlackOnly());
    }

    private void InGameFadeOUt()
    {
        // 기존 함수명 유지: 내부에서 올바른 순서로 코루틴 실행
        StartCoroutine(FadeOutOnStart());
    }

    private IEnumerator FadeOutOnStart()
    {
        BlackPannel.SetActive(true);
        overlay.alpha = 1f;
        yield return StartCoroutine(NormalScreenFadeOut()); // 1 -> 0
        // NormalScreenFadeOut 끝에서 패널 끔
    }

    private IEnumerator FadeToBlackOnly()
    {
        BlackPannel.SetActive(true);
        overlay.alpha = 0f;
        yield return StartCoroutine(NormalScreenFade());    // 0 -> 1
        // 여기서 검정 유지 (끄지 않음)
    }

    private IEnumerator ReplayRoutine()
    {
        BlackPannel.SetActive(true);
        overlay.alpha = 0f;
        yield return StartCoroutine(NormalScreenFade());    // 0 -> 1
        yield return new WaitForSecondsRealtime(0.1f);      // 살짝 여유
        SceneBoot.ReloadFresh();                             // 여기서 리로드
    }

    private IEnumerator NormalScreenFade() // 0 -> 1
    {
        yield return StartCoroutine(Fade(0f, 1f, duration));
    }

    private IEnumerator NormalScreenFadeOut() // 1 -> 0
    {
        yield return StartCoroutine(Fade(1f, 0f, duration));
        BlackPannel.SetActive(false);
    }

    private IEnumerator WaitTime()
    {
        yield return new WaitForSecondsRealtime(1f);
    }

    private IEnumerator Fade(float start, float end, float time)
    {
        float t = 0f;
        overlay.alpha = start;
        if (time <= 0f) { overlay.alpha = end; yield break; }

        while (t < time)
        {
            t += Time.unscaledDeltaTime; // 타임스케일 무시
            float p = Mathf.Clamp01(t / time);
            overlay.alpha = Mathf.Lerp(start, end, p);
            yield return null;
        }
        overlay.alpha = end;
    }
}
