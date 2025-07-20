using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public string[] targetTags;
    private bool alreadyHit = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyHit) return; // ȸ��ü�� ���� �浹 ó�� ������

        foreach (string tag in targetTags)
        {
            if (other.CompareTag(tag))
            {
                if ((CompareTag("Enemy") &&
                    (other.CompareTag("Bullet") || other.CompareTag("Guard")))) PlayerShoot.Score++;
                alreadyHit = true; // ȸ��ü�� ���� �浹 ó�� ������
                Destroy(gameObject);
                if (other.CompareTag("Bullet")) Destroy(other.gameObject);
                break;
            }
        }
    }
}
