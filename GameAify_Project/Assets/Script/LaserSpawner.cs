using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    public GameObject warningPrefab;  // ���� ������
    public GameObject laserPrefab;    // ���� ������ ������
    public Transform firePoint;       // ������ ���� ��ġ
    public float warningTime = 1.5f;  // ��� �� �߻���� ���
    public float laserTime = 2f;      // ������ ���� �ð�

    private Vector2[][] directionGroups = new Vector2[][] // ������ ���� ����
    {
        new Vector2[] { Vector2.up, Vector2.down },
        new Vector2[] { Vector2.left, Vector2.right },
        new Vector2[] { new Vector2(1,1).normalized, new Vector2(-1,-1).normalized },
        new Vector2[] { new Vector2(-1,1).normalized, new Vector2(1,-1).normalized }
    };

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 firePos = transform.position;

            // �׷� �� 3�� ���� ����
            List<Vector2[]> chosenGroups = directionGroups.OrderBy(_ => Random.value).Take(3).ToList();

            // �� �׷쿡�� 1���� �����ϰ� ���� ����
            List<Vector2> selectedDirs = new List<Vector2>();
            foreach (var group in chosenGroups)
            {
                selectedDirs.Add(group[Random.Range(0, group.Length)]);
            }

            // ������ ���
            foreach (var dir in selectedDirs)
            {
                StartCoroutine(WarningAndFire(firePos, dir));
            }
        }
    }

    private IEnumerator WarningAndFire(Vector2 firePos, Vector2 dir)
    {
        // ��� ����
        GameObject warning = Instantiate(warningPrefab, firePos, Quaternion.identity);
        warning.transform.right = dir;

        yield return new WaitForSeconds(warningTime);

        // ��� ���� �� ������ �߻�
        Destroy(warning);
        GameObject laser = Instantiate(laserPrefab, firePos, Quaternion.identity);
        laser.transform.right = dir;

        // 2�� �� ������ ����
        Destroy(laser, laserTime);
    }
}
