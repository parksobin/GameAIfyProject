using System;
using UnityEngine;

public class DroneMove : MonoBehaviour
{
    public Transform player; 
    public GameObject DronePrefab; // �Ϲ� ��� ������
    public GameObject UpgradeDronePrefab; // ���׷��̵� ��� ������
    private int currentDroneStep = -1;

    // �Ϲ� ���
    private GameObject[] normalDrones;
    private float[] normalAngles;
    private float normalRadius = 3f;
    private float normalSpeed = 2f;

    // ���׷��̵� ���
    private GameObject[] upgradeDrones;
    private float[] upgradeAngles;
    private float upgradeRadius = 6f;
    private float upgradeSpeed = 3f;

    void Start()
    {
        SetDrone();
    }
    private void SetDrone()
    {
        // ���� ��� ����
        if (normalDrones != null)
            foreach (var d in normalDrones) Destroy(d);
        if (upgradeDrones != null)
            foreach (var d in upgradeDrones) Destroy(d);

        switch (PlayerStat.DroneLevel)
        {
            case 1:
                CreateNormalDrones(1);
                break;
            case 2:
                CreateNormalDrones(2);
                break;
            case 3:
                CreateNormalDrones(3);
                break;
            case 4:
                CreateNormalDrones(3);
                CreateUpgradeDrones(2);
                break;
        }
    }

    void Update()
    {
        // DroneLevel ���� ����
        if (PlayerStat.DroneLevel != currentDroneStep)
        {
            currentDroneStep = PlayerStat.DroneLevel;
            DroneUpdate();
        }

        if (PlayerStat.DroneLevel == 3) PlayerStat.DronePower = PlayerStat.AttackPower / 2.0f;
        else PlayerStat.DronePower = PlayerStat.AttackPower / 3.0f;

        // �Ϲ� ��� ����
        if (normalDrones != null)
        {
            for (int i = 0; i < normalDrones.Length; i++)
            {
                normalAngles[i] += normalSpeed * Time.deltaTime;
                float x = Mathf.Cos(normalAngles[i]) * normalRadius;
                float y = Mathf.Sin(normalAngles[i]) * normalRadius;
                normalDrones[i].transform.position = player.position + new Vector3(x, y, 0);
            }
        }

        // ���׷��̵� ��� ����
        if (upgradeDrones != null)
        {
            for (int i = 0; i < upgradeDrones.Length; i++)
            {
                upgradeAngles[i] += upgradeSpeed * Time.deltaTime;
                float x = Mathf.Cos(upgradeAngles[i]) * upgradeRadius;
                float y = Mathf.Sin(upgradeAngles[i]) * upgradeRadius;
                upgradeDrones[i].transform.position = player.position + new Vector3(x, y, 0);
            }
        }
    }


    private void CreateNormalDrones(int count)
    {
        normalDrones = new GameObject[count];
        normalAngles = new float[count];
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            normalAngles[i] = angle;
            Vector3 pos = player.position + new Vector3(Mathf.Cos(angle) * normalRadius, Mathf.Sin(angle) * normalRadius, 0);
            normalDrones[i] = Instantiate(DronePrefab, pos, Quaternion.identity);
        }
    }

    private void CreateUpgradeDrones(int count)
    {
        upgradeDrones = new GameObject[count];
        upgradeAngles = new float[count];
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            upgradeAngles[i] = angle;
            Vector3 pos = player.position + new Vector3(Mathf.Cos(angle) * upgradeRadius, Mathf.Sin(angle) * upgradeRadius, 0);
            upgradeDrones[i] = Instantiate(UpgradeDronePrefab, pos, Quaternion.identity);
        }
    }

    private void DroneUpdate()
    {
        SetDrone();
    }
}
