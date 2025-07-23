using System;
using UnityEngine;

public class DroneMove : MonoBehaviour
{
    public Transform player;      // 중심이 될 플레이어
    public GameObject orbitPrefab; // 공전할 오브젝트 프리팹
    public int numberOfGuard = 1; // 몇 개를 공전시킬지
    private float radius = 2f;      // 공전 반지름
    public float speed = 1f;       // 공전 속도
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
