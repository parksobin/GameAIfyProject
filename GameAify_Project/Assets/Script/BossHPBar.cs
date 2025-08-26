using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    public Image  BossHpFill;   // 빨간 HP 이미지
    private float BossMaxHP = 50000f; //보스 전체 체력 저장
    private float duration = 0.4f; // 몇 초동안
    private float blinkInterval = 0.1f; // 몇 초 간격으로 깜빡이게 할 껀지

    public SpriteRenderer sr;
    public GameObject hitEffectPrefab;
    private float autoDestroyAfter = 1.0f; // 자동 파괴

    void Start()
    {
        BossMaxHP = PlayerStat.BossStamina;
        sr = GetComponent<SpriteRenderer>();
        UpdateHPBar();
    }

    private void Update()
    {
        UpdateHPBar();
    }

    public void TakeDamage(float damage)
    {
        float previousStamina = PlayerStat.BossStamina;
        PlayerStat.BossStamina -= damage;

        if (PlayerStat.BossStamina < 0) PlayerStat.BossStamina = 0;

        // 100의 배수 경계를 넘었는지 확인
        int previousThreshold = (int)(previousStamina / 100);
        int currentThreshold = (int)(PlayerStat.BossStamina / 100);

        // 이전 임계값보다 현재 임계값이 작으면 SpawnEffect 호출
        if (currentThreshold < previousThreshold)
        {
            SpawnEffect();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.layer == LayerMask.NameToLayer("Weapon")) SpawnEffect();
        
        if (collision.gameObject.name.StartsWith("Syringe")) TakeDamage(PlayerStat.SyringePower);
        else if (collision.gameObject.name.StartsWith("Drone")) TakeDamage(PlayerStat.DronePower);
        else if (collision.gameObject.name.StartsWith("Mess") || collision.gameObject.name.StartsWith("MessUnique")
            || collision.gameObject.name.StartsWith("MessBullet") || collision.gameObject.name.StartsWith("UniqueMessBullet"))
            TakeDamage(PlayerStat.MessPower);
        else if (collision.gameObject.name.StartsWith("VaccineFeild")) EnemyStat.OnVaccineDamage = true;
    }
    private void SpawnEffect()
    {
        if (hitEffectPrefab == null) return;

        // 적의 현재 위치 & 회전값으로 생성
        GameObject fx = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        // 자동 파괴(프리팹에 애니메이션/파티클 길이에 맞게 조절)
        if (autoDestroyAfter > 0f)
            Destroy(fx, autoDestroyAfter);
    }
    private IEnumerator BlinkRoutine(float duration, float blinkInterval)
    {
        float time = 0f;

        while (time < duration)
        {
            sr.enabled = !sr.enabled; // ON/OFF 토글
            yield return new WaitForSeconds(blinkInterval);
            time += blinkInterval;
        }

        sr.enabled = true; // 마지막에 켜둠
    }

    void UpdateHPBar() // 보스 이미지에 따른 체력 표시
    {
        BossHpFill.fillAmount = (float)PlayerStat.BossStamina / BossMaxHP;
    }
}
