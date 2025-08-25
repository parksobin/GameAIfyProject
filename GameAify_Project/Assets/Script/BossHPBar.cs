using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    public Image  BossHpFill;   // 빨간 HP 이미지
    private float BossMaxHP = 10000f; //보스 전체 체력 저장
    private float currentHP = 10000f; //현재 체력
    private float duration = 0.4f; // 몇 초동안
    private float blinkInterval = 0.1f; // 몇 초 간격으로 깜빡이게 할 껀지
    public SpriteRenderer sr;

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
        PlayerStat.BossStamina -= damage;
        if (PlayerStat.BossStamina < 0) PlayerStat.BossStamina = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Weapon")) StartCoroutine(BlinkRoutine(duration, blinkInterval));
        
        if (collision.gameObject.name.StartsWith("Syringe")) TakeDamage(PlayerStat.SyringePower);
        else if (collision.gameObject.name.StartsWith("Drone")) TakeDamage(PlayerStat.DronePower);
        else if (collision.gameObject.name.StartsWith("Mess") || collision.gameObject.name.StartsWith("MessUnique")
            || collision.gameObject.name.StartsWith("MessBullet") || collision.gameObject.name.StartsWith("UniqueMessBullet"))
            TakeDamage(PlayerStat.MessPower);
        else if (collision.gameObject.name.StartsWith("VaccineFeild")) EnemyStat.OnVaccineDamage = true;
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
