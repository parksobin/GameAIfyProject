using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusSet : MonoBehaviour
{
    [Header("Prefab & Layout")]
    public GameObject prefab;
    public float radius = 4f;             // 중심→꼭짓점 거리
    public int pointsPerSide = 5;         // 꼭짓점 포함 한 면의 점 개수(=5면 16개)
    public float diamondRotationDeg = 0f; // 마름모 자체 회전(옵션)

    [Header("Move")]
    public float moveTime = 0.6f;         // (center)→타겟 이동 시간
    public bool faceOutwards = false;     // 타겟 바라보게 회전(옵션)

    void Start()
    {

    }

    public void SpawnCenterBoss3_Hit()
    {
        // 생성 직전의 이 오브젝트 좌표를 기준점으로 고정
        Vector3 center = transform.position;
        SpawnDiamond(center);
    }

    void SpawnDiamond(Vector3 center)
    {
        List<Vector3> targets = BuildDiamondTargets(center, radius, pointsPerSide, diamondRotationDeg);

        foreach (var target in targets)
        {
            // 모두 center에서 생성 → 각 타겟으로 이동
            var go = Instantiate(prefab, center, Quaternion.identity);

            if (faceOutwards)
            {
                Vector3 dir = (target - center).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f; // 스프라이트가 위(+)를 보는 기준일 때
                go.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            StartCoroutine(MoveTo(go.transform, target, moveTime));
        }
    }

    // center 기준으로 16개 좌표 생성 (면당 5, 꼭짓점 중복 제외)
    List<Vector3> BuildDiamondTargets(Vector3 center, float r, int perSide, float rotDeg)
    {
        // 기본 꼭짓점 (위, 오른쪽, 아래, 왼쪽)
        Vector3[] v = {
            center + new Vector3(0,  r, 0),
            center + new Vector3(r,  0, 0),
            center + new Vector3(0, -r, 0),
            center + new Vector3(-r, 0, 0)
        };

        // 마름모 자체 회전(센터 기준)
        var rot = Quaternion.Euler(0, 0, rotDeg);
        for (int i = 0; i < v.Length; i++)
            v[i] = center + rot * (v[i] - center);

        // 각 변을 perSide-1 등분(끝 꼭짓점은 다음 변 시작이라 제외)
        int steps = Mathf.Max(2, perSide);
        var list = new List<Vector3>(4 * (steps - 1));

        for (int side = 0; side < 4; side++)
        {
            Vector3 a = v[side];
            Vector3 b = v[(side + 1) % 4];

            for (int k = 0; k < steps - 1; k++)     // 0,1,2,3 (perSide=5일 때)
            {
                float t = k / (float)(steps - 1);   // 0, 0.25, 0.5, 0.75
                list.Add(Vector3.Lerp(a, b, t));
            }
        }
        return list; // 총 16개
    }

    IEnumerator MoveTo(Transform tr, Vector3 target, float duration)
    {
        Vector3 start = tr.position;
        float t = 0f;
        while (t < duration)
        {
            // 이미 파괴되었으면 즉시 종료
            if (tr == null) yield break;

            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            p = p * p * (3f - 2f * p); // 스무스 스텝(가감속)
            tr.position = Vector3.Lerp(start, target, p);
            yield return null;
        }
        tr.position = target;
    }
}
