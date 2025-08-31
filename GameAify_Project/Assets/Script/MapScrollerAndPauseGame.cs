using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapScrollerAndPauseGame : MonoBehaviour
{
    public Transform player;
    private float tileSize = 19f;           // 타일 크기(테스트용)
    public GameObject[] tiles;                   // 9개의 Image(테스트용)
    public GameObject PausePanel; // 일시정지 패널
    public static bool isPaused=false; // 일시정지 여부
    public GameObject StatCanvas; // 통계 패널
    private bool isStatOn;

    void Awake()
    {
        isPaused = false;
        isStatOn = false;
        Time.timeScale = 1.0f;   
    }

    void OnEnable()
    {
        // 씬이 로드될 때마다 자동 리셋
        SceneManager.sceneLoaded += OnSceneLoaded_ResetPause;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded_ResetPause;
    }

    //  씬 재로드/전환 직후 초기화
    private void OnSceneLoaded_ResetPause(Scene scene, LoadSceneMode mode)
    {
        isPaused = false;
        Time.timeScale = 1.0f;

        if (PausePanel && PausePanel.activeSelf)
            PausePanel.SetActive(false);

        if (StatCanvas && StatCanvas.activeSelf)
        {
            StatCanvas.SetActive(false);
            isStatOn = false;
        }
    }

    void Update()
    {
        // 강제 유지: 다른 스크립트가 timeScale을 되돌려도, 일시정지 중에는 매 프레임 0으로 고정
        if (isPaused)
        {
            Time.timeScale = 0f;
            return; // 일시정지 중에는 이하 로직(지형 스위칭 등) 스킵
        }

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

    public void OnOff()
    {
        if (!isStatOn)
        {
            StatCanvas.SetActive(true);
            isStatOn = true;
        }
        else
        {
            StatCanvas.SetActive(false);
            isStatOn = false;
        }
    }

    public void CheckPauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused
        && (BossHPBar.BossStamina > 0 && PlayerStat.HP > 0))
        {
            isPaused = true;
            Time.timeScale = 0f;
            PausePanel.SetActive(true);
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
        AudioManager.instance.SwitchToBasicBGM();
        SceneManager.LoadScene("MainScene");
    }
}
