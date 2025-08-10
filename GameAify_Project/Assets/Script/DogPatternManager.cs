using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogPatternManager : MonoBehaviour
{
    // ������ ���� ���� ��� ������
    public Transform player;
    public GameObject RunningDogPrefab;
    private readonly List<GameObject> RunningDogList = new List<GameObject>();
    private float RunningDogSpawnDistance = 30f; // ������ ���� �Ÿ�
    private int RunningDogCount = 10;   // ������ �� ��
    private float RunningDogSpawnDelay = 20f;  // ������ ��½�ų ����
    private float spacing = 2.0f; // ��ü �� �ּ� ����
    private float spreadRadius = 5f;   // ù �� ���� Ȯ�� �ݰ�
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
            // wave 10 �̻��� �� ������ ���
            yield return new WaitUntil(() => MainSpawnerAndTimer.waveIndex + 1 >= 10);
            // ���� ������ ���� ���� �ִٸ� ���� ����� ������ ��� (�ߺ� ���� ����)
            yield return new WaitUntil(AllDogsCleared);
            // 20�� ī��Ʈ
            yield return new WaitForSeconds(RunningDogSpawnDelay);
            // ���� ����
            SpawnOneHorde();
        }
    }

    void SpawnOneHorde()
    {
        RunningDogList.Clear(); // Ȥ�� �����ִ� null ���� ����
        PruneDeadDogs();

        // 1) ù ����: �÷��̾� ���� 30���� �Ÿ�, ���� ����
        float randomAngle = Random.Range(0f, 360f);
        Vector3 dir = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad), 0f);
        Vector3 firstDogPos = player.position + dir * RunningDogSpawnDistance;
        List<Vector3> spawnPositions = new List<Vector3>();
        spawnPositions.Add(firstDogPos);
        var firstDog = Instantiate(RunningDogPrefab, firstDogPos, Quaternion.identity);
        RunningDogList.Add(firstDog);

        // 2) ������ 9����: ù ���� �ֺ����� ��ġ�� �ʰ� �л�
        int created = 1;
        int attempt = 0;

        while (created < RunningDogCount && attempt < 1000)
        {
            attempt++;
            // �� ���� �� ���� ��ġ
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
            Debug.LogWarning("�Ϻ� ���� �������� ���߽��ϴ�. spacing �Ǵ� spreadRadius�� �÷�������.");
    }

    // ��� ������ ��ü�� '������' ���������(�ı��Ǿ��ų� ��Ȱ��ȭ�Ǿ�����) Ȯ��
    private bool AllDogsCleared()
    {
        PruneDeadDogs();
        return RunningDogList.Count == 0;
    }

    // ����Ʈ���� Destroy�Ǿ��ų� SetActive(false)�� ��ü ����
    private void PruneDeadDogs()
    {
        RunningDogList.RemoveAll(go => go == null || !go.activeInHierarchy);
    }
}
