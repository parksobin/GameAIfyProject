using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public TextMeshProUGUI Syringe; // 아이템 선택창의 글씨
    public TextMeshProUGUI Mess;
    public TextMeshProUGUI Drone;
    public TextMeshProUGUI Vaccine;
    public TextMeshProUGUI Capsule;

    public TextMeshProUGUI Hp;
    public TextMeshProUGUI AttRange;
    public TextMeshProUGUI PlayerSpeed;
    public TextMeshProUGUI AttSpeed;
    public TextMeshProUGUI AttPower;

    public static int maxGauge = 5000; // 전체 정화 게이지
    public static int currentGauge = 0; // 현재 정화 게이지

    public static int purificationGauge = 0; //정화 게이지 퍼센트
    public static bool purificationClearposSign = false; // 정화 완료 여부 확인 
    public static int BossStamina = 10000; //보스 체력

    public static int SyringeLevel = 1; // 주사기 단계
    public static int MessLevel = 1; // 메스 단계
    public static int DroneLevel = 1; // 드론 단계
    public static int VaccineLevel = 1;  // 백신 투하 단계
    public static int CapsuleLevel = 1; // 캡슐 단계

    public int HpLevel = 0, AttRangeLevel = 0, PlayerSpeedLevel = 0, 
         AttSpeedLevel = 0, AttPowerLevel = 0; // 알약 수치 조정을 위한 변수 선언
    public static float VCFS = 0f; // 백신 구역의 크기

    public static float maxHP = 300f; // 플레이어의 기본 체력
    public static float HP = 300f; // 플레이어의 현재 체력
    public static float AttackRange = 10f; // 플레이어의 기본 공격 범위
    public static float PlayerMoveSpeed = 5f; // 플레이어 기본 이동 속도
    public static float AttackSpeed = 5.0f; // 플레이어의 기본 공격 속도
    public static float AttackPower = 100f; // 플레이어의 기본 공격력

    public static float DronePower = AttackPower / 3.0f; // 드론 : 플레이어 공격력의 3분의 1
    public static float SyringePower = AttackPower / 4.0f; // 주사기 : 플레이어 공격력의 4분의 1
    public static float VaccinePower = AttackPower / 10.0f; // 백신 : 플레이어 공격력의 10분의 1
    public static float MessPower = AttackPower / 2.0f; // 메스 : 플레이어 공격력의 2분의 1

    public void SyringeLevelUp() // 주사기 레벨업
    {
        if(SyringeLevel < 4)
        {
            SyringeLevel++;
            Syringe.text = "Level : " + SyringeLevel;
            CheckisSelected();
        }
        if (SyringeLevel == 4) Syringe.text = "Clear";
    }
    public void MessLevelUp() // 메스 레벨업
    {
        if(MessLevel < 4)
        {
            MessLevel++;
            Mess.text = "Level : " + MessLevel;
            CheckisSelected();
        }
        if (MessLevel == 4) Mess.text = "Clear";
    }
    public void DroneLevelUp() // 드론 레벨업
    {
        if(DroneLevel < 4)
        {
            DroneLevel++;
            Drone.text = "Level : " + DroneLevel;
            CheckisSelected();
        }
        if (DroneLevel == 4) Drone.text = "Clear";
    }
    public void VaccineLevelUp() // 백신 레벨업
    {
        if(VaccineLevel < 4)
        {
            VaccineLevel++;
            Vaccine.text = "Level : " + VaccineLevel;
            CheckisSelected();
        }
        if (VaccineLevel == 4) Vaccine.text = "Clear";
    }
    public void CapsuleLevelUp() // 캡슐 레벨업
    {
        if(CapsuleLevel < 4)
        {
            CapsuleLevel++;
            Capsule.text = "Level : " + CapsuleLevel;
            CheckisSelected();
        }
        if (CapsuleLevel == 4) Capsule.text = "Clear";
    }
    public void HealthPointSet() // 체력 증가
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
    public void AttackRangeSet() // 공격 범위 증가(백신 구역 범위도 같이 증가)
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
        Debug.Log("VCFS : " + VCFS);
    }
    public void PlayerSpeedSet() // 이동 속도 증가
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
    public void AttackSpeedSet() // 공격 속도 증가 (백신 쿨타임, 캡슐 쿨타임 감소)
    {
        if(AttSpeedLevel < 5)
        {
            AttackSpeed = 5.0f - (0.5f * (AttSpeedLevel + 1));
            AttSpeed.text = " + %" + ((AttSpeedLevel + 1) * 10).ToString();
            VaccineAndCapsuleCheck();
            AttSpeedLevel++;
            CheckisSelected();
        }
        if (AttSpeedLevel == 5) AttSpeed.text = "Clear";
    }
    public void AttackPowerSet() // 공격력 증가
    {
        if(AttPowerLevel < 5)
        {
            AttackPower += AttackPower * 0.1f - AttPowerLevel;
            AttPower.text = " + %" + ((AttPowerLevel + 1) * 10).ToString();
            AttPowerLevel++;
            CheckisSelected();
        }
        if (AttPowerLevel == 5) AttPower.text = "Clear";
    }

    void VaccineAndCapsuleCheck() // 백신, 캡슐 공격속도 함수
    {
        if (VaccineLevel <= 2) PlayerAttack.VaccineWaitSeconds -= 0.8f * (AttSpeedLevel + 1);
        else PlayerAttack.VaccineWaitSeconds -= 0.5f * (AttSpeedLevel + 1);
        if (CapsuleLevel == 4) PlayerAttack.CapsuleTime -= 1.5f * (AttSpeedLevel + 1);
        else PlayerAttack.CapsuleTime -= 2.0f * (AttSpeedLevel + 1);
    }

    void CheckisSelected() // 아이템을 선택했는지 확인하는 함수
    {
        PlayerAttack.NowCount--;
        if(PlayerAttack.NowCount <= 0) ItemChecker.SelectedItem = true;
    }

    public static void CheckUniqueLevel() // 유니크 레벨 달성 수 만큼 업그레이드 +1
    {
        int[] levels = { SyringeLevel, MessLevel, DroneLevel, VaccineLevel, CapsuleLevel };
        foreach (int level in levels)
        {
            if (level == 4) PlayerAttack.NowCount++;
        }
    }
}
