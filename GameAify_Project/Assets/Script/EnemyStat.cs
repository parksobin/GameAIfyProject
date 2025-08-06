using TMPro;
using UnityEngine;

public class EnemyStat : MonoBehaviour
{
    public static float MonDogAttack = 5f; // Mondog 공격력
    public static float SpikerAttack = 20f; // Spiker 공격력
    public static float SnailerAttack = 10f; // Snailer 공격력
    public static float VirusAttack = 10f; // Virus 공격력

    private float maxHP;
    private float currentHP;
    private TextMeshProUGUI hpText;

    void Awake()
    {
        // 이름 기반 HP 설정
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

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
