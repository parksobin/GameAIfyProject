using UnityEngine;

public class ItemChecker : MonoBehaviour
{
    public static bool SelectedItem = false; // �������� �� ����°�?

    void Start()
    {
            
    }

    void Update()
    {
        
    }

    public void isSelected()
    {
        if(PlayerAttack.NowCount <= 0) SelectedItem = true;
    }
}
