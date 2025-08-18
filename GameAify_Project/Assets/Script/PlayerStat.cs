using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public TextMeshProUGUI Syringe; // ������ ����â�� �۾�
    public TextMeshProUGUI Mess;
    public TextMeshProUGUI Drone;
    public TextMeshProUGUI Vaccine;
    public TextMeshProUGUI Capsule;

    public TextMeshProUGUI Hp;
    public TextMeshProUGUI AttRange;
    public TextMeshProUGUI PlayerSpeed;
    public TextMeshProUGUI AttSpeed;
    public TextMeshProUGUI AttPower;

    public static int maxGauge = 5000;
    public static int currentGauge = 0;

    public static int SyringeLevel = 1; // �ֻ�� �ܰ�
    public static int MessLevel = 1; // �޽� �ܰ�
    public static int DroneLevel = 1; // ��� �ܰ�
    public static int VaccineLevel = 1;  // ��� ���� �ܰ�
    public static int CapsuleLevel = 1; // ĸ�� �ܰ�

    public int HpLevel = 0, AttRangeLevel = 0, PlayerSpeedLevel = 0, 
         AttSpeedLevel = 0, AttPowerLevel = 0; // �˾� ��ġ ������ ���� ���� ����
    public static float VCFS = 0f; // ��� ������ ũ��

    public static float maxHP = 300f; // �÷��̾��� �⺻ ü��
    public static float HP = 300f; // �÷��̾��� ���� ü��
    public static float AttackRange = 10f; // �÷��̾��� �⺻ ���� ����
    public static float PlayerMoveSpeed = 5f; // �÷��̾� �⺻ �̵� �ӵ�
    public static float AttackSpeed = 2.0f; // �÷��̾��� �⺻ ���� �ӵ�
    public static float AttackPower = 100f; // �÷��̾��� �⺻ ���ݷ�

    public static float DronePower = AttackPower / 3.0f; // ��� : �÷��̾� ���ݷ��� 3���� 1
    public static float SyringePower = AttackPower / 4.0f; // �ֻ�� : �÷��̾� ���ݷ��� 4���� 1
    public static float VaccinePower = AttackPower / 5.0f; // ��� : �÷��̾� ���ݷ��� 10���� 1
    public static float MessPower = AttackPower / 2.0f; // �޽� : �÷��̾� ���ݷ��� 2���� 1

    public void SyringeLevelUp() // �ֻ�� ������
    {
        if (SyringeLevel < 4)
        {
            SyringeLevel++;
            Syringe.text = "Level : " + SyringeLevel;
            CheckisSelected();
        }
        if(SyringeLevel == 4) Syringe.text = "Clear";
    }
    public void MessLevelUp() // �޽� ������
    {
        if(MessLevel < 4)
        {
            MessLevel++;
            Mess.text = "Level : " + MessLevel;
            CheckisSelected();
        }
        if (MessLevel == 4) Mess.text = "Clear";
    }
    public void DroneLevelUp() // ��� ������
    {
        if(DroneLevel < 4)
        {
            DroneLevel++;
            Drone.text = "Level : " + DroneLevel;
            CheckisSelected();
        }
        if (DroneLevel == 4) Drone.text = "Clear";
    }
    public void VaccineLevelUp() // ��� ������
    {
        if (VaccineLevel < 4)
        {
            VaccineLevel++;
            Vaccine.text = "Level : " + VaccineLevel;
            CheckisSelected();
        }
        if (VaccineLevel == 4) Vaccine.text = "Clear";
    }
    public void CapsuleLevelUp() // ĸ�� ������
    {
        if (CapsuleLevel < 4)
        {
            CapsuleLevel++;
            Capsule.text = "Level : " + CapsuleLevel;
            CheckisSelected();
        }
        if (CapsuleLevel == 4) Capsule.text = "Clear";
    }
    public void HealthPointSet() // ü�� ����
    {
        if(HpLevel < 5)
        {
            maxHP += maxHP * 0.1f - HpLevel;
            Hp.text = " + %" + ((HpLevel + 1) * 10).ToString();
            HpLevel++;
            CheckisSelected();
        }
        if (HpLevel == 5) Hp.text = "Clear";
    }
    public void AttackRangeSet() // ���� ���� ����(��� ���� ������ ���� ����)
    {
        if(AttRangeLevel < 5)
        {
            AttackRange += AttackRange * 0.1f - AttRangeLevel;
            AttRange.text = " + %" + ((AttRangeLevel + 1) * 10).ToString();
            if (VaccineLevel == 4) VCFS = 3.0f + 0.3f * (AttRangeLevel + 1);
            else VCFS = 1.5f + (0.15f * (AttRangeLevel + 1));
            AttRangeLevel++;
            CheckisSelected();
        }
        if (AttRangeLevel == 5) AttRange.text = "Clear";
        //Debug.Log("VCFS : " + VCFS);
    }
    public void PlayerSpeedSet() // �̵� �ӵ� ����
    {
        if (PlayerSpeedLevel < 5)
        {
            PlayerMoveSpeed += 0.5f;
            PlayerSpeed.text = " + %" + ((PlayerSpeedLevel + 1) * 10).ToString();
            PlayerSpeedLevel++;
            CheckisSelected();
        }
        if (PlayerSpeedLevel == 5) PlayerSpeed.text = "Clear";
    }
    public void AttackSpeedSet() // ���� �ӵ� ���� (��� ��Ÿ��, ĸ�� ��Ÿ�� ����)
    {
        if (AttSpeedLevel < 5)
        {
            AttackSpeed = 5.0f - (0.5f * (AttSpeedLevel + 1));
            AttSpeed.text = " + %" + ((AttSpeedLevel + 1) * 10).ToString();
            VaccineAndCapsuleCheck();
            AttSpeedLevel++;
            CheckisSelected();
        }
        if (AttSpeedLevel == 5) AttSpeed.text = "Clear";
    }
    public void AttackPowerSet() // ���ݷ� ����
    {
        if (AttPowerLevel < 5)
        {
            AttackPower += AttackPower * 0.1f - AttPowerLevel;
            AttPower.text = " + %" + ((AttPowerLevel + 1) * 10).ToString();
            AttPowerLevel++;
            CheckisSelected();
        }
        if(AttPowerLevel == 5) AttPower.text = "Clear";
    }

    void VaccineAndCapsuleCheck() // ���, ĸ�� ���ݼӵ� �Լ�
    {
        if (VaccineLevel <= 2) PlayerAttack.VaccineWaitSeconds -= 0.8f * (AttSpeedLevel + 1);
        else PlayerAttack.VaccineWaitSeconds -= 0.5f * (AttSpeedLevel + 1);
        if (CapsuleLevel == 4) PlayerAttack.CapsuleTime -= 1.5f * (AttSpeedLevel + 1);
        else PlayerAttack.CapsuleTime -= 2.0f * (AttSpeedLevel + 1);
    }

    void CheckisSelected() // �������� �����ߴ��� Ȯ���ϴ� �Լ�
    {
        PlayerAttack.NowCount--;
        if(PlayerAttack.NowCount <= 0) ItemChecker.SelectedItem = true;
    }

    public static void CheckUniqueLevel() // ����ũ ���� �޼� �� ��ŭ ���׷��̵� +1
    {
        int[] levels = { SyringeLevel, MessLevel, DroneLevel, VaccineLevel, CapsuleLevel };
        foreach (int level in levels)
        {
            if (level == 4) PlayerAttack.NowCount++;
        }
    }
}
