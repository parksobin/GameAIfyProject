using TMPro;
using UnityEngine;

public class ItemSelectManager : MonoBehaviour
{
    public GameObject ItemPanel;
    public TextMeshProUGUI UpgradeCountText;
    private readonly int[] ItemSelectOn =
    {
        10, 25, 50, 100, 250, 400, 500, 650, 850,
        1000, 1250, 1500, 1850, 2200, 2500, 3000,
        3500, 4100, 4650, 4950
    };
    public static int StepIndex = 0;
    public static int allUpgrade = 0;
    private bool panelOpen = false;

    void Start()
    {
         
    }

    void Update()
    {
        UpgradeCountText.text = "Count : " + PlayerAttack.NowCount;
        // 패널이 닫혀있을 때만 다음 임계값 체크
        if (!panelOpen && StepIndex < ItemSelectOn.Length && PlayerStat.itemSelectCount != 10)
        {
            if (PlayerStat.currentGauge >= ItemSelectOn[StepIndex] && PlayerStat.itemSelectCount != 10)   // 핵심: >=
            {
                OpenPanel();
                StepIndex++; // 이 임계값은 소비
            }
        }

        // 선택 완료 시 닫기
        if (panelOpen && ItemChecker.SelectedItem || PlayerStat.itemSelectCount == 10)
        {
            ItemChecker.SelectedItem = false; // 재진입 방지
            ClosePanel();

            // 닫자마자 "이미 넘은" 다음 임계값이 있으면 바로 또 띄움
            while (!panelOpen && StepIndex < ItemSelectOn.Length &&
                   PlayerStat.currentGauge >= ItemSelectOn[StepIndex])
            {
                OpenPanel();
                StepIndex++;
            }
        }
    }

    private void OpenPanel()
    {
        panelOpen = true;
        PlayerAttack.NowCount++;
        ItemPanel.SetActive(true);
        PlayerStat.CheckUniqueLevel();
        Time.timeScale = 0f;
    }

    private void ClosePanel()
    {
        Time.timeScale = 1f;
        ItemPanel.SetActive(false);
        panelOpen = false;
    }
}
