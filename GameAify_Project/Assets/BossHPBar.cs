using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    public Image  BossHpFill;   // ���� HP �̹���
    private float BossMaxHP;
    private float currentHP;

    void Start()
    {
        BossMaxHP = PlayerStat.BossStamina;
        currentHP = BossMaxHP;
        UpdateHPBar();
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;
        UpdateHPBar();
    }

    void UpdateHPBar()
    {
        currentHP = PlayerStat.BossStamina;
        BossHpFill.fillAmount = currentHP / BossMaxHP;
    }
}
