using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SubSpawner : MonoBehaviour
{
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private List<GameObject> spawnedMonDog = new List<GameObject>();
    private List<GameObject> spawnedItems = new List<GameObject>();

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


    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemyCheck), 0f, EnemySpawnDelay);
        //StartCoroutine(SpawnDogsRoutine());
    }

    void SpawnEnemyCheck()
    {
        //if (spawnedEnemies.Count < maxEnemy) SpawnDefaultEnemy();
    }

    /*IEnumerator SpawnDogsRoutine()
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
    }*/

    //void SpawnItem()
    //{
    //    Vector2 randomDir = Random.insideUnitCircle.normalized;
    //    Vector3 spawnPos = player.position + new Vector3(randomDir.x, randomDir.y, 0) * ItemDistance;

    //    GameObject obj = Instantiate(spawnItemPrefab, spawnPos, Quaternion.identity);
    //    spawnedItems.Add(obj);

    //    obj.AddComponent<AutoRemove>().Init(spawnedItems, obj);
    //}

    
}
