using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapScrollerAndPauseGame : MonoBehaviour
{
    public Transform player;
    public float tileSize = 50f;           // 타일 한 변(월드 유닛)
    public GameObject[] tiles;                   // 9개 Image(순서 무관)
    public GameObject PausePanel; // 일시정지 패널
    private bool isPaused; // 일시정지 상태인가를 확인

    void Start()
    {
        isPaused = false;
    }

    void Update()
    {
        CheckPauseGame();
        SwitchGround();
    }

    void SwitchGround()
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

    void CheckPauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            Time.timeScale = 0f;
            PausePanel.SetActive(true);
            isPaused = true;
        }
    }

    public void Resume()
    {
        if (isPaused)
        {
            PausePanel.SetActive(false);
            isPaused = false;
            Time.timeScale = 1.0f;
        }
    }

    public void GoMain()
    {
        SceneManager.LoadScene("MainScene");
    }
}
