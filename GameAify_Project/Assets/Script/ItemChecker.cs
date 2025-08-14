using UnityEngine;

public class ItemChecker : MonoBehaviour
{
    public static bool SelectedItem = false; // 아이템을 다 골랐는가?

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
