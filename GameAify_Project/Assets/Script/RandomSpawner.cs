using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RandomSpawner : MonoBehaviour
{
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private List<GameObject> spawnedMonDog = new List<GameObject>();
    private List<GameObject> spawnedItems = new List<GameObject>();

    public Transform player;          // �߽��� �� �÷��̾�
    public GameObject[] spawnEnemyPrefab;    // ������ �� ������
    public GameObject spawnItemPrefab;
    
    private float EnemySpawnDistance = 20f; // �÷��̾�κ����� ���� ���� �Ÿ�
    
    private float ItemDistance = 15f;
    private int maxEnemy = 200;       // �ִ� �� ��
    private int maxItem = 20;
    private float EnemySpawnDelay = 1.0f;
    private float itemSpawnDelay = 5.0f;

    public GameObject dogPrefab; // ������ ������
    private float MonDogSpawnDistance = 30f; // ������ ���� �Ÿ�
    private int MonDogCount = 10; // ������ �� ��
    private float MonDogSpawnDelay = 10f;
    private float spacing = 2.0f;
    private float spreadRadius = 4f; // ù �� ���� Ȯ�� �ݰ�


    public GameObject applePrefabInspector;     // �ν����Ϳ��� �Ҵ�
    public GameObject appleDamagePrefabInspector;
    public static GameObject ApplePrefab;
    public static GameObject AppleDamagePrefab;
    public static bool isDropApple = false;
    public static Vector2 lastDropPosition; // ����� ������ ��ġ Ȯ��

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

            // 1. ù ����: �÷��̾� ���� 30���� �Ÿ�, ���� ����
            float randomAngle = Random.Range(0f, 360f);
            Vector3 dir = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad), 0f);
            Vector3 firstDogPos = player.transform.position + dir * MonDogSpawnDistance;

            spawnPositions.Add(firstDogPos);
            Instantiate(dogPrefab, firstDogPos, Quaternion.identity);

            int created = 1;
            int attempt = 0;

            // 2. ������ 9����: ù ���� �ֺ� ���� ���� �� �ڿ������� �л� ��ġ
            while (created < MonDogCount && attempt < 1000)
            {
                attempt++;

                // �� ���� �� ���� ��ġ ����
                Vector2 randCircle = Random.insideUnitCircle.normalized * Random.Range(spacing, spreadRadius);
                Vector3 newPos = firstDogPos + new Vector3(randCircle.x, randCircle.y, 0f);

                // ���� ��ġ��� ����� ������ �ִ��� Ȯ��
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
                Debug.LogWarning("�Ϻ� ���� �������� ���߽��ϴ�. �����̳� �ݰ��� �ø�����.");
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
