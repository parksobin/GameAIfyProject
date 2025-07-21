using UnityEngine;

public class MapScroller : MonoBehaviour
{
    public Transform player;
    public float tileSize = 50f;
    public Transform[] tiles; // 9개 타일을 순서 상관없이 할당

    void Update()
    {
        foreach (Transform tile in tiles)
        {
            Vector2 playerPos = player.position;
            Vector2 tilePos = tile.position;

            float dx = playerPos.x - tilePos.x;
            float dy = playerPos.y - tilePos.y;

            if (Mathf.Abs(dx) >= tileSize * 1.5f)
            {
                tile.position += new Vector3(Mathf.Sign(dx) * tileSize * 3, 0, 0);
            }

            if (Mathf.Abs(dy) >= tileSize * 1.5f)
            {
                tile.position += new Vector3(0, Mathf.Sign(dy) * tileSize * 3, 0);
            }
        }
    }
}
