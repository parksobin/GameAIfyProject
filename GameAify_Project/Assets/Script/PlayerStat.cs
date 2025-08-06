using NUnit.Framework.Internal;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static float maxGauge = 5000f;
    public static float currentGauge = 0f;

    public static int SyringeLevel = 1; // �ֻ�� �ܰ�
    public static int MessLevel = 1; // �޽� �ܰ�
    public static int DroneLevel = 1; // ��� �ܰ�
    public static int VaccineLevel = 1;  // ��� ���� �ܰ�
    public static int CapsuleLevel = 1; // ĸ�� �ܰ�

    public static float maxHP = 300f; // �÷��̾��� �⺻ ü��
    public static float HP = 300f; // �÷��̾��� ���� ü��
    public static float AttackRange = 10f; // �÷��̾��� �⺻ ���� ����
    public static float PlayerMoveSpeed = 5f; // �÷��̾� �⺻ �̵� �ӵ�
    public static float AttackSpeed = 1.5f; // �÷��̾��� �⺻ ���� �ӵ�
    public static float AttackPower = 30f; // �÷��̾��� �⺻ ���ݷ�

    public void SyringeLevelUp()
    {
        SyringeLevel += 1;
    }
    public void MessLevelUp()
    {
        MessLevel += 1;
    }
    public void DroneLevelUp()
    {
        DroneLevel += 1;
    }
    public void VaccineLevelUp()
    {
        VaccineLevel += 1;
    }
    public void CapsuleLevelUp()
    {
        CapsuleLevel += 1;
    }
    public void HealthPointSet(float per)
    {
        maxHP *= (1f + per / 100f);
    }
    public void AttackRangeSet(float per)
    {
        AttackRange *= (1f + per / 100f);
    }
    public void PlayerSpeedSet(float per)
    {
        PlayerMoveSpeed *= (1f + per / 100f);
    }
    public void AttackSpeedSet(float per)
    {
        AttackSpeed /= (1f + per / 100f);
    }
    public void AttackPowerSet(float per)
    {
        AttackPower *= (1f + per / 100f);
    }
}
