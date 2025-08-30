using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class MainSpawnerAndTimer : MonoBehaviour
{
    public Transform player;          // �߽��� �� �÷��̾�
    
    // ���� ���� ���� ��� ������
    public List<GameObject> spawnEnemyPrefab; // 0~4 Ÿ�� ������
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private float EnemySpawnDistance = 20f; // �÷��̾�κ����� ���� ���� �Ÿ�
    public static int waveIndex = -1;   // -1�̸� ���̺� ����(���)
    private bool waveActive = false;
    private float timeInWave = 0f; // ���̺꺰 ����� �ð�
    private const short MAX_ALIVE = 150;
    // alive ����Ʈ ���� �ֱ�(���ɿ�)
    private float aliveCleanInterval = 0.25f;
    private float aliveCleanTimer = 0f;
    private readonly int[] remain = new int[5]; // ���̺굿�� �����ؾ� �ϴ� ���� ���� ��
    private readonly float[] rate = new float[5]; // �ʴ� ���� ���� �ӵ�
    private readonly float[] acc = new float[5]; // ���� �׿��ִ� ���� ��
    public static int SpawnCount;
    public float[,] SpawnRate;

    // Ÿ�̸� ���� ��� ������
    private float waveDuration = 45.0f; // ���̺꺰 ����ð�
    private float SpawnPercentCheckTime = 45.0f; // ������ ����â ���
    public TextMeshProUGUI timerText; // Text ��� �ÿ��� Text�� ����
    public static float timeRemaining = 15 * 60; // 15�� = 900��
    private bool timerRunning = true;

    // ��� ������ ���� ��� ������
    public GameObject applePrefabInspector;     // �ν����Ϳ��� �Ҵ�
    public GameObject appleDamagePrefabInspector;
    public static GameObject ApplePrefab;
    public static GameObject AppleDamagePrefab;
    public static bool isDropApple = false;
    public static Vector2 lastDropPosition; // ����� ������ ��ġ Ȯ��

    private IObjectPool<PooledEnemy>[] pools = new IObjectPool<PooledEnemy>[5];
    private readonly int prewarmPerType = 300; // Ÿ�Ժ� ������

    void Awake()
    {
        ApplePrefab = applePrefabInspector;
        AppleDamagePrefab = appleDamagePrefabInspector;
        waveDuration = 45.0f; // ���̺꺰 ����ð�
        SpawnPercentCheckTime = 45.0f; // ������ ����â ���
        timeInWave = 0f;
        timeRemaining = 15 * 60;
        waveIndex = -1;
        SpawnRate = new float[,] // ���̺꺰 ������
        {
            // 0 ~ 4�� : ���� �� ���� ��
            // 5 ~ 9�� : �ʴ� ���� ��
            // Virus1, Virus2, RunningDog, AppleBomber, Snailer ����
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
        // ���� üũ (�ν����� ���� ����)
        if (player == null) Debug.LogError("player�� ����ֽ��ϴ�.");
        if (spawnEnemyPrefab == null || spawnEnemyPrefab.Count < 5) Debug.LogError("spawnEnemyPrefab 5�� �̻��� �ʿ��մϴ�.");
        if (timerText == null) Debug.LogWarning("timerText�� ����ֽ��ϴ�."); // ��� ������ ��
        // �ٷ� ���̺� 0 ����
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
                    var go = Instantiate(spawnEnemyPrefab[type], typeRoot); // �θ� ����
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

            // �� ���⼭ �񵿱� ������ �ڷ�ƾ ����
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

            // n���� ������ ���� �����
            for (int k = 0; k < n; k++)
            {
                var pe = pools[poolIndex].Get();
                tmp.Add(pe);
            }

            // ���� �͵� Release �� Ǯ�� Inactive�� ����
            for (int k = 0; k < n; k++)
                pools[poolIndex].Release(tmp[k]);

            made += n;

            // �� �����ӿ� ���� ����� ������ �� �־ ������ �л�
            yield return null;
        }

        var op = (ObjectPool<PooledEnemy>)pools[poolIndex];
        //Debug.Log($"[Pool Prewarm] Type {poolIndex} -> Inactive: {op.CountInactive}, Children: {typeRoot.childCount}");
    }

    void Update()
    {
        //Debug.Log($"���� ���� �� : {SpawnCount}");
        if (!PlayerStat.purificationClearposSign)
        {
            if (timerRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    if (timeRemaining > 0f && timeRemaining <= (15f * 60f) - SpawnPercentCheckTime)
                    {
                        SpawnCheck(); // �̺�Ʈ ����
                        SpawnPercentCheckTime += waveDuration; // ���� �̺�Ʈ �ð� ����
                    }
                    UpdateTimerDisplay();
                }
                else
                {
                    timeRemaining = 0;
                    timerRunning = false;
                    UpdateTimerDisplay();
                    PlayerStat.HP = 0;
                    // ���⿡ Ÿ�̸� ���� �� �̺�Ʈ �߰�
                }
            }

            // ���̺갡 ���� ���� ���ȿ��� ���� ������ ������.
            if (waveActive)
            {
                timeInWave += Time.deltaTime;
                // --- ���� Ȱ�� ���� üũ ---
                int alive = GetAliveCount();
                // 500 �̻��̸� ����/������ ��� '�Ͻ� ����'
                if (alive >= MAX_ALIVE)
                {
                    // ����(acc) ������ ���� ���� ���� ����
                    // (�ƹ��͵� ���� �ʰ� �ٷ� ��������)
                    // ���̺� �ð��� ��� �帣�Ƿ�,
                    // ���� ������ �ش� ���̺��� ���� ��(remain)�� ���� ä�� ����� �� ����(�ǵ�).
                }
                else
                {
                    // ���� ����(�̹� �����ӿ� �ִ� �� ���� �� ���� �� �ֳ�)
                    int slotsLeft = MAX_ALIVE - alive;

                    for (int i = 0; i < 5; i++)
                    {
                        if (remain[i] <= 0 || rate[i] <= 0f) continue;

                        // �� ���� ���°� �ƴϹǷ� �̶��� ����
                        acc[i] += rate[i] * Time.deltaTime;

                        int toSpawn = Mathf.Min(remain[i], Mathf.FloorToInt(acc[i]));
                        if (toSpawn <= 0) continue;

                        // �� ������ �������� ���� �������� ����
                        if (toSpawn > slotsLeft) toSpawn = slotsLeft;

                        for (int k = 0; k < toSpawn; k++)
                            SpawnOne(i);

                        acc[i] -= toSpawn;
                        remain[i] -= toSpawn;
                        slotsLeft -= toSpawn;

                        if (slotsLeft <= 0) break; // ���� ���� �� ���� �����ӱ��� ���
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

        // Ǯ���� ������
        var pe = pools[enemyIdx].Get();
        pe.pool = pools[enemyIdx];
        pe.transform.SetPositionAndRotation(pos, Quaternion.identity);

        // ���� alive ī��Ʈ/����Ʈ�� �״�� Ȱ�� ����
        SpawnCount++;
        spawnedEnemies.Add(pe.gameObject);

        // (����) ���� �׽�Ʈ�� �ڵ� �ݳ��� �ʿ��ϸ� �Ʒ�ó��:
        // pe.StartCoroutine(AutoRelease(pe, 12f));
    }

    // 45�� ��踶�� ȣ���: ���� ���̺� 1ȸ �ʱ�ȭ�� ����
    private void SpawnCheck()
    {
        int totalWaves = SpawnRate.GetLength(0);
        int nextWave = waveIndex + 1; // ���� ���� ���̺�
        if (nextWave >= totalWaves) return;

        waveIndex = nextWave;
        timeInWave = 0f;
        waveActive = true;

        for (int i = 0; i < 5; i++)
        {
            remain[i] = (int)SpawnRate[waveIndex, i];      // 0~4: ������
            rate[i] = SpawnRate[waveIndex, 5 + i]; // 5~9: �ʴ� �ӵ�
            acc[i] = 0f;
        }
        // ����׿�(���ϸ�)
        Debug.Log($"Wave {waveIndex+1} ���� - counts: {remain[0]},{remain[1]},{remain[2]},{remain[3]},{remain[4]} / rates: {rate[0]},{rate[1]},{rate[2]},{rate[3]},{rate[4]}");
    }

    int GetAliveCount()
    {
        // �ֱ������θ� null ����(����)
        aliveCleanTimer += Time.deltaTime;
        if (aliveCleanTimer >= aliveCleanInterval)
        {
            aliveCleanTimer = 0f;
            // �ı��Ǿ��ų� ��Ȱ���� �� ����
            spawnedEnemies.RemoveAll(go => go == null || !go.activeInHierarchy);
        }
        return spawnedEnemies.Count;
    }

    public static void SetDropPosition(Vector2 pos)
    {
        lastDropPosition = pos;
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        if (randomValue >= 98f) Instantiate(ApplePrefab, lastDropPosition, Quaternion.identity);
        else if (randomValue >= 90f && randomValue < 98f) Instantiate(AppleDamagePrefab, lastDropPosition, Quaternion.identity);
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
