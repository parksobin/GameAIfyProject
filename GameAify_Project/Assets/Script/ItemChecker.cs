using UnityEngine;

public class ItemChecker : MonoBehaviour
{
    public ItemSelectManager ItemSelectManager;
    public static bool SelectedItem = false; // �������� �� ����°�?
    public static int NowCount = 1;

    void Start()
    {
            
    }

    void Update()
    {
        
    }

    public void isSelected()
    {
        ItemSelectManager.UpgradeCountText.text = "Count : " + NowCount;
        if(NowCount <= 0) SelectedItem = true;
    }
}
