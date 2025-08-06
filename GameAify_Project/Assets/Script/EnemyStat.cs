using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyStat : MonoBehaviour
{
    public static float MonDogAttack = 5f; // Mondog ���ݷ�
    public static float SpikerAttack = 20f; // Spiker ���ݷ�
    public static float SnailerAttack = 10f; // Snailer ���ݷ�
    public static float VirusAttack = 10f; // Virus ���ݷ�

    private float maxHP;
    private float currentHP;
    private TextMeshProUGUI hpText;

    void Awake()
    {
        // �̸� ��� maxHP ����
        switch (gameObject.name.Replace("(Clone)", ""))
        {
            case "MonDog":
                maxHP = 20f;
                break;
            case "Spiker":
                maxHP = 20f;
                break;
            case "Snailer":
                maxHP = 50f;
                break;
            case "Virus":
                maxHP = 50f;
                break;
        }

        currentHP = maxHP;
        hpText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        if (hpText != null)
            hpText.text = currentHP.ToString();
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        PlayerStat.currentGauge += 1.0f;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Weapon"))
        {
            TakeDamage(PlayerStat.AttackPower);
        }
    }
}
