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
                if (CompareTag("Apple") && other.CompareTag("Player")) PlayerStat.HP += 50f;
                if (CompareTag("AppleDmg") && other.CompareTag("Player")) PlayerStat.HP -= 50f;
                //if (CompareTag("Box") && other.CompareTag("Weapon"))
                //{
                //    SubSpawner.isDropApple = true;
                //    Vector2 collisionPosition = other.transform.position;
                //    SubSpawner.SetDropPosition(collisionPosition); // ��ġ ����
                //}
                alreadyHit = true; // ȸ��ü�� ���� �浹 ó�� ������
                Destroy(gameObject);
                break;
            }
        }
    }
}
