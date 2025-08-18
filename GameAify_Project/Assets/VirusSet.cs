using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusSet : MonoBehaviour
{
    [Header("Prefab & Layout")]
    public GameObject prefab;
    public float radius = 4f;             // �߽ɡ������ �Ÿ�
    public int pointsPerSide = 5;         // ������ ���� �� ���� �� ����(=5�� 16���� ��)
    public float diamondRotationDeg = 0f; // ������ ��ü ȸ��(�ɼ�)

    [Header("Move")]
    public float moveTime = 0.6f;         // (0,0,0)��Ÿ�� �̵� �ð�
    public bool faceOutwards = false;     // Ÿ�� �ٶ󺸰� ȸ��(�ɼ�)

    void Start()
    {
        SpawnDiamond();
    }

    void SpawnDiamond()
    {
        List<Vector3> targets = BuildDiamondTargets(radius, pointsPerSide, diamondRotationDeg);

        foreach (var target in targets)
        {
            var go = Instantiate(prefab, Vector3.zero, Quaternion.identity);

            if (faceOutwards)
            {
                Vector3 dir = (target - Vector3.zero).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f; // ��������Ʈ�� ��(+)�� ���� ������ ��
                go.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            StartCoroutine(MoveTo(go.transform, target, moveTime));
        }
    }

    // �� ���� ����ؼ� 16�� ��ǥ ���� (��� 5��, ������ �ߺ� ����)
    List<Vector3> BuildDiamondTargets(float r, int perSide, float rotDeg)
    {
        // �⺻ ������ (��, ������, �Ʒ�, ����)
        Vector3[] v = {
            new Vector3(0,  r, 0),
            new Vector3(r,  0, 0),
            new Vector3(0, -r, 0),
            new Vector3(-r, 0, 0)
        };

        // ������ ��ü ȸ��(�ɼ�)
        var rot = Quaternion.Euler(0, 0, rotDeg);
        for (int i = 0; i < v.Length; i++) v[i] = rot * v[i];

        // �� ���� perSide-1 ��� �������� �����ϵ�,
        // ���� ���� ���� �������� �ߺ��̹Ƿ� ���� �� 4 * (perSide-1)��
        int steps = Mathf.Max(2, perSide);
        var list = new List<Vector3>(4 * (steps - 1));

        for (int side = 0; side < 4; side++)
        {
            Vector3 a = v[side];
            Vector3 b = v[(side + 1) % 4];

            for (int k = 0; k < steps - 1; k++)     // 0,1,2,3 (perSide=5�� ��)
            {
                float t = k / (float)(steps - 1);   // 0, 0.25, 0.5, 0.75
                list.Add(Vector3.Lerp(a, b, t));
            }
        }
        return list; // �� 16��
    }

    IEnumerator MoveTo(Transform tr, Vector3 target, float duration)
    {
        Vector3 start = tr.position;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            // ������ ����(������)
            p = p * p * (3f - 2f * p);
            tr.position = Vector3.Lerp(start, target, p);
            yield return null;
        }
        tr.position = target;
    }
}
