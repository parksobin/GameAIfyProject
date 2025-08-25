using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class MainSpawnerAndTimer : MonoBehaviour
{
    public Transform player;          // 중심이 될 플레이어
    
    // 몬스터 스폰 관련 멤버 변수들
    public List<GameObject> spawnEnemyPrefab; // 0~4 타입 프리팹
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private float EnemySpawnDistance = 20f; // 플레이어로부터의 적의 생성 거리
    public static int waveIndex = -1;   // -1이면 웨이브 없음(대기)
    private bool waveActive = false;
    private float timeInWave = 0f; // 웨이브별 진행된 시간
    private const short MAX_ALIVE = 150;
    // alive 리스트 정리 주기(성능용)
    private float aliveCleanInterval = 0.25f;
    private float aliveCleanTimer = 0f;
    private readonly int[] remain = new int[5]; // 웨이브동안 생성해야 하는 남은 몬스터 수
    private readonly float[] rate = new float[5]; // 초당 몬스터 스폰 속도
    private readonly float[] acc = new float[5]; // 현재 쌓여있는 몬스터 수
    public static int SpawnCount;
    public float[,] SpawnRate;

    // 타이머 관련 멤버 변수들
    private float waveDuration = 45.0f; // 웨이브별 진행시간
    private float SpawnPercentCheckTime = 45.0f; // 아이템 선택창 출력
    public TextMeshProUGUI timerText; // Text 사용 시에는 Text로 변경
    public static float timeRemaining = 15 * 60; // 15분 = 900초
    private bool timerRunning = true;

    // 사과 아이템 관련 멤버 변수들
    public GameObject applePrefabInspector;     // 인스펙터에서 할당
    public GameObject appleDamagePrefabInspector;
    public static GameObject ApplePrefab;
    public static GameObject AppleDamagePrefab;
    public static bool isDropApple = false;
    public static Vector2 lastDropPosition; // 사과를 생성할 위치 확인

    private IObjectPool<PooledEnemy>[] pools = new IObjectPool<PooledEnemy>[5];
    private readonly int prewarmPerType = 300; // 타입별 프리웜

    void Awake()
    {
        ApplePrefab = applePrefabInspector;
        AppleDamagePrefab = appleDamagePrefabInspector;
        waveDuration = 45.0f; // 웨이브별 진행시간
        SpawnPercentCheckTime = 45.0f; // 아이템 선택창 출력
        timeInWave = 0f;
        timeRemaining = 15 * 60;
        waveIndex = -1;
        SpawnRate = new float[,] // 웨이브별 스폰률
        {
            // 0 ~ 4번 : 몬스터 총 스폰 수
            // 5 ~ 9번 : 초당 스폰 수
            // Virus1, Virus2, RunningDog, AppleBomber, Snailer 순서
                { 158, 0, 0, 0, 0, 3.51f, 0, 0, 0, 0}, // 1
            { 186, 0, 0, 0, 0, 4.13f, 0, 0, 0, 0}, // 2
            { 213, 0, 0, 0, 0, 4.73f, 0, 0, 0, 0}, // 3
            { 191, 48, 0, 0, 0, 4.24f, 1.07f, 0, 0, 0}, // 4
            { 212, 53, 0, 0, 0, 4.71f, 1.18f, 0, 0, 0}, // 5
            { 222, 55, 0, 0, 0, 4.93f, 1.22f, 0, 0, 0}, // 6
            { 174, 87, 25, 0, 0, 3.87f, 1.93f, 0.56f, 0, 0}, // 7
            { 182, 91, 27, 0, 0, 4.04f, 2.02f, 0.6f, 0, 0}, // 8
            { 190, 95, 29, 0, 0, 4.22f, 2.11f, 0.64f, 0, 0}, // 9
            { 66, 132, 88, 0, 43, 1.47f, 2.93f, 1.96f, 0, 0.96f}, // 10
            { 68, 137, 92, 0, 45, 1.51f, 3.04f, 2.04f, 0, 1f}, // 11
            { 71, 142, 96, 0, 46, 1.58f, 3.16f, 2.13f, 0, 1.02f}, // 12
            { 0, 136, 123, 73, 35, 0, 3.02f, 2.73f, 1.62f, 0.78f}, // 13
            { 0, 144, 137, 79, 34, 0, 3.2f, 3.04f, 1.76f, 0.76f}, // 14
            { 0, 152, 150, 84, 35, 0, 3.38f, 3.33f, 1.87f, 0.78f}, // 15
            { 0, 103, 191, 90, 64, 0, 2.29f, 4.24f, 2f, 1.42f}, // 16
            { 0, 106, 199, 86, 31, 0, 2.36f, 4.42f, 1.91f, 0.69f}, // 17
            { 0, 103, 180, 126, 95, 0, 2.29f, 4f, 2.8f, 2.11f}, // 18
            { 0, 103, 168, 159, 83, 0, 2.29f, 3.73f, 3.53f, 1.84f}, // 19
            { 0, 112, 130, 175, 156, 0, 2.49f, 2.89f, 3.89f, 3.47f} // 20
        };
    }

    void Start()
    {
        // 안전 체크 (인스펙터 빠짐 방지)
        if (player == null) Debug.LogError("player가 비어있습니다.");
        if (spawnEnemyPrefab == null || spawnEnemyPrefab.Count < 5) Debug.LogError("spawnEnemyPrefab 5개 이상이 필요합니다.");
        if (timerText == null) Debug.LogWarning("timerText가 비어있습니다."); // 없어도 스폰은 됨
        // 바로 웨이브 0 시작
        BuildPools();        
        SpawnCheck();           
    }
    void BuildPools()
    {
        var root = new GameObject("EnemyPools").transform;

        for (int i = 0; i < 5; i++)
        {
            int type = i;
            var typeRoot = new GameObject($"Pool_Type_{type}").transform;
            typeRoot.SetParent(root, false);

            pools[i] = new ObjectPool<PooledEnemy>(
                createFunc: () =>
                {
                    var go = Instantiate(spawnEnemyPrefab[type], typeRoot); // 부모 지정
                    var pe = go.GetComponent<PooledEnemy>() ?? go.AddComponent<PooledEnemy>();
                    pe.pool = pools[type];
                    go.SetActive(false);
                    return pe;
                },
                actionOnGet: (pe) =>
                {
                    if (pe.transform.parent == null) pe.transform.SetParent(typeRoot, false);
                    pe.gameObject.SetActive(true);
                    pe.OnSpawned();
                },
                actionOnRelease: (pe) =>
                {
                    if (pe.transform.parent != typeRoot) pe.transform.SetParent(typeRoot, false);
                    pe.gameObject.SetActive(false);
                },
                actionOnDestroy: (pe) => Destroy(pe.gameObject),
                collectionCheck: false,
                defaultCapacity: Mathf.Max(8, prewarmPerType),
                maxSize: 2000
            );

            // ★ 여기서 비동기 프리웜 코루틴 시작
            StartCoroutine(PrewarmRoutine(prewarmPerType, typeRoot, type));
        }
    }

    IEnumerator PrewarmRoutine(int countPerType, Transform typeRoot, int poolIndex, int batch = 300)
    {
        var tmp = new List<PooledEnemy>(batch);
        int made = 0;

        while (made < countPerType)
        {
            int n = Mathf.Min(batch, countPerType - made);
            tmp.Clear();

            // n개를 강제로 새로 만들기
            for (int k = 0; k < n; k++)
            {
                var pe = pools[poolIndex].Get();
                tmp.Add(pe);
            }

            // 만든 것들 Release → 풀에 Inactive로 쌓임
            for (int k = 0; k < n; k++)
                pools[poolIndex].Release(tmp[k]);

            made += n;

            // 한 프레임에 전부 만들면 버벅일 수 있어서 프레임 분산
            yield return null;
        }

        var op = (ObjectPool<PooledEnemy>)pools[poolIndex];
        //Debug.Log($"[Pool Prewarm] Type {poolIndex} -> Inactive: {op.CountInactive}, Children: {typeRoot.childCount}");
    }

    void Update()
    {
        //Debug.Log($"현재 몬스터 수 : {SpawnCount}");
        if (!PlayerStat.purificationClearposSign)
        {
            if (timerRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    if (timeRemaining > 0f && timeRemaining <= (15f * 60f) - SpawnPercentCheckTime)
                    {
                        SpawnCheck(); // 이벤트 실행
                        SpawnPercentCheckTime += waveDuration; // 다음 이벤트 시간 갱신
                    }
                    UpdateTimerDisplay();
                }
                else
                {
                    timeRemaining = 0;
                    timerRunning = false;
                    UpdateTimerDisplay();
                    // 여기에 타이머 종료 시 이벤트 추가
                }
            }

            // 웨이브가 진행 중인 동안에만 스폰 로직을 돌린다.
            if (waveActive)
            {
                timeInWave += Time.deltaTime;
                // --- 동시 활성 상한 체크 ---
                int alive = GetAliveCount();
                // 500 이상이면 스폰/누적을 모두 '일시 정지'
                if (alive >= MAX_ALIVE)
                {
                    // 누적(acc) 증가도 멈춰 폭발 스폰 방지
                    // (아무것도 하지 않고 바로 빠져나감)
                    // 웨이브 시간은 계속 흐르므로,
                    // 오래 막히면 해당 웨이브의 남은 수(remain)가 남은 채로 종료될 수 있음(의도).
                }
                else
                {
                    // 남은 슬롯(이번 프레임에 최대 몇 마리 더 뽑을 수 있나)
                    int slotsLeft = MAX_ALIVE - alive;

                    for (int i = 0; i < 5; i++)
                    {
                        if (remain[i] <= 0 || rate[i] <= 0f) continue;

                        // ★ 정지 상태가 아니므로 이때만 누적
                        acc[i] += rate[i] * Time.deltaTime;

                        int toSpawn = Mathf.Min(remain[i], Mathf.FloorToInt(acc[i]));
                        if (toSpawn <= 0) continue;

                        // 한 프레임 스폰량을 남은 슬롯으로 제한
                        if (toSpawn > slotsLeft) toSpawn = slotsLeft;

                        for (int k = 0; k < toSpawn; k++)
                            SpawnOne(i);

                        acc[i] -= toSpawn;
                        remain[i] -= toSpawn;
                        slotsLeft -= toSpawn;

                        if (slotsLeft <= 0) break; // 슬롯 소진 → 다음 프레임까지 대기
                    }
                }

                if (timeInWave >= waveDuration || (remain[0] + remain[1] + remain[2] + remain[3] + remain[4]) == 0)
                    waveActive = false;
            }
        }
    }
    void SpawnOne(int enemyIdx)
    {
        if (enemyIdx < 0 || enemyIdx >= spawnEnemyPrefab.Count) return;

        Vector2 dir = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 pos = player.position + new Vector3(dir.x, dir.y, 0f) * EnemySpawnDistance;

        // 풀에서 꺼내기
        var pe = pools[enemyIdx].Get();
        pe.pool = pools[enemyIdx];
        pe.transform.SetPositionAndRotation(pos, Quaternion.identity);

        // 기존 alive 카운트/리스트는 그대로 활용 가능
        SpawnCount++;
        spawnedEnemies.Add(pe.gameObject);

        // (선택) 수명 테스트용 자동 반납이 필요하면 아래처럼:
        // pe.StartCoroutine(AutoRelease(pe, 12f));
    }

    // 45초 경계마다 호출됨: 다음 웨이브 1회 초기화만 수행
    private void SpawnCheck()
    {
        int totalWaves = SpawnRate.GetLength(0);
        int nextWave = waveIndex + 1; // 현재 다음 웨이브
        if (nextWave >= totalWaves) return;

        waveIndex = nextWave;
        timeInWave = 0f;
        waveActive = true;

        for (int i = 0; i < 5; i++)
        {
            remain[i] = (int)SpawnRate[waveIndex, i];      // 0~4: 마릿수
            rate[i] = SpawnRate[waveIndex, 5 + i]; // 5~9: 초당 속도
            acc[i] = 0f;
        }
        // 디버그용(원하면)
        Debug.Log($"Wave {waveIndex+1} 시작 - counts: {remain[0]},{remain[1]},{remain[2]},{remain[3]},{remain[4]} / rates: {rate[0]},{rate[1]},{rate[2]},{rate[3]},{rate[4]}");
    }

    int GetAliveCount()
    {
        // 주기적으로만 null 정리(성능)
        aliveCleanTimer += Time.deltaTime;
        if (aliveCleanTimer >= aliveCleanInterval)
        {
            aliveCleanTimer = 0f;
            // 파괴되었거나 비활성인 것 제거
            spawnedEnemies.RemoveAll(go => go == null || !go.activeInHierarchy);
        }
        return spawnedEnemies.Count;
    }

    public static void SetDropPosition(Vector2 pos)
    {
        lastDropPosition = pos;
        int randomValue = UnityEngine.Random.Range(0, 100);
        if (randomValue < 10) Instantiate(ApplePrefab, lastDropPosition, Quaternion.identity);
        else if (randomValue >= 10 && randomValue < 20) Instantiate(AppleDamagePrefab, lastDropPosition, Quaternion.identity);
        isDropApple = false;
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        if (PlayerStat.currentGauge >= 5000) timerText.text = " ";
        else timerText.text = $"[{minutes:00}:{seconds:00}]";
    }
}
