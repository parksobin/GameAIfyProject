using UnityEngine;
using System.Collections;

public class InGameFade : MonoBehaviour
{
    public CanvasGroup overlay;
    public float duration = 1.0f;
    public GameObject BlackPannel;

    private void Start()
    {
        InGameFadeOUt(); // ���� �� 1 -> 0
    }

    public void ReplayBtn()
    {
        // �������� �ø� �� ���ε�
        StartCoroutine(ReplayRoutine());
    }

    public void MainBtn()
    {
        // ��ư���� ȣ��: ȭ�鸸 ��İ�(���ε� ����)
        StartCoroutine(FadeToBlackOnly());
    }

    // Start �� Fade ó��(ȣ��� ����)
    public void FadeIN()
    {
        // ȣȯ��: ȭ�鸸 ��İ� (���ε� X)
        StartCoroutine(FadeToBlackOnly());
    }

    private void InGameFadeOUt()
    {
        // ���� �Լ��� ����: ���ο��� �ùٸ� ������ �ڷ�ƾ ����
        StartCoroutine(FadeOutOnStart());
    }

    private IEnumerator FadeOutOnStart()
    {
        BlackPannel.SetActive(true);
        overlay.alpha = 1f;
        yield return StartCoroutine(NormalScreenFadeOut()); // 1 -> 0
        // NormalScreenFadeOut ������ �г� ��
    }

    private IEnumerator FadeToBlackOnly()
    {
        BlackPannel.SetActive(true);
        overlay.alpha = 0f;
        yield return StartCoroutine(NormalScreenFade());    // 0 -> 1
        // ���⼭ ���� ���� (���� ����)
    }

    private IEnumerator ReplayRoutine()
    {
        BlackPannel.SetActive(true);
        overlay.alpha = 0f;
        yield return StartCoroutine(NormalScreenFade());    // 0 -> 1
        yield return new WaitForSecondsRealtime(0.1f);      // ��¦ ����
        SceneBoot.ReloadFresh();                             // ���⼭ ���ε�
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
            t += Time.unscaledDeltaTime; // Ÿ�ӽ����� ����
            float p = Mathf.Clamp01(t / time);
            overlay.alpha = Mathf.Lerp(start, end, p);
            yield return null;
        }
        overlay.alpha = end;
    }
}
