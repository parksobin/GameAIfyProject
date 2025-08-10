using UnityEngine;

public class ClickChecker : MonoBehaviour
{
    public static bool SelectedItem = false;
    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0)) // ÁÂÅ¬¸¯
        {
            SelectedItem = true;
        }
    }
}
