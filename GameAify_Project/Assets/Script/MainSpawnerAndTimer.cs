using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainSpawnerAndTimer : MonoBehaviour
{
    public Transform player;          // 중심이 될 플레이어
    
    // 몬스터 스폰 관련 멤버 변수들
    public List<GameObject> spawnEnemyPrefab; // 0~4 타입 프리팹
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private float EnemySpawnDistance = 20f; // 플레이어로부터의 적의 생성 거리
    private int waveIndex = -1;   // -1이면 웨이브 없음(대기)
    private bool waveActive = false;
    private float timeInWave = 0f; // 웨이브별 진행된 시간
    private readonly int[] remain = new int[5]; // 웨이브동안 생성해야 하는 남은 몬스터 수
    private readonly float[] rate = new float[5]; // 초당 몬스터 스폰 속도
    private readonly float[] acc = new float[5]; // 현재 쌓여있는 몬스터 수
    public static float[,] SpawnRate = // 웨이브별 스폰률
    {
        {158, 0, 0, 0, 0, 3.51f, 0, 0, 0, 0}, // 1
        {186, 0, 0, 0, 0, 4.13f, 0, 0, 0, 0}, // 2
        {213, 0, 0, 0, 0, 4.73f, 0, 0, 0, 0}, // 3
        {191, 48, 0, 0, 0, 4.24f, 1.07f, 0, 0, 0}, // 4
        {212, 53, 0, 0, 0, 4.71f, 1.18f, 0, 0, 0}, // 5
        {222, 55, 0, 0, 0, 4.93f, 1.22f, 0, 0, 0}, // 6
        {174, 87, 25, 0, 0, 3.87f, 1.93f, 0.56f, 0, 0}, // 7
        {182, 91, 27, 0, 0, 4.04f, 2.02f, 0.6f, 0, 0}, // 8
        {190, 95, 29, 0, 0, 4.22f, 2.11f, 0.64f, 0, 0}, // 9
        {66, 132, 88, 0, 43, 1.47f, 2.93f, 1.96f, 0, 0.96f}, // 10
        {68, 137, 92, 0, 45, 1.51f, 3.04f, 2.04f, 0, 1f}, // 11
        {71, 142, 96, 0, 46, 1.58f, 3.16f, 2.13f, 0, 1.02f}, // 12
        {0, 136, 123, 73, 35, 0, 3.02f, 2.73f, 1.62f, 0.78f}, // 13
        {0, 144, 137, 79, 34, 0, 3.2f, 3.04f, 1.76f, 0.76f}, // 14
        {0, 152, 150, 84, 35, 0, 3.38f, 3.33f, 1.87f, 0.78f}, // 15
        {0, 103, 191, 90, 64, 0, 2.29f, 4.24f, 2f, 1.42f}, // 16
        {0, 106, 199, 86, 31, 0, 2.36f, 4.42f, 1.91f, 0.69f}, // 17
        {0, 103, 180, 126, 95, 0, 2.29f, 4f, 2.8f, 2.11f}, // 18
        {0, 103, 168, 159, 83, 0, 2.29f, 3.73f, 3.53f, 1.84f}, // 19
        {0, 112, 130, 175, 156, 0, 2.49f, 2.89f, 3.89f, 3.47f} // 20
    };

    // 타이머 관련 멤버 변수들
    private float waveDuration = 45.0f; // 웨이브별 진행시간
    private float SpawnPercentCheckTime = 45.0f; // 아이템 선택창 출력
    public TextMeshProUGUI timerText; // Text 사용 시에는 Text로 변경
    private float timeRemaining = 15 * 60; // 15분 = 900초
    private bool timerRunning = true;

    // 사과 아이템 관련 멤버 변수들
    public GameObject applePrefabInspector;     // 인스펙터에서 할당
    public GameObject appleDamagePrefabInspector;
    public static GameObject ApplePrefab;
    public static GameObject AppleDamagePrefab;
    public static bool isDropApple = false;
    public static Vector2 lastDropPosition; // 사과를 생성할 위치 확인

    void Awake()
    {
        ApplePrefab = applePrefabInspector;
        AppleDamagePrefab = appleDamagePrefabInspector;
    }

    void Start()
    {
        // 안전 체크 (인스펙터 빠짐 방지)
        if (player == null) Debug.LogError("player가 비어있습니다.");
        if (spawnEnemyPrefab == null || spawnEnemyPrefab.Count < 5) Debug.LogError("spawnEnemyPrefab 5개 이상이 필요합니다.");
        if (timerText == null) Debug.LogWarning("timerText가 비어있습니다."); // 없어도 스폰은 됨

        // 바로 웨이브 0 시작
        SpawnCheck();
    }

    void Update()
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

        if (waveActive)
        {
            timeInWave += Time.deltaTime; // 0초부터 45초까지 연산

            // 각 타입 누적→정수만큼 스폰
            for (int i = 0; i < 5; i++)
            {
                if (remain[i] <= 0 || rate[i] <= 0f) continue;

                acc[i] += rate[i] * Time.deltaTime;
                int toSpawn = Mathf.Min(remain[i], Mathf.FloorToInt(acc[i]));
                if (toSpawn <= 0) continue;

                for (int k = 0; k < toSpawn; k++)
                    SpawnOne(i);

                acc[i] -= toSpawn;
                remain[i] -= toSpawn;
            }

            // 웨이브 종료(시간 만료 or 목표 소진)
            if (timeInWave >= waveDuration || (remain[0] + remain[1] + remain[2] + remain[3] + remain[4]) == 0)
                waveActive = false;
        }
    }
    void SpawnOne(int enemyIdx)
    {
        if (enemyIdx < 0 || enemyIdx >= spawnEnemyPrefab.Count) return;

        Vector2 dir = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 pos = player.position + new Vector3(dir.x, dir.y, 0f) * EnemySpawnDistance;

        GameObject obj = Instantiate(spawnEnemyPrefab[enemyIdx], pos, Quaternion.identity);
        spawnedEnemies.Add(obj);
        obj.AddComponent<AutoRemove>().Init(spawnedEnemies, obj);
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

    public static void SetDropPosition(Vector2 pos)
    {
        lastDropPosition = pos;
        int randomValue = UnityEngine.Random.Range(0, 100);
        if (randomValue < 10) Instantiate(ApplePrefab, lastDropPosition, Quaternion.identity);
        else Instantiate(AppleDamagePrefab, lastDropPosition, Quaternion.identity);
        isDropApple = false;
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"Timer [{minutes:00}:{seconds:00}]";
    }
}
