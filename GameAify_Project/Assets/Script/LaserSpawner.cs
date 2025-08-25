using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    public BossMove BossMove;
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
    public StageSetting stageSetting;
    private bool Boss2_Laser_make = false;

    public float RotateSpeed = 30f;   // 회전 속도 (deg/s)
    public float TogglePeriod = 10f;  // 10초마다 방향 반전
    private float _dirTimer = 0f;
    private int _dir = 1;             // +1(정방향) → -1(역방향) 토글
    private bool _enteredPhase4 = false;


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

    public void BossLevel2_Rotate()
    {
        if (!Boss2_Laser_make)
        {
            laser1 = Instantiate(laserPrefab, Vector3.zero, Quaternion.identity);
            laser2 = Instantiate(laserPrefab, Vector3.zero, Quaternion.Euler(0, 0, 90));
            Boss2_Laser_make = true;

            _dir = 1;
            _dirTimer = 0f;
            _enteredPhase4 = false;
            return;
        }

        // 페이즈별 on/off
        if (BossMove.BossLevel <= 2)
        {
            laser1.SetActive(true);
            laser2.SetActive(true);

            // 4페이즈에서 빠져나오면 초기화
            if (_enteredPhase4) { _enteredPhase4 = false; _dir = 1; _dirTimer = 0f; }

            float dt = Time.deltaTime;
            float delta = RotateSpeed * dt;          // 항상 정방향
            laser1.transform.Rotate(0, 0, delta);
            laser2.transform.Rotate(0, 0, delta);
        }
        else if (BossMove.BossLevel == 4) // 10초마다 방향 반전
        {
            laser1.SetActive(true);
            laser2.SetActive(true);

            float dt = Time.deltaTime; // 일시정지 중에도 돌리려면 Time.unscaledDeltaTime
            if (!_enteredPhase4)
            {
                _enteredPhase4 = true;
                _dir = 1;
                _dirTimer = 0f;
            }

            _dirTimer += dt;
            if (_dirTimer >= TogglePeriod)
            {
                _dirTimer = 0f;
                _dir *= -1; // 방향 토글
            }

            float delta = RotateSpeed * _dir * dt;   // ★ 방향 적용!
            laser1.transform.Rotate(0, 0, delta);
            laser2.transform.Rotate(0, 0, delta);
        }
        else // (예: 3페이즈)
        {
            laser1.SetActive(false);
            laser2.SetActive(false);

            // 끄는 동안 상태 유지 or 필요 시 초기화
            // _enteredPhase4 = false; _dir = 1; _dirTimer = 0f;
        }
    }
}
