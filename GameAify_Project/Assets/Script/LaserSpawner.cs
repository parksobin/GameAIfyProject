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
    public float laserTime = 3f;      // ������ ���� �ð�
    private BossMove bossMove;

    private void Start()
    {
        bossMove = gameObject.GetComponent<BossMove>();   
    }

    private Vector2[][] directionGroups = new Vector2[][] // ������ ���� ����
    {
        new Vector2[] { Vector2.up, Vector2.down },
        new Vector2[] { Vector2.left, Vector2.right },
        new Vector2[] { new Vector2(1,1).normalized, new Vector2(-1,-1).normalized },
        new Vector2[] { new Vector2(-1,1).normalized, new Vector2(1,-1).normalized }
    };

    void Update()
    {
    }

    public  void SponLevel1_Laser()
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

        bossMove.DelayTimeReset();
    }

    private IEnumerator WarningAndFire(Vector2 firePos, Vector2 dir)
    {
        // ��� ����
        GameObject warning = Instantiate(warningPrefab, firePos, Quaternion.identity);
        warning.transform.right = dir;

        // ������ ���� (�� 1��)
        yield return StartCoroutine(BlinkWarning(warning, 1f));
        //yield return new WaitForSeconds(warningTime);

        // ��� ���� �� ������ �߻�
        Destroy(warning);
        GameObject laser = Instantiate(laserPrefab, firePos, Quaternion.identity);
        laser.transform.right = dir;

        // 2�� �� ������ ����
        Destroy(laser, laserTime);
    }

    IEnumerator BlinkWarning(GameObject obj, float totalTime)  //������ ������ �Լ�
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        float half = totalTime / 2f;


        //�����Ÿ��� ������
        sr.enabled = true;
        yield return new WaitForSeconds(half * 0.5f);  //0.25 ����
         
        sr.enabled = false;
        yield return new WaitForSeconds(half * 0.5f);  //0.25 ����

        sr.enabled = true;
        yield return new WaitForSeconds(half);  //0.5 ����
    }

}
