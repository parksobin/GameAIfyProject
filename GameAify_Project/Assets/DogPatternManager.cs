using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogPatternManager : MonoBehaviour
{
    // 개무리 패턴 관련 멤버 변수들
    public Transform player;
    public GameObject RunningDogPrefab;
    private readonly List<GameObject> RunningDogList = new List<GameObject>();
    private float RunningDogSpawnDistance = 30f; // 개무리 생성 거리
    private int RunningDogCount = 10;   // 무리당 개 수
    private float RunningDogSpawnDelay = 20f;  // 무리를 출력시킬 간격
    private float spacing = 2.0f; // 개체 간 최소 간격
    private float spreadRadius = 5f;   // 첫 개 기준 확산 반경
    private bool isRoutineRunning;

    void Start()
    {
        if (!isRoutineRunning)
        {
            isRoutineRunning = true;
            StartCoroutine(SpawnDogsRoutine());
        }
    }

    IEnumerator SpawnDogsRoutine()
    {
        while (true)
        {
            // wave 10 이상이 될 때까지 대기
            yield return new WaitUntil(() => MainSpawnerAndTimer.waveIndex + 1 >= 10);
            // 직전 무리가 아직 남아 있다면 전부 사라질 때까지 대기 (중복 스폰 방지)
            yield return new WaitUntil(AllDogsCleared);
            // 20초 카운트
            yield return new WaitForSeconds(RunningDogSpawnDelay);
            // 스폰 시작
            SpawnOneHorde();
        }
    }

    void SpawnOneHorde()
    {
        RunningDogList.Clear(); // 혹시 남아있던 null 참조 정리
        PruneDeadDogs();

        // 1) 첫 마리: 플레이어 기준 30유닛 거리, 랜덤 방향
        float randomAngle = Random.Range(0f, 360f);
        Vector3 dir = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad), 0f);
        Vector3 firstDogPos = player.position + dir * RunningDogSpawnDistance;
        List<Vector3> spawnPositions = new List<Vector3>();
        spawnPositions.Add(firstDogPos);
        var firstDog = Instantiate(RunningDogPrefab, firstDogPos, Quaternion.identity);
        RunningDogList.Add(firstDog);

        // 2) 나머지 9마리: 첫 마리 주변으로 겹치지 않게 분산
        int created = 1;
        int attempt = 0;

        while (created < RunningDogCount && attempt < 1000)
        {
            attempt++;
            // 원 범위 내 랜덤 위치
            Vector2 rand = Random.insideUnitCircle.normalized * Random.Range(spacing, spreadRadius);
            Vector3 newPos = firstDogPos + new Vector3(rand.x, rand.y, 0f);

            bool tooClose = false;
            for (int i = 0; i < spawnPositions.Count; i++)
            {
                if (Vector3.Distance(newPos, spawnPositions[i]) < spacing)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose) continue;

            spawnPositions.Add(newPos);
            var dog = Instantiate(RunningDogPrefab, newPos, Quaternion.identity);
            RunningDogList.Add(dog);
            created++;
        }
        if (created < RunningDogCount)
            Debug.LogWarning("일부 개를 생성하지 못했습니다. spacing 또는 spreadRadius를 늘려보세요.");
    }

    // 모든 개무리 개체가 '완전히' 사라졌는지(파괴되었거나 비활성화되었는지) 확인
    private bool AllDogsCleared()
    {
        PruneDeadDogs();
        return RunningDogList.Count == 0;
    }

    // 리스트에서 Destroy되었거나 SetActive(false)된 개체 제거
    private void PruneDeadDogs()
    {
        RunningDogList.RemoveAll(go => go == null || !go.activeInHierarchy);
    }
}
