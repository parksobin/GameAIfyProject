using UnityEngine;

public class AppleScript : MonoBehaviour
{
    void Update()
    {
        if(PlayerStat.purificationClearposSign) Destroy(gameObject);
    }
}
