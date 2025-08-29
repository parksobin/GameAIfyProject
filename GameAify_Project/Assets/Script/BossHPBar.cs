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
    private bool canTakeDamage = true; // 데미지 무적 상태 확인
    
    // 애니메이션 컴포넌트 참조
    public Animator bossAnimator;

    void Start()
    {
        BossMaxHP = PlayerStat.BossStamina;
        sr = GetComponent<SpriteRenderer>();
        bossAnimator = GetComponent<Animator>();
        UpdateHPBar();
    }

    private void Update()
    {
        UpdateHPBar();
        CheckBossDownAnimation();
        //Debug.Log("보스 체력 : " + PlayerStat.BossStamina);
    }

    public void TakeDamage(float damage)
    {
        // 데미지 무적 상태일 때는 데미지를 입지 않음
        if (!canTakeDamage) return;
        
        PlayerStat.BossStamina -= damage;
        if (PlayerStat.BossStamina < 0) PlayerStat.BossStamina = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 데미지 무적 상태일 때는 이펙트도 생성하지 않고 데미지도 입지 않음
        if (collision.gameObject.layer == LayerMask.NameToLayer("Weapon")) 
        {
            // 무적 상태가 아닐 때만 이펙트 생성
            if (canTakeDamage)
            {
                SpawnRandomEffect();
            }
        }
        
        if (collision.gameObject.name.StartsWith("Syringe")) TakeDamage(PlayerStat.SyringePower);
        else if (collision.gameObject.name.StartsWith("Drone")) TakeDamage(PlayerStat.DronePower);
        else if (collision.gameObject.name.StartsWith("Mess") || collision.gameObject.name.StartsWith("MessUnique")
            || collision.gameObject.name.StartsWith("MessBullet") || collision.gameObject.name.StartsWith("UniqueMessBullet"))
            TakeDamage(PlayerStat.MessPower);
        else if (collision.gameObject.name.StartsWith("VaccineFeild")) EnemyStat.OnVaccineDamage = true;
    }
    
    private void SpawnRandomEffect()
    {
        if (hitEffectPrefab == null) return;

        // 보스 위치 기준으로 2유닛 거리에서 랜덤한 각도로 위치 계산
        float randomAngle = Random.Range(0f, 360f);
        float distance = 1.5f;
        
        Vector2 randomOffset = new Vector2(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad) * distance,
            Mathf.Sin(randomAngle * Mathf.Deg2Rad) * distance
        );
        
        Vector2 spawnPosition = (Vector2)transform.position + randomOffset;

        // 랜덤 위치에 이펙트 생성
        GameObject fx = Instantiate(hitEffectPrefab, spawnPosition, Quaternion.identity);

        // 자동 파괴(프리팹에 애니메이션/파티클 길이에 맞게 조절)
        if (autoDestroyAfter > 0f)
            Destroy(fx, autoDestroyAfter);
    }
    
    /*private void SpawnEffect()
    {
        if (hitEffectPrefab == null) return;

        // 적의 현재 위치 & 회전값으로 생성
        GameObject fx = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        // 자동 파괴(프리팹에 애니메이션/파티클 길이에 맞게 조절)
        if (autoDestroyAfter > 0f)
            Destroy(fx, autoDestroyAfter);
    }*/
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
    
    // StartBossDown 애니메이션 상태를 감지하여 데미지 무적 상태 제어
    private void CheckBossDownAnimation()
    {
        if (bossAnimator == null) return;
        
        // StartBossDown 애니메이션이 재생 중인지 확인
        if (bossAnimator.GetCurrentAnimatorStateInfo(0).IsName("StartBossDown"))
        {
            // 애니메이션이 재생 중이면 데미지 무적 상태
            canTakeDamage = false;
        }
        else
        {
            // 애니메이션이 끝나면 데미지를 입을 수 있는 상태로 변경
            canTakeDamage = true;
        }
    }
}
