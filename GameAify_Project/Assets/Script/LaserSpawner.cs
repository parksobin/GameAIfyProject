using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    public GameObject warningPrefab;  // 경고용 프리팹
    public GameObject laserPrefab;    // 실제 레이저 프리팹
    public Transform firePoint;       // (원점 조준형이면 사용)
    public float warningTime = 1.5f;
    public float laserTime = 3f;

    private BossMove bossMove;
    private Transform player;         //  매 발사 시 좌표를 잠그기 위해 사용

    //2차 공격 관련
    private GameObject laser1;
    private GameObject laser2;
    public float RotateSpeed = 30f;
    public StageSetting stageSetting;
    private bool Boss2_Laser_make = false;

    private void Start()
    {
        bossMove = GetComponent<BossMove>();
        player = GameObject.FindWithTag("Player").transform;          // 캐싱
        stageSetting = GameObject.Find("MainManager").GetComponent<StageSetting>();
    }

    private Vector2[][] directionGroups = new Vector2[][]
    {
        new Vector2[] { Vector2.up, Vector2.down },
        new Vector2[] { Vector2.left, Vector2.right },
        new Vector2[] { new Vector2(1,1).normalized, new Vector2(-1,-1).normalized },
        new Vector2[] { new Vector2(-1,1).normalized, new Vector2(1,-1).normalized }
    };

    public void SponLevel1_Laser()
    {
        // ★ 경고 시작 "직전"에 현재 플레이어 좌표를 잠금
        Vector2 lockPosNow = player.position;

        // 그룹 중 3개 랜덤 선택
        var chosenGroups = directionGroups.OrderBy(_ => Random.value).Take(3).ToList();

        // 각 그룹에서 1개씩 랜덤하게 방향 선택
        var selectedDirs = new List<Vector2>();
        foreach (var group in chosenGroups)
            selectedDirs.Add(group[Random.Range(0, group.Length)]);

        // 코루틴에 '잠근 좌표'를 전달 → 경고도, 실제 레이저도 이 좌표만 사용
        foreach (var dir in selectedDirs)
            StartCoroutine(WarningAndFire(lockPosNow, dir));

        bossMove.DelayTimeReset();
    }

    private IEnumerator WarningAndFire(Vector2 lockedPos, Vector2 dir)
    {
        // 경고도 잠근 좌표로 생성
        GameObject warning = Instantiate(warningPrefab, lockedPos, Quaternion.identity);
        warning.transform.right = dir;

        // 깜빡임(총 1초)
        yield return StartCoroutine(BlinkWarning(warning, 1f));

        // 발사 직전에 '절대' player.position 재읽지 않음!
        Destroy(warning);

        // 실제 레이저도 잠근 좌표로 발사
        GameObject laser = Instantiate(laserPrefab, lockedPos, Quaternion.identity);
        laser.transform.right = dir;

        Destroy(laser, laserTime);
    }

    IEnumerator BlinkWarning(GameObject obj, float totalTime)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        float half = totalTime / 2f;

        sr.enabled = true;
        yield return new WaitForSeconds(half * 0.5f);

        sr.enabled = false;
        yield return new WaitForSeconds(half * 0.5f);

        sr.enabled = true;
        yield return new WaitForSeconds(half);
    }

    public void BossLevel2_Rotate() //보스2 회전 레이저 생성과 실제 회전
    {
        if (!Boss2_Laser_make)
        {
            laser1 = Instantiate(laserPrefab, Vector3.zero, Quaternion.identity);
            laser2 = Instantiate(laserPrefab, Vector3.zero, Quaternion.Euler(0, 0, 90));
            Boss2_Laser_make = true;
        }
        else
        {
            laser1.transform.Rotate(0, 0, RotateSpeed * Time.deltaTime);
            laser2.transform.Rotate(0, 0, RotateSpeed * Time.deltaTime);
        }
    }
}
