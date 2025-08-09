using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyStat : MonoBehaviour
{
    public float EnemyAttack; // �� ���ݷ�
    public float EnemyMoveSpeed; // �� �̵��ӵ�
    private float maxHP; // �� �⺻ü��
    private float currentHP; // �� ����ü��
    private TextMeshProUGUI hpText;
    private bool OnVaccineDamage = false;
    void Awake()
    {
        // �̸� ��� maxHP ����
        switch (gameObject.name.Replace("(Clone)", ""))
        {
            case "Virus1":
                maxHP = 100f;
                EnemyAttack = 20f;
                EnemyMoveSpeed = 6.0f;
                break;
            case "Virus2":
                maxHP = 150f;
                EnemyAttack = 30f;
                EnemyMoveSpeed = 10.0f;
                break;
            case "RunningDog":
                maxHP = 100f;
                EnemyAttack = 20f;
                EnemyMoveSpeed = 15f;
                break;
            case "AppleBomber":
                maxHP = 200f;
                EnemyAttack = 40f;
                EnemyMoveSpeed = 7.0f;
                break;
            case "Snailer":
                maxHP = 600f;
                EnemyAttack = 60f;
                EnemyMoveSpeed = 3.0f;
                break;
        }

        currentHP = maxHP;
        hpText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        if (hpText != null) hpText.text = currentHP.ToString("N0");
        if (OnVaccineDamage)
        {
            currentHP -= PlayerStat.VaccinePower * Time.deltaTime;
            isDead();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.StartsWith("Syringe")) TakeDamage(PlayerStat.SyringePower);
        else if (collision.gameObject.name.StartsWith("Drone")) TakeDamage(PlayerStat.DronePower);
        else if (collision.gameObject.name.StartsWith("Mess") || 
            collision.gameObject.name.StartsWith("UniqueMess")) TakeDamage(PlayerStat.MessPower);
        else if (collision.gameObject.name.StartsWith("VaccineFeild")) OnVaccineDamage = true;
        if (collision.CompareTag("Player")) PlayerStat.HP -= EnemyAttack;
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
        if (currentHP <= 0) Die();
    }

    void Die()
    {
        PlayerStat.currentGauge += 1.0f;
        Destroy(gameObject);
    }
}
