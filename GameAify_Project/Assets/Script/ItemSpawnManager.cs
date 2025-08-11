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
        //����׿�
        //Debug.Log("Count : " + PlayerStat.currentGauge + ", index : " + index);
        
        // �г��� �������� ���� ���� �Ӱ谪 üũ
        if (!panelOpen && index < ItemSelectOn.Length)
        {
            if (PlayerStat.currentGauge >= ItemSelectOn[index])   // �ٽ�: >=
            {
                OpenPanel();
                index++; // �� �Ӱ谪�� �Һ�
            }
        }

        // ���� �Ϸ� �� �ݱ�
        if (panelOpen && ClickChecker.SelectedItem)
        {
            ClickChecker.SelectedItem = false; // ������ ����
            ClosePanel();

            // ���ڸ��� "�̹� ����" ���� �Ӱ谪�� ������ �ٷ� �� ���
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
