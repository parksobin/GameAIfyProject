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
    private bool panelOpen = false;

    void Start()
    {
        UpgradeCountText.text = "Count : " + ItemChecker.NowCount;        
    }

    void Update()
    {  
        // �г��� �������� ���� ���� �Ӱ谪 üũ
        if (!panelOpen && StepIndex < ItemSelectOn.Length)
        {
            if (PlayerStat.currentGauge >= ItemSelectOn[StepIndex])   // �ٽ�: >=
            {
                OpenPanel();
                StepIndex++; // �� �Ӱ谪�� �Һ�
            }
        }

        // ���� �Ϸ� �� �ݱ�
        if (panelOpen && ItemChecker.SelectedItem)
        {
            ItemChecker.SelectedItem = false; // ������ ����
            ClosePanel();

            // ���ڸ��� "�̹� ����" ���� �Ӱ谪�� ������ �ٷ� �� ���
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
