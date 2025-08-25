using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    public GameObject warningPrefab;  // ���� ������
    public GameObject laserPrefab;    // ���� ������ ������
    public Transform firePoint;       // (���� �������̸� ���)
    public float warningTime = 1.5f;
    public float laserTime = 3f;

    private BossMove bossMove;
    private Transform player;         //  �� �߻� �� ��ǥ�� ��ױ� ���� ���

    //2�� ���� ����
    private GameObject laser1;
    private GameObject laser2;
    public float RotateSpeed = 30f;
    public StageSetting stageSetting;
    private bool Boss2_Laser_make = false;

    private void Start()
    {
        bossMove = GetComponent<BossMove>();
        player = GameObject.FindWithTag("Player").transform;          // ĳ��
        stageSetting = GameObject.Find("MainManager").GetComponent<StageSetting>();
    }

    private Vector2[][] directionGroups = new Vector2[][]
    {
        new Vector2[] { Vector2.up, Vector2.down },
        new Vector2[] { Vector2.left, Vector2.right },
        new Vector2[] { new Vector2(1,1).normalized, new Vector2(-1,-1).normalized },
        new Vector2[] { new Vector2(-1,1).normalized, new Vector2(1,-1).normalized }
    };

    public void SponLevel1_Laser()
    {
        // �� ��� ���� "����"�� ���� �÷��̾� ��ǥ�� ���
        Vector2 lockPosNow = player.position;

        // �׷� �� 3�� ���� ����
        var chosenGroups = directionGroups.OrderBy(_ => Random.value).Take(3).ToList();

        // �� �׷쿡�� 1���� �����ϰ� ���� ����
        var selectedDirs = new List<Vector2>();
        foreach (var group in chosenGroups)
            selectedDirs.Add(group[Random.Range(0, group.Length)]);

        // �ڷ�ƾ�� '��� ��ǥ'�� ���� �� ���, ���� �������� �� ��ǥ�� ���
        foreach (var dir in selectedDirs)
            StartCoroutine(WarningAndFire(lockPosNow, dir));

        bossMove.DelayTimeReset();
    }

    private IEnumerator WarningAndFire(Vector2 lockedPos, Vector2 dir)
    {
        // ��� ��� ��ǥ�� ����
        GameObject warning = Instantiate(warningPrefab, lockedPos, Quaternion.identity);
        warning.transform.right = dir;

        // ������(�� 1��)
        yield return StartCoroutine(BlinkWarning(warning, 1f));

        // �߻� ������ '����' player.position ������ ����!
        Destroy(warning);

        // ���� �������� ��� ��ǥ�� �߻�
        GameObject laser = Instantiate(laserPrefab, lockedPos, Quaternion.identity);
        laser.transform.right = dir;

        Destroy(laser, laserTime);
    }

    IEnumerator BlinkWarning(GameObject obj, float totalTime)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        float half = totalTime / 2f;

        sr.enabled = true;
        yield return new WaitForSeconds(half * 0.5f);

        sr.enabled = false;
        yield return new WaitForSeconds(half * 0.5f);

        sr.enabled = true;
        yield return new WaitForSeconds(half);
    }

    public void BossLevel2_Rotate() //����2 ȸ�� ������ ������ ���� ȸ��
    {
        if (!Boss2_Laser_make)
        {
            laser1 = Instantiate(laserPrefab, Vector3.zero, Quaternion.identity);
            laser2 = Instantiate(laserPrefab, Vector3.zero, Quaternion.Euler(0, 0, 90));
            Boss2_Laser_make = true;
        }
        else
        {
            laser1.transform.Rotate(0, 0, RotateSpeed * Time.deltaTime);
            laser2.transform.Rotate(0, 0, RotateSpeed * Time.deltaTime);
        }
    }
}
