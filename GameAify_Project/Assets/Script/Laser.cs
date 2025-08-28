using UnityEngine;

public class Laser : MonoBehaviour
{
    private bool isCooldown = false;   // 데미지 쿨타임 여부
    private float delayTime = 0f;      // 경과 시간
    private float cooldownTime = 1f;   // 데미지 쿨타임 (1초)
    public static float BossAttackLaser = 30f;

    private void Update()
    {
        if(PlayerStat.BossStamina <=0)
        {
            this.gameObject.SetActive(false);
        }
        // 쿨타임 진행 중이면 시간 측정
        if (isCooldown)
        {
            delayTime += Time.deltaTime;
            if (delayTime >= cooldownTime)
            {
                // 쿨타임 종료
                isCooldown = false;
                delayTime = 0f;
            }
        }
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCooldown)
        {
            // 플레이어 체력 감소
            EnemyStat.CapsuleDamageCalcurate(BossAttackLaser);
        }
    }
}
