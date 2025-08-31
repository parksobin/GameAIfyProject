using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusSet : MonoBehaviour
{
    [Header("Prefab & Layout")]
    public GameObject prefab;
    public float radius = 4f;
    public int pointsPerSide = 5;
    public float diamondRotationDeg = 0f;

    [Header("Move")]
    public float moveTime = 0.6f;
    public bool faceOutwards = false;

    public void SpawnCenterBoss3_Hit()
    {
        Vector3 center = transform.position; // 보스 중심
        SpawnDiamond(center);
    }

    void SpawnDiamond(Vector3 center)
    {
        if (!prefab) return;

        List<Vector3> targets = BuildDiamondTargets(center, radius, pointsPerSide, diamondRotationDeg);

        foreach (var target in targets)
        {
            var go = Instantiate(prefab, center, Quaternion.identity);

            if (faceOutwards && go) // 바깥 방향
            {
                Vector3 dir = (target - center).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
                go.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            if (go) StartCoroutine(MoveTo(go.transform, target, moveTime));
        }
    }

    List<Vector3> BuildDiamondTargets(Vector3 center, float r, int perSide, float rotDeg)
    {
        Vector3[] v = {
            center + new Vector3(0,  r, 0),
            center + new Vector3(r,  0, 0),
            center + new Vector3(0, -r, 0),
            center + new Vector3(-r, 0, 0)
        };

        var rot = Quaternion.Euler(0, 0, rotDeg);
        for (int i = 0; i < v.Length; i++)
            v[i] = center + rot * (v[i] - center);

        int steps = Mathf.Max(2, perSide);
        var list = new List<Vector3>(4 * (steps - 1));

        for (int side = 0; side < 4; side++)
        {
            Vector3 a = v[side];
            Vector3 b = v[(side + 1) % 4];

            for (int k = 0; k < steps - 1; k++)
            {
                float t = k / (float)(steps - 1);
                list.Add(Vector3.Lerp(a, b, t));
            }
        }
        return list;
    }

    IEnumerator MoveTo(Transform tr, Vector3 target, float duration)
    {
        if (!tr) yield break;

        if (duration <= 0f)
        {
            if (tr) tr.position = target; // 즉시 이동
            yield break;
        }

        Vector3 start = tr.position;
        float t = 0f;

        while (t < duration)
        {
            if (!tr) yield break; // 오브젝트가 비활성화/파괴된 경우

            // 보스 스태미나가 0 이하일 때 이동 중지
            if (BossHPBar.BossStamina <= 0)
            {
                yield break;
            }

            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            p = p * p * (3f - 2f * p); // smoothstep

            // 안전성 체크
            if (!tr) yield break;
            tr.position = Vector3.Lerp(start, target, p);

            yield return null;
        }

        if (tr) tr.position = target; // 마지막 위치 보장
    }

    // 스크립트가 비활성화/파괴될 때 모든 코루틴 정리
    void OnDisable() { StopAllCoroutines(); }
    void OnDestroy() { StopAllCoroutines(); }
}
