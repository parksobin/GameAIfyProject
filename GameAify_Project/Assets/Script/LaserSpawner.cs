using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    public GameObject warningPrefab;  // 경고용 프리팹
    public GameObject laserPrefab;    // 실제 레이저 프리팹
    public Transform firePoint;       // 레이저 시작 위치
    public float warningTime = 1.5f;  // 경고 후 발사까지 대기
    public float laserTime = 2f;      // 레이저 유지 시간

    private Vector2[][] directionGroups = new Vector2[][] // 레이저 방향 설정
    {
        new Vector2[] { Vector2.up, Vector2.down },
        new Vector2[] { Vector2.left, Vector2.right },
        new Vector2[] { new Vector2(1,1).normalized, new Vector2(-1,-1).normalized },
        new Vector2[] { new Vector2(-1,1).normalized, new Vector2(1,-1).normalized }
    };

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 firePos = transform.position;

            // 그룹 중 3개 랜덤 선택
            List<Vector2[]> chosenGroups = directionGroups.OrderBy(_ => Random.value).Take(3).ToList();

            // 각 그룹에서 1개씩 랜덤하게 방향 선택
            List<Vector2> selectedDirs = new List<Vector2>();
            foreach (var group in chosenGroups)
            {
                selectedDirs.Add(group[Random.Range(0, group.Length)]);
            }

            // 레이저 출력
            foreach (var dir in selectedDirs)
            {
                StartCoroutine(WarningAndFire(firePos, dir));
            }
        }
    }

    private IEnumerator WarningAndFire(Vector2 firePos, Vector2 dir)
    {
        // 경고 생성
        GameObject warning = Instantiate(warningPrefab, firePos, Quaternion.identity);
        warning.transform.right = dir;

        yield return new WaitForSeconds(warningTime);

        // 경고 제거 및 레이저 발사
        Destroy(warning);
        GameObject laser = Instantiate(laserPrefab, firePos, Quaternion.identity);
        laser.transform.right = dir;

        // 2초 뒤 레이저 제거
        Destroy(laser, laserTime);
    }
}
