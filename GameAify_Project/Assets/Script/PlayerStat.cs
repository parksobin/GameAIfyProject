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

    public void SyringeLevelUp(int Stat)
    {
        SyringeLevel += Stat;
    }
    public void MessLevelUp(int Stat)
    {
        MessLevel += Stat;
    }
    public void DroneLevelUp(int Stat)
    {
        DroneLevel += Stat;
    }
    public void VaccineLevelUp(int Stat)
    {
        VaccineLevel += Stat;
    }
    public void CapsuleLevelUp(int Stat)
    {
        CapsuleLevel += Stat;
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
