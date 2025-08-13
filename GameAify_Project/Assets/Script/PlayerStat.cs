using NUnit.Framework.Internal;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static int maxGauge = 5000;
    public static int currentGauge = 0;

    public static int SyringeLevel = 1; // �ֻ�� �ܰ�
    public static int MessLevel = 1; // �޽� �ܰ�
    public static int DroneLevel = 1; // ��� �ܰ�
    public static int VaccineLevel = 1;  // ��� ���� �ܰ�
    public static int CapsuleLevel = 1; // ĸ�� �ܰ�

    public static float maxHP = 300f; // �÷��̾��� �⺻ ü��
    public static float HP = 300f; // �÷��̾��� ���� ü��
    public static float AttackRange = 10f; // �÷��̾��� �⺻ ���� ����
    public static float PlayerMoveSpeed = 10f/2; // �÷��̾� �⺻ �̵� �ӵ�
    public static float AttackSpeed = 1.5f; // �÷��̾��� �⺻ ���� �ӵ�
    public static float AttackPower = 100f; // �÷��̾��� �⺻ ���ݷ�

    public static float DronePower = AttackPower / 3.0f; // ��� : �÷��̾� ���ݷ��� 3���� 1
    public static float SyringePower = AttackPower / 4.0f; // �ֻ�� : �÷��̾� ���ݷ��� 4���� 1
    public static float VaccinePower = AttackPower / 10.0f; // ��� : �÷��̾� ���ݷ��� 10���� 1
    public static float MessPower = AttackPower / 2.0f; // �޽� : �÷��̾� ���ݷ��� 2���� 1

    public void SyringeLevelUp()
    {
        SyringeLevel++;
        ItemChecker.NowCount--;
    }
    public void MessLevelUp()
    {
        MessLevel++;
        ItemChecker.NowCount--;
    }
    public void DroneLevelUp()
    {
        DroneLevel++;
        ItemChecker.NowCount--;
    }
    public void VaccineLevelUp()
    {
        VaccineLevel++;
        ItemChecker.NowCount--;
    }
    public void CapsuleLevelUp()
    {
        CapsuleLevel++;
        ItemChecker.NowCount--;
    }
    public void HealthPointSet(float per)
    {
        maxHP *= (1f + per / 100f);
        ItemChecker.NowCount--;
    }
    public void AttackRangeSet(float per)
    {
        AttackRange *= (1f + per / 100f);
        ItemChecker.NowCount--;
    }
    public void PlayerSpeedSet(float per)
    {
        PlayerMoveSpeed *= (1f + per / 100f);
        ItemChecker.NowCount--;
    }
    public void AttackSpeedSet(float per)
    {
        AttackSpeed /= (1f + per / 100f);
        ItemChecker.NowCount--;
    }
    public void AttackPowerSet(float per)
    {
        AttackPower *= (1f + per / 100f);
        ItemChecker.NowCount--;
    }

    public static void CheckUniqueLevel()
    {
        int[] levels = { SyringeLevel, MessLevel, DroneLevel, VaccineLevel, CapsuleLevel };
        foreach (int level in levels)
        {
            if (level == 4) ItemChecker.NowCount++;
        }
    }
}
