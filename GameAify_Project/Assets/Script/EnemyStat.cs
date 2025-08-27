using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class EnemyStat : MonoBehaviour
{
    public float EnemyAttack; // 적 공격력
    public float EnemyMoveSpeed; // 적 이동속도
    private float maxHP; // 적 최대 체력
    private float currentHP; // 적 현재 체력
    public static bool OnVaccineDamage = false;
    public GameObject DieEffect; // 적 사망 이펙트

    public Image fillImage;
    void Awake()
    {
        // 적 스탯 부여
        switch (gameObject.name.Replace("(Clone)", ""))
        {
            case "Virus1":
                maxHP = 100f;
                EnemyAttack = 20f;
                EnemyMoveSpeed = 3.0f;
                break;
            case "Virus2":
                maxHP = 150f;
                EnemyAttack = 30f;
                EnemyMoveSpeed = 3.0f;
                break;
            case "RunningDog":
                maxHP = 50f;
                EnemyAttack = 20f;
                EnemyMoveSpeed = 5.0f;
                break;
            case "AppleBomber":
                maxHP = 200f;
                EnemyAttack = 40f;
                EnemyMoveSpeed = 2.0f;
                break;
            case "Snailer":
                maxHP = 1000f;
                EnemyAttack = 60f;
                EnemyMoveSpeed = 1.0f;
                break;
            case "Virus_BossMap":
                EnemyAttack = 20f;
                break;
        }
        currentHP = maxHP;
    }

    void Update()
    {
        if (OnVaccineDamage)
        {
            currentHP -= PlayerStat.VaccinePower * Time.deltaTime;
            UpdateHPBar();
            isDead();
        }
    }
    void UpdateHPBar()
    {
        if (fillImage != null)
        {
            float ratio = (float) currentHP / maxHP;
            fillImage.fillAmount = ratio;
        }
        if (currentHP <= 0) currentHP = 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.StartsWith("Syringe")) TakeDamage(PlayerStat.SyringePower);
        else if (collision.gameObject.name.StartsWith("Drone")) TakeDamage(PlayerStat.DronePower);
        else if (collision.gameObject.name.StartsWith("Mess") || collision.gameObject.name.StartsWith("MessUnique")
            || collision.gameObject.name.StartsWith("MessBullet") || collision.gameObject.name.StartsWith("UniqueMessBullet"))
            TakeDamage(PlayerStat.MessPower);
        else if (collision.gameObject.name.StartsWith("VaccineFeild")) OnVaccineDamage = true;
        else if (gameObject.name.StartsWith("Virus_BossMap"))
        {
            if (collision.CompareTag("Player") && !PlayerMove.isInvincible)
            {
                CapsuleDamageCalcurate(EnemyAttack);
                Destroy(gameObject);
            }
        }

        // ���� �÷��̾ ����� �� �÷��̾� ���� ���
        else if (collision.CompareTag("Player") && !PlayerMove.isInvincible)
        {
            CapsuleDamageCalcurate(EnemyAttack);
        }
        UpdateHPBar();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.name.StartsWith("VaccineFeild")) OnVaccineDamage = false;
    }

    public static void CapsuleDamageCalcurate(float EnemyAttack)
    {
        float damage = EnemyAttack;

        // 캡슐이 활성화된 경우에만 데미지를 감소시킴
        if (PlayerStat.CapsuleState)
        {
            switch (PlayerStat.CapsuleLevel)
            {
                case 1: // 캡슐 레벨 1: 30% 데미지 감소 (70%만 받음)
                    damage *= 0.7f; 
                    break;
                case 2: // 캡슐 레벨 2: 50% 데미지 감소 (50%만 받음)
                    damage *= 0.5f; 
                    break;
                case 3: // 캡슐 레벨 3: 70% 데미지 감소 (30%만 받음)
                    damage *= 0.3f; 
                    break;
                case 4: // 캡슐 레벨 4: 100% 데미지 감소 (완전 무적 상태)
                    damage = 0f; 
                    break;
                default:
                    // 캡슐 레벨이 0이거나 예상치 못한 값: 데미지 감소 효과 없음
                    break;
            }
        }

        // 최종 데미지가 0 이하가 되지 않도록 보정
        if (damage <= 0f) damage = 0f;
        PlayerStat.HP -= damage;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        isDead();
    }

    public void isDead()
    {
        if (currentHP <= 0)
        {
            Die();
            if (gameObject.name.StartsWith("AppleBomber"))
            {
                MainSpawnerAndTimer.isDropApple = true;
                Vector2 collisionPosition = transform.position;
                MainSpawnerAndTimer.SetDropPosition(collisionPosition); // ��ġ ����
            }
        }
    }

    void Die()
    {
        PlayerStat.currentGauge++;
        MainSpawnerAndTimer.SpawnCount--;
        //����׿�
        //Debug.Log("Gauge : " + PlayerStat.currentGauge + ", Upgrade : " + PlayerAttack.NowCount);
        Instantiate(DieEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    
}
