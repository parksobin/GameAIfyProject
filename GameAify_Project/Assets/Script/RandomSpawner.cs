using UnityEngine;
using System.Collections.Generic;

public class RandomSpawner : MonoBehaviour
{
    public Transform player;          // �߽��� �� �÷��̾�
    public GameObject spawnEnemyPrefab;    // ������ �� ������
    public GameObject spawnItemPrefab;
    public float EnemyDistance = 10f; // �÷��̾�κ����� ���� ���� �Ÿ�
    public float ItemDistance = 15f;
    public int maxEnemy = 20;       // �ִ� �� ��
    public int maxItem = 5;
    private float EnemySpawnDelay = 2.0f;
    private float itemSpawnDelay = 5.0f;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private List<GameObject> spawnedItems = new List<GameObject>();

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
}
