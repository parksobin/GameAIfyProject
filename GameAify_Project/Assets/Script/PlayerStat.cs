using NUnit.Framework.Internal;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static int maxGauge = 5000;
    public static int currentGauge = 0;

    public static int SyringeLevel = 1; // 주사기 단계
    public static int MessLevel = 1; // 메스 단계
    public static int DroneLevel = 1; // 드론 단계
    public static int VaccineLevel = 1;  // 백신 투하 단계
    public static int CapsuleLevel = 1; // 캡슐 단계

    public static float maxHP = 300f; // 플레이어의 기본 체력
    public static float HP = 300f; // 플레이어의 현재 체력
    public static float AttackRange = 10f; // 플레이어의 기본 공격 범위
    public static float PlayerMoveSpeed = 10f/2; // 플레이어 기본 이동 속도
    public static float AttackSpeed = 1.5f; // 플레이어의 기본 공격 속도
    public static float AttackPower = 100f; // 플레이어의 기본 공격력

    public static float DronePower = AttackPower / 3.0f; // 드론 : 플레이어 공격력의 3분의 1
    public static float SyringePower = AttackPower / 4.0f; // 주사기 : 플레이어 공격력의 4분의 1
    public static float VaccinePower = AttackPower / 10.0f; // 백신 : 플레이어 공격력의 10분의 1
    public static float MessPower = AttackPower / 2.0f; // 메스 : 플레이어 공격력의 2분의 1

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
