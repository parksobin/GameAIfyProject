using System;
using System.Collections;
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
    public static int waveIndex = -1;   // -1이면 웨이브 없음(대기)
    private bool waveActive = false;
    private float timeInWave = 0f; // 웨이브별 진행된 시간
    private readonly int[] remain = new int[5]; // 웨이브동안 생성해야 하는 남은 몬스터 수
    private readonly float[] rate = new float[5]; // 초당 몬스터 스폰 속도
    private readonly float[] acc = new float[5]; // 현재 쌓여있는 몬스터 수
    public float[,] SpawnRate = // 웨이브별 스폰률
    {
        // 0 ~ 4번 : 몬스터 총 스폰 수
        // 5 ~ 9번 : 초당 스폰 수
        // Virus1, Virus2, RunningDog, AppleBomber, Snailer 순서
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
    public static float timeRemaining = 15 * 60; // 15분 = 900초
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
                timeInWave += Time.deltaTime; // 0초부터 waveDuration초까지 누적

                // 몬스터 타입 0~4까지 각각에 대해 스폰 계산을 수행한다.
                for (int i = 0; i < 5; i++)
                {
                    if (remain[i] <= 0 || rate[i] <= 0f) continue;

                    // 이번 프레임 동안 자란 수량을 누적치에 더한다.
                    // 예: rate=2.5, deltaTime=0.02면 acc += 0.05 → 몇 프레임 지나면 1을 넘고, 그때 실제로 스폰한다.
                    acc[i] += rate[i] * Time.deltaTime;

                    // acc의 "정수 부분"만큼 실제로 스폰할 수 있다.
                    // 다만 이번 웨이브에서 남은 마릿수(remain[i])를 넘지 않도록 최소값을 취한다.
                    int toSpawn = Mathf.Min(remain[i], Mathf.FloorToInt(acc[i]));

                    // 정수로 꺼낼 게 없다면(아직 1 미만 누적) 다음 타입으로 넘어간다.
                    if (toSpawn <= 0) continue;

                    // 방금 계산된 toSpawn 만큼 실제로 생성한다.
                    // 프레임 누락/저fps 등으로 acc가 많이 쌓였을 경우, 한 프레임에 여러 마리가 나올 수 있다(의도된 동작).
                    for (int k = 0; k < toSpawn; k++)
                        SpawnOne(i);

                    // 스폰으로 변환한 만큼 누적치에서 빼 준다.
                    // (예: acc가 1.7이고 toSpawn=1이면 acc는 0.7이 남아서 다음 프레임에 이어서 누적된다.)
                    acc[i] -= toSpawn;

                    // 실제로 뽑은 만큼 남은 마릿수도 줄인다.
                    // 이 값이 0이 되면 다음 프레임부터는 위의 continue에 걸려 더 이상 뽑지 않는다.
                    remain[i] -= toSpawn;
                }

                // 웨이브 종료 조건:
                // 1) 시간이 다 지났거나(timeInWave가 waveDuration 이상),
                // 2) 모든 타입의 남은치 합이 0(목표량을 모두 소진)인 경우.
                // 둘 중 하나라도 참이면 이번 웨이브를 종료한다.
                if (timeInWave >= waveDuration || (remain[0] + remain[1] + remain[2] + remain[3] + remain[4]) == 0)
                    waveActive = false; // 다음 웨이브 트리거가 들어올 때까지 대기 상태로 전환
            }
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
        //Debug.Log($"Wave {waveIndex+1} 시작 - counts: {remain[0]},{remain[1]},{remain[2]},{remain[3]},{remain[4]} / rates: {rate[0]},{rate[1]},{rate[2]},{rate[3]},{rate[4]}");
    }

    // 개무리 구현 함수
    

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
