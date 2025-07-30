using System;
using UnityEngine;

public class DroneMove : MonoBehaviour
{
    public Transform player; 
    public GameObject DronePrefab; // 일반 드론 프리팹
    public GameObject UpgradeDronePrefab; // 업그레이드 드론 프리팹
    private int currentDroneStep = -1;

    // 일반 드론
    private GameObject[] normalDrones;
    private float[] normalAngles;
    private float normalRadius = 5f;
    private float normalSpeed = 2f;

    // 업그레이드 드론
    private GameObject[] upgradeDrones;
    private float[] upgradeAngles;
    private float upgradeRadius = 10f;
    private float upgradeSpeed = 3f;

    void Start()
    {
        SetDrone();
    }

    void Update()
    {
        // DroneLevel 변경 감지
        if (PlayerStat.DroneLevel != currentDroneStep)
        {
            currentDroneStep = PlayerStat.DroneLevel;
            DroneUpdate();
        }

        // 일반 드론 공전
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

        // 업그레이드 드론 공전
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

    private void SetDrone()
    {
        // 기존 드론 제거
        if (normalDrones != null)
            foreach (var d in normalDrones) Destroy(d);
        if (upgradeDrones != null)
            foreach (var d in upgradeDrones) Destroy(d);

        switch (PlayerStat.DroneLevel)
        {
            case 1:
                CreateNormalDrones(2);
                break;
            case 2:
                CreateNormalDrones(3);
                break;
            case 3:
                CreateNormalDrones(3);
                CreateUpgradeDrones(2);
                break;
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
