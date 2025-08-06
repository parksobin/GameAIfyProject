using NUnit.Framework.Internal;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static float maxGauge = 5000f;
    public static float currentGauge = 0f;

    public static int SyringeLevel = 1; // 주사기 단계
    public static int MessLevel = 1; // 메스 단계
    public static int DroneLevel = 1; // 드론 단계
    public static int VaccineLevel = 1;  // 백신 투하 단계
    public static int CapsuleLevel = 1; // 캡슐 단계

    public static float maxHP = 300f; // 플레이어의 기본 체력
    public static float HP = 300f; // 플레이어의 현재 체력
    public static float AttackRange = 10f; // 플레이어의 기본 공격 범위
    public static float PlayerMoveSpeed = 5f; // 플레이어 기본 이동 속도
    public static float AttackSpeed = 1.5f; // 플레이어의 기본 공격 속도
    public static float AttackPower = 30f; // 플레이어의 기본 공격력

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
