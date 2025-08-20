using UnityEngine;
using UnityEngine.UI;

public class MapScroller : MonoBehaviour
{
    public Transform player;
    public float tileSize = 50f;           // Ÿ�� �� ��(���� ����)
    public GameObject[] tiles;                   // 9�� Image(���� ����)

    void Update()
    {
        foreach (var tile in tiles)
        {
            Transform rt = tile.transform;

            Vector2 playerPos = player.position;    // world
            Vector2 tilePos = rt.position;        // world

            float dx = playerPos.x - tilePos.x;
            float dy = playerPos.y - tilePos.y;

            if (Mathf.Abs(dx) >= tileSize * 1.5f)
                rt.position += new Vector3(Mathf.Sign(dx) * tileSize * 3f, 0f, 0f);

            if (Mathf.Abs(dy) >= tileSize * 1.5f)
                rt.position += new Vector3(0f, Mathf.Sign(dy) * tileSize * 3f, 0f);
        }
    }
}
