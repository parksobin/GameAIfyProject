using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    public Image  BossHpFill;   // 빨간 HP 이미지
    private float BossMaxHP = 10000f; //보스 전체 체력 저장
    private float currentHP = 10000f; //현재 체력

    void Start()
    {
        BossMaxHP = PlayerStat.BossStamina;
        BossCurrentHP = BossMaxHP;
        UpdateHPBar();
    }

    private void Update()
    {
        UpdateHPBar();
    }

    public void TakeDamage(float damage)
    {
        BossCurrentHP -= damage;
        if (BossCurrentHP < 0) BossCurrentHP = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.StartsWith("Syringe")) TakeDamage(PlayerStat.SyringePower);
        else if (collision.gameObject.name.StartsWith("Drone")) TakeDamage(PlayerStat.DronePower);
        else if (collision.gameObject.name.StartsWith("Mess") || collision.gameObject.name.StartsWith("MessUnique")
            || collision.gameObject.name.StartsWith("MessBullet") || collision.gameObject.name.StartsWith("UniqueMessBullet"))
            TakeDamage(PlayerStat.MessPower);
        else if (collision.gameObject.name.StartsWith("VaccineFeild")) EnemyStat.OnVaccineDamage = true;

        // 적이 플레이어에 닿았을 때 플레이어 피해 계산
        /*if (collision.CompareTag("Player") && !PlayerMove.isInvincible)
        {
            float damage = EnemyAttack;

            // 캡슐이 활성화된 경우에만 감소율 적용
            if (PlayerStat.CapsuleState)
            {
                switch (PlayerStat.CapsuleLevel)
                {
                    case 1: // 30% 감소 → 70%만 받음
                        damage *= 0.7f; break;
                    case 2: // 50% 감소
                        damage *= 0.5f; break;
                    case 3: // 70% 감소
                        damage *= 0.3f; break;
                    case 4: // 100% 감소(유니크)
                        damage = 0f; break;
                    default:
                        // 레벨 0 또는 정의 밖: 감소 없음
                        break;
                }
            }

            // 음수 방지 및 적용
            if (damage < 0f) damage = 0f;
            PlayerStat.HP -= damage;
        }*/

        UpdateHPBar();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.StartsWith("Syringe")) TakeDamage(PlayerStat.SyringePower);
        else if (collision.gameObject.name.StartsWith("Drone")) TakeDamage(PlayerStat.DronePower);
        else if (collision.gameObject.name.StartsWith("Mess") || collision.gameObject.name.StartsWith("MessUnique")
            || collision.gameObject.name.StartsWith("MessBullet") || collision.gameObject.name.StartsWith("UniqueMessBullet"))
            TakeDamage(PlayerStat.MessPower);
        else if (collision.gameObject.name.StartsWith("VaccineFeild")) EnemyStat.OnVaccineDamage = true;

        // 적이 플레이어에 닿았을 때 플레이어 피해 계산
        /*if (collision.CompareTag("Player") && !PlayerMove.isInvincible)
        {
            float damage = EnemyAttack;

            // 캡슐이 활성화된 경우에만 감소율 적용
            if (PlayerStat.CapsuleState)
            {
                switch (PlayerStat.CapsuleLevel)
                {
                    case 1: // 30% 감소 → 70%만 받음
                        damage *= 0.7f; break;
                    case 2: // 50% 감소
                        damage *= 0.5f; break;
                    case 3: // 70% 감소
                        damage *= 0.3f; break;
                    case 4: // 100% 감소(유니크)
                        damage = 0f; break;
                    default:
                        // 레벨 0 또는 정의 밖: 감소 없음
                        break;
                }
            }

            // 음수 방지 및 적용
            if (damage < 0f) damage = 0f;
            PlayerStat.HP -= damage;
        }*/

        UpdateHPBar();
    }
    void UpdateHPBar() // 보스 이미지에 따른 체력 표시
    {
        BossHpFill.fillAmount = (float)BossCurrentHP / BossMaxHP;
    }
}
