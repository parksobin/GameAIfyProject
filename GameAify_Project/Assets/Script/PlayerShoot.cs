using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public CollisionHandler collisionHandler;
    public GameObject BulletPrefab; // 발사체 프리팹
    public float BulletSpeed = 10f; // 발사체 속도
    public float shootInterval = 1.0f; // 발사 간격
    private float BulletLifetime = 1f; // 발사체 수명
    public static int Score = 0;
    private float timer = 0f;

    int[] scoreThresholds = { 20, 40, 60, 80, 100 }; // 점수별
    float[] delays = { 1.0f, 0.8f, 0.6f, 0.4f, 0.2f }; // 발사 딜레이

    public TextMeshProUGUI ScoreText;

    void Update()
    {
        CalculateDelay();
        ScoreText.text = "Score : " + Score.ToString();
        timer += Time.deltaTime;
        if (timer >= shootInterval)
        {
            Shoot();
            timer = 0f;
        }
    }

    void CalculateDelay()
    {
        for (int i = 0; i < scoreThresholds.Length; i++)
        {
            if (Score <= scoreThresholds[i])
            {
                shootInterval = delays[i];
                return;
            }
        }
        shootInterval = 0.2f; // 80 초과
    }

    void Shoot()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 direction = (mousePos - transform.position).normalized; // 방향 계산
        GameObject proj = Instantiate(BulletPrefab, transform.position, Quaternion.identity); // 발사체 생성
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
        proj.transform.rotation = Quaternion.Euler(0, 0, angle); // 마우스 커서에 맞춰서 프리팹 각도 변경
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = direction * BulletSpeed;
        Destroy(proj, BulletLifetime); // 일정 시간 뒤 발사체 제거
    }
}
