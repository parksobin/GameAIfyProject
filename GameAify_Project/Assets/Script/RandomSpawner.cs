using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RandomSpawner : MonoBehaviour
{
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private List<GameObject> spawnedMonDog = new List<GameObject>();
    private List<GameObject> spawnedItems = new List<GameObject>();

    public Transform player;          // 중심이 될 플레이어
    public GameObject[] spawnEnemyPrefab;    // 생성할 적 프리팹
    public GameObject spawnItemPrefab;
    
    private float EnemySpawnDistance = 20f; // 플레이어로부터의 적의 생성 거리
    
    private float ItemDistance = 15f;
    private int maxEnemy = 200;       // 최대 적 수
    private int maxItem = 20;
    private float EnemySpawnDelay = 1.0f;
    private float itemSpawnDelay = 5.0f;

    public GameObject dogPrefab; // 개무리 프리팹
    private float MonDogSpawnDistance = 30f; // 개무리 생성 거리
    private int MonDogCount = 10; // 무리당 개 수
    private float MonDogSpawnDelay = 10f;
    private float spacing = 2.0f;
    private float spreadRadius = 4f; // 첫 개 기준 확산 반경


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
        InvokeRepeating(nameof(SpawnEnemyCheck), 0f, EnemySpawnDelay);
        InvokeRepeating(nameof(SpawnitemCheck), 0f, itemSpawnDelay);
        StartCoroutine(SpawnDogsRoutine());
    }

    void SpawnEnemyCheck()
    {
        if (spawnedEnemies.Count < maxEnemy) SpawnDefaultEnemy();
    }

    void SpawnitemCheck()
    {
        if(spawnedItems.Count < maxItem) SpawnItem();
    }

    void SpawnDefaultEnemy()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.position + new Vector3(randomDir.x, randomDir.y, 0) * EnemySpawnDistance;
        int EnemyNum = Random.Range(0, 3);
        GameObject obj = Instantiate(spawnEnemyPrefab[EnemyNum], spawnPos, Quaternion.identity);
        spawnedEnemies.Add(obj);

        obj.AddComponent<AutoRemove>().Init(spawnedEnemies, obj);
    }
    IEnumerator SpawnDogsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(MonDogSpawnDelay);

            List<Vector3> spawnPositions = new List<Vector3>();

            // 1. 첫 마리: 플레이어 기준 30유닛 거리, 랜덤 방향
            float randomAngle = Random.Range(0f, 360f);
            Vector3 dir = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad), 0f);
            Vector3 firstDogPos = player.transform.position + dir * MonDogSpawnDistance;

            spawnPositions.Add(firstDogPos);
            Instantiate(dogPrefab, firstDogPos, Quaternion.identity);

            int created = 1;
            int attempt = 0;

            // 2. 나머지 9마리: 첫 마리 주변 원형 범위 내 자연스럽게 분산 배치
            while (created < MonDogCount && attempt < 1000)
            {
                attempt++;

                // 원 범위 내 랜덤 위치 생성
                Vector2 randCircle = Random.insideUnitCircle.normalized * Random.Range(spacing, spreadRadius);
                Vector3 newPos = firstDogPos + new Vector3(randCircle.x, randCircle.y, 0f);

                // 기존 위치들과 충분히 떨어져 있는지 확인
                bool tooClose = false;
                foreach (var pos in spawnPositions)
                {
                    if (Vector3.Distance(newPos, pos) < spacing)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (tooClose) continue;

                spawnPositions.Add(newPos);
                Instantiate(dogPrefab, newPos, Quaternion.identity);
                created++;
            }

            if (created < MonDogCount)
                Debug.LogWarning("일부 개를 생성하지 못했습니다. 간격이나 반경을 늘리세요.");
        }
    }

    void SpawnItem()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.position + new Vector3(randomDir.x, randomDir.y, 0) * ItemDistance;

        GameObject obj = Instantiate(spawnItemPrefab, spawnPos, Quaternion.identity);
        spawnedItems.Add(obj);

        obj.AddComponent<AutoRemove>().Init(spawnedItems, obj);
    }

    public static void SetDropPosition(Vector2 pos)
    {
        lastDropPosition = pos;
        int randomValue = Random.Range(0,100);
        if (randomValue < 10) Instantiate(ApplePrefab, lastDropPosition, Quaternion.identity);
        else Instantiate(AppleDamagePrefab, lastDropPosition, Quaternion.identity);
        isDropApple = false;
    }
}
