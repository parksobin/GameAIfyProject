using System.Security.Cryptography;
using UnityEngine;

public class SyringeCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy") || collision.CompareTag("MonDog"))
        {
            if(PlayerStat.SyringeLevel <= 1) Destroy(gameObject);
        }
    }
}
