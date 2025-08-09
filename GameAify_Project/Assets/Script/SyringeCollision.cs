using System.Security.Cryptography;
using UnityEngine;

public class SyringeCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy") || collision.CompareTag("RunningDog"))
        {
            if(PlayerStat.SyringeLevel <= 1) Destroy(gameObject);
        }
    }
}
