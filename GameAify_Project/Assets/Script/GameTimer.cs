using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Text ��� �ÿ��� Text�� ����
    private float timeRemaining = 15 * 60; // 15�� = 900��
    private bool timerRunning = true;

    void Update()
    {
        if (timerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                timeRemaining = 0;
                timerRunning = false;
                UpdateTimerDisplay();
                // ���⿡ Ÿ�̸� ���� �� �̺�Ʈ �߰�
            }
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"Timer [{minutes:00}:{seconds:00}]";
    }
}
