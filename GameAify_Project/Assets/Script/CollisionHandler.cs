using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public string[] targetTags;
    private bool alreadyHit = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyHit) return; // 회전체의 이중 충돌 처리 방지용

        foreach (string tag in targetTags)
        {
            if (other.CompareTag(tag))
            {
                if ((CompareTag("Enemy") &&
                    (other.CompareTag("Bullet") || other.CompareTag("Guard")))) PlayerShoot.Score++;
                alreadyHit = true; // 회전체의 이중 충돌 처리 방지용
                Destroy(gameObject);
                if (other.CompareTag("Bullet")) Destroy(other.gameObject);
                break;
            }
        }
    }
}
