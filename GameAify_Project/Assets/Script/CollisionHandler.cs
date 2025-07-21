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
                if ((CompareTag("Enemy") && (other.CompareTag("Bullet") || other.CompareTag("Guard")))) PlayerShoot.Score++;
                if (other.CompareTag("Bullet")) Destroy(other.gameObject);
                if (CompareTag("Apple") && other.CompareTag("Player")) PlayerMove.HP += 1.0f;
                if (CompareTag("AppleDmg") && other.CompareTag("Player")) PlayerMove.HP -= 0.5f;
                if ((CompareTag("Box") && (other.CompareTag("Bullet") || other.CompareTag("Guard"))))
                {
                    RandomSpawner.isDropApple = true;
                    Vector2 collisionPosition = other.transform.position;
                    RandomSpawner.SetDropPosition(collisionPosition); // 위치 전달
                }
                alreadyHit = true; // 회전체의 이중 충돌 처리 방지용
                Destroy(gameObject);
                break;
            }
        }
    }
}
