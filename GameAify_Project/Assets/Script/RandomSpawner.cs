using UnityEngine;
using System.Collections.Generic;

public class RandomSpawner : MonoBehaviour
{
    public Transform player;          // 중심이 될 플레이어
    public GameObject spawnEnemyPrefab;    // 생성할 적 프리팹
    public GameObject spawnItemPrefab;
    private float EnemyDistance = 20f; // 플레이어로부터의 적의 생성 거리
    private float ItemDistance = 15f;
    private int maxEnemy = 200;       // 최대 적 수
    private int maxItem = 20;
    private float EnemySpawnDelay = 1.0f;
    private float itemSpawnDelay = 5.0f;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private List<GameObject> spawnedItems = new List<GameObject>();

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
    }

    void SpawnEnemyCheck()
    {
        if (spawnedEnemies.Count < maxEnemy) SpawnEnemy();
    }

    void SpawnitemCheck()
    {
        if(spawnedItems.Count < maxItem) SpawnItem();
    }

    void SpawnEnemy()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.position + new Vector3(randomDir.x, randomDir.y, 0) * EnemyDistance;

        GameObject obj = Instantiate(spawnEnemyPrefab, spawnPos, Quaternion.identity);
        spawnedEnemies.Add(obj);

        obj.AddComponent<AutoRemove>().Init(spawnedEnemies, obj);
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
        int randomValue = Random.Range(0, 2);
        if (randomValue == 0) Instantiate(ApplePrefab, lastDropPosition, Quaternion.identity);
        else Instantiate(AppleDamagePrefab, lastDropPosition, Quaternion.identity);
        isDropApple = false;
    }
}
