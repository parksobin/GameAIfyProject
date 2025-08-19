using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusSet : MonoBehaviour
{
    [Header("Prefab & Layout")]
    public GameObject prefab;
    public float radius = 4f;             // �߽ɡ������ �Ÿ�
    public int pointsPerSide = 5;         // ������ ���� �� ���� �� ����(=5�� 16��)
    public float diamondRotationDeg = 0f; // ������ ��ü ȸ��(�ɼ�)

    [Header("Move")]
    public float moveTime = 0.6f;         // (center)��Ÿ�� �̵� �ð�
    public bool faceOutwards = false;     // Ÿ�� �ٶ󺸰� ȸ��(�ɼ�)

    void Start()
    {

    }

    public void SpawnCenterBoss3_Hit()
    {
        // ���� ������ �� ������Ʈ ��ǥ�� ���������� ����
        Vector3 center = transform.position;
        SpawnDiamond(center);
    }

    void SpawnDiamond(Vector3 center)
    {
        List<Vector3> targets = BuildDiamondTargets(center, radius, pointsPerSide, diamondRotationDeg);

        foreach (var target in targets)
        {
            // ��� center���� ���� �� �� Ÿ������ �̵�
            var go = Instantiate(prefab, center, Quaternion.identity);

            if (faceOutwards)
            {
                Vector3 dir = (target - center).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f; // ��������Ʈ�� ��(+)�� ���� ������ ��
                go.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            StartCoroutine(MoveTo(go.transform, target, moveTime));
        }
    }

    // center �������� 16�� ��ǥ ���� (��� 5, ������ �ߺ� ����)
    List<Vector3> BuildDiamondTargets(Vector3 center, float r, int perSide, float rotDeg)
    {
        // �⺻ ������ (��, ������, �Ʒ�, ����)
        Vector3[] v = {
            center + new Vector3(0,  r, 0),
            center + new Vector3(r,  0, 0),
            center + new Vector3(0, -r, 0),
            center + new Vector3(-r, 0, 0)
        };

        // ������ ��ü ȸ��(���� ����)
        var rot = Quaternion.Euler(0, 0, rotDeg);
        for (int i = 0; i < v.Length; i++)
            v[i] = center + rot * (v[i] - center);

        // �� ���� perSide-1 ���(�� �������� ���� �� �����̶� ����)
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
            // �̹� �ı��Ǿ����� ��� ����
            if (tr == null) yield break;

            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            p = p * p * (3f - 2f * p); // ������ ����(������)
            tr.position = Vector3.Lerp(start, target, p);
            yield return null;
        }
        tr.position = target;
    }
}
