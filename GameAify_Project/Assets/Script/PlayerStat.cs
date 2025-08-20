using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public Button[] SelectBtn; // 아이템 선택 버튼

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
    public static int itemSelectCount = 0;
    public static float VCFS = 0f; // 백신 구역의 크기

    public static float maxHP = 300f; // 플레이어의 기본 체력
    public static float HP = 300f; // 플레이어의 현재 체력
    public static float AttackRange = 10f; // 플레이어의 기본 공격 범위
    public static float PlayerMoveSpeed = 5f; // 플레이어 기본 이동 속도
    public static float AttackSpeed = 5.0f; // 플레이어의 기본 공격 속도
    public static float AttackPower = 100f; // 플레이어의 기본 공격력

    public static float DronePower; // 드론 공격력
    public static float SyringePower; // 주사기 공격력
    public static float VaccinePower; // 백신 공격력
    public static float MessPower = AttackPower / 2.0f; // 메스 공격력

    public void DroneLevelUp() // 드론 레벨업
    {
        if (DroneLevel < 4)
        {
            DroneLevel++;
            Drone.text = "Level : " + DroneLevel;
            CheckisSelected();
        }
        CheckClear(0, DroneLevel, Drone);
    }

    public void SyringeLevelUp() // 주사기 레벨업
    {
        if (SyringeLevel < 4)
        {
            SyringeLevel++;
            Syringe.text = "Level : " + SyringeLevel;
            CheckisSelected();
        }
        CheckClear(1, SyringeLevel, Syringe);
    }
    public void VaccineLevelUp() // 백신 레벨업
    {
        if (VaccineLevel < 4)
        {
            VaccineLevel++;
            Vaccine.text = "Level : " + VaccineLevel;
            CheckisSelected();
        }
        CheckClear(2, VaccineLevel, Vaccine);
    }
    public void MessLevelUp() // 메스 레벨업
    {
        if (MessLevel < 4)
        {
            MessLevel++;
            Mess.text = "Level : " + MessLevel;
            CheckisSelected();
        }
        CheckClear(3, MessLevel, Mess);
    }
    public void CapsuleLevelUp() // 캡슐 레벨업
    {
        if (CapsuleLevel < 4)
        {
            CapsuleLevel++;
            Capsule.text = "Level : " + CapsuleLevel;
            CheckisSelected();
        }
        CheckClear(4, CapsuleLevel, Capsule);
    }
    public void HealthPointSet() // 체력 증가
    {
        if (HpLevel < 5)
        {
            float PlusHP = maxHP * 0.1f - HpLevel;
            HP += PlusHP;
            maxHP += PlusHP;
            Hp.text = " + %" + ((HpLevel + 1) * 10).ToString();
            HpLevel++;
            CheckisSelected();
        }
        CheckClear(5, HpLevel, Hp, 5);
    }
    public void AttackPowerSet() // 공격력 증가
    {
        if (AttPowerLevel < 5)
        {
            AttackPower += AttackPower * 0.1f - AttPowerLevel;
            AttPower.text = " + %" + ((AttPowerLevel + 1) * 10).ToString();
            AttPowerLevel++;
            CheckisSelected();
        }
        CheckClear(6, AttPowerLevel, AttPower, 5);
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
        CheckClear(7, PlayerSpeedLevel, PlayerSpeed, 5);
    }
    public void AttackSpeedSet() // 공격 속도 증가 (백신 쿨타임, 캡슐 쿨타임 감소)
    {
        if (AttSpeedLevel < 5)
        {
            AttackSpeed = 5.0f - (0.5f * (AttSpeedLevel + 1));
            AttSpeed.text = " + %" + ((AttSpeedLevel + 1) * 10).ToString();
            VaccineAndCapsuleCheck();
            AttSpeedLevel++;
            CheckisSelected();
        }
        CheckClear(8, AttSpeedLevel, AttSpeed, 5);
    }
    public void AttackRangeSet() // 공격 범위 증가(백신 구역 범위도 같이 증가)
    {
        if (AttRangeLevel < 5)
        {
            AttackRange += (AttRangeLevel + 1.0f);
            AttRange.text = " + %" + ((AttRangeLevel + 1) * 10).ToString();
            if (VaccineLevel == 4) VCFS = 3.0f + 0.3f * (AttRangeLevel + 1);
            else VCFS = 1.5f + (0.15f * (AttRangeLevel + 1));
            AttRangeLevel++;
            CheckisSelected();
        }
        CheckClear(9, AttRangeLevel, AttRange, 5);
        Debug.Log("VCFS : " + VCFS);
    }

    void CheckClear(int btnNum, int level, TextMeshProUGUI text, int maxValue = 4)
    {
        if (level == maxValue)
        {
            text.text = "Clear";
            itemSelectCount++;
            SelectBtn[btnNum].interactable = false;
        }
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
        if (PlayerAttack.NowCount <= 0) ItemChecker.SelectedItem = true;
    }

    public static void CheckUniqueLevel() // 유니크 레벨 달성 수 만큼 업그레이드 +1
    {
        int[] levels = { SyringeLevel, MessLevel, DroneLevel, VaccineLevel, CapsuleLevel };
        foreach (int level in levels)
        {
            if (level == 4)
            {
                PlayerAttack.NowCount++;
                if (PlayerAttack.NowCount >= 3) PlayerAttack.NowCount = 3;
            }
        }
    }
}
