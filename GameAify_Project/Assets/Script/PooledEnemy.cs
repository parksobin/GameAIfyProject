// PooledEnemy.cs
using UnityEngine;
using UnityEngine.Pool;

public class PooledEnemy : MonoBehaviour
{
    [HideInInspector] public IObjectPool<PooledEnemy> pool;

    // Ǯ���� ���� ������ ȣ������ �ʱ�ȭ(�ʿ�� Ȯ��)
    public void OnSpawned()
    {
        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = true;
        }
        // ��ƼŬ/Ʈ����/�ִ� �ʱ�ȭ �ʿ��ϸ� ���⼭
    }

    // �� ���/���� ���� ��� ȣ��
    public void Release()
    {
        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }
        pool?.Release(this);
    }
}
