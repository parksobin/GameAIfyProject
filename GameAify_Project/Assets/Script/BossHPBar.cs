using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    public Image  BossHpFill;   // 빨간 HP 이미지
    private float BossMaxHP; //보스 전체 체력 저장
    private float currentHP; //현재 체력

    void Start()
    {
        BossMaxHP = PlayerStat.BossStamina;
        currentHP = BossMaxHP;
        UpdateHPBar();
    }

    private void Update()
    {
        TakeDamage(currentHP);
    }
    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;
        UpdateHPBar();
    }

    void UpdateHPBar() // 보스 이미지에 따른 체력 표시
    {
        currentHP = PlayerStat.BossStamina;
        BossHpFill.fillAmount = currentHP / BossMaxHP;
    }
}
