using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Text 사용 시에는 Text로 변경
    private float timeRemaining = 15 * 60; // 15분 = 900초
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
                // 여기에 타이머 종료 시 이벤트 추가
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
