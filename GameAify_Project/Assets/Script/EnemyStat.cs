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
    private float maxHP; // 적 기본체력
    private float currentHP; // 적 현재체력
    private bool OnVaccineDamage = false;
    public GameObject DieEffect; // 적 사망 시 이펙트

    public Image fillImage;
    void Awake()
    {
        // 이름 기반 maxHP 설정
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
                maxHP = 600f;
                EnemyAttack = 60f;
                EnemyMoveSpeed = 1.0f;
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
        if (collision.CompareTag("Player") && !PlayerMove.isInvincible)
        {
            if (collision.CompareTag("Capsule") && CapsuleState.CapsuleActive)
            {
                switch (PlayerStat.CapsuleLevel)
                {
                    case 0: PlayerStat.HP -= EnemyAttack * 0.7f; break;
                    case 1: PlayerStat.HP -= EnemyAttack * 0.5f; break;
                    case 2: PlayerStat.HP -= EnemyAttack * 0.3f; break;
                    default: PlayerStat.HP -= 0; break;
                }
            }
            else PlayerStat.HP -= EnemyAttack;
        }
        UpdateHPBar();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.name.StartsWith("VaccineFeild")) OnVaccineDamage = false;
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
                MainSpawnerAndTimer.SetDropPosition(collisionPosition); // 위치 전달
            }
        }
    }

    void Die()
    {
        PlayerStat.currentGauge++;
        MainSpawnerAndTimer.SpawnCount--;
        //디버그용
        //Debug.Log("Gauge : " + PlayerStat.currentGauge + ", Upgrade : " + PlayerAttack.NowCount);
        Instantiate(DieEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
