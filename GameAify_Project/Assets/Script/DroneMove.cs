using System;
using UnityEngine;

public class DroneMove : MonoBehaviour
{
    public Transform player;      // �߽��� �� �÷��̾�
    public GameObject orbitPrefab; // ������ ������Ʈ ������
    public int numberOfGuard = 1; // �� ���� ������ų��
    private float radius = 2f;      // ���� ������
    public float speed = 1f;       // ���� �ӵ�
    public int maxGuard = 5;

    private GameObject[] orbitObjects;
    private float[] angles;

    void Start()
    {
        SetGuard();
    }

    void Update()
    {
        for (int i = 0; i < numberOfGuard; i++)
        {
            angles[i] += speed * Time.deltaTime;
            float x = Mathf.Cos(angles[i]) * radius;
            float y = Mathf.Sin(angles[i]) * radius;
            orbitObjects[i].transform.position = player.position + new Vector3(x, y, 0);
        }
    }

    private void SetGuard()
    {
        orbitObjects = new GameObject[numberOfGuard];
        angles = new float[numberOfGuard];

        float angleStep = 360f / numberOfGuard;

        for (int i = 0; i < numberOfGuard; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            angles[i] = angle;

            Vector3 pos = player.position + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            orbitObjects[i] = Instantiate(orbitPrefab, pos, Quaternion.identity);
        }
    }

    public void AddOrbitingObject()
    {
        if(numberOfGuard < maxGuard)
        {
            numberOfGuard++;
            GuardUpdate();
        }
    }

    private void GuardUpdate()
    {
        foreach (GameObject obj in orbitObjects)
        {
            Destroy(obj);
        }
        SetGuard();    
    }
}
