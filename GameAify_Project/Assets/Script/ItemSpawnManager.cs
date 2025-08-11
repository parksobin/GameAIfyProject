using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    public GameObject ItemPanel;

    private readonly int[] ItemSelectOn =
    {
        10, 25, 50, 100, 250, 400, 500, 650, 850,
        1000, 1250, 1500, 1850, 2200, 2500, 3000,
        3500, 4100, 4650, 4950
    };

    private int index = 0;
    private bool panelOpen = false;

    void Update()
    {
        //디버그용
        //Debug.Log("Count : " + PlayerStat.currentGauge + ", index : " + index);
        
        // 패널이 닫혀있을 때만 다음 임계값 체크
        if (!panelOpen && index < ItemSelectOn.Length)
        {
            if (PlayerStat.currentGauge >= ItemSelectOn[index])   // 핵심: >=
            {
                OpenPanel();
                index++; // 이 임계값은 소비
            }
        }

        // 선택 완료 시 닫기
        if (panelOpen && ClickChecker.SelectedItem)
        {
            ClickChecker.SelectedItem = false; // 재진입 방지
            ClosePanel();

            // 닫자마자 "이미 넘은" 다음 임계값이 있으면 바로 또 띄움
            while (!panelOpen && index < ItemSelectOn.Length &&
                   PlayerStat.currentGauge >= ItemSelectOn[index])
            {
                OpenPanel();
                index++;
            }
        }
    }

    private void OpenPanel()
    {
        panelOpen = true;
        ItemPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void ClosePanel()
    {
        Time.timeScale = 1f;
        ItemPanel.SetActive(false);
        panelOpen = false;
    }
}
