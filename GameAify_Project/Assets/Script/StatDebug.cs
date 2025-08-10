using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class StatDebug : MonoBehaviour
{
    public TMP_InputField Wave;
    private float wave;
    // 플레이어 아이템 관련
    public TMP_InputField SyringeLev;
    public TMP_InputField MessLev;
    public TMP_InputField DroneLev;
    public TMP_InputField VaccineLev;
    public TMP_InputField CapsuleLev;

    // 플레이어 스탯 관련
    public TMP_InputField HealthPoint;
    public TMP_InputField AttackRange;
    public TMP_InputField PlayerSpeed;
    public TMP_InputField AttackSpeed;
    public TMP_InputField AttackPower;

    public TextMeshProUGUI waveText;
    public TextMeshProUGUI SyrText;
    public TextMeshProUGUI MesText;
    public TextMeshProUGUI DroText;
    public TextMeshProUGUI VacText;
    public TextMeshProUGUI CapText;

    public TextMeshProUGUI HeaText;
    public TextMeshProUGUI AraText;
    public TextMeshProUGUI SpeText;
    public TextMeshProUGUI AspText;
    public TextMeshProUGUI AtpText;

    public void Start()
    {
        UpdateStatus();
    }

    public void ApplySettings()
    {
        if (int.TryParse(SyringeLev.text, out int Syr)) PlayerStat.SyringeLevel = Syr;
        if (int.TryParse(MessLev.text, out int Mes)) PlayerStat.MessLevel = Mes;
        if (int.TryParse(DroneLev.text, out int Dro)) PlayerStat.DroneLevel = Dro;
        if (int.TryParse(VaccineLev.text, out int Vac)) PlayerStat.VaccineLevel = Vac;
        if (int.TryParse(CapsuleLev.text, out int Cap)) PlayerStat.CapsuleLevel = Cap;

        if (float.TryParse(HealthPoint.text, out float Hea)) PlayerStat.maxHP = Hea;
        if (float.TryParse(AttackRange.text, out float Ara)) PlayerStat.AttackRange = Ara;
        if (float.TryParse(PlayerSpeed.text, out float Spe)) PlayerStat.PlayerMoveSpeed = Spe;
        if (float.TryParse(AttackSpeed.text, out float Asp)) PlayerStat.AttackSpeed = Asp;
        if (float.TryParse(AttackPower.text, out float Apo)) PlayerStat.AttackPower = Apo;

        if (float.TryParse(Wave.text, out float wav))
        {
            MainSpawnerAndTimer.timeRemaining = 900f;
            MainSpawnerAndTimer.timeRemaining -= (wav - 1) * 45.0f;
            wave = wav;
        }

        UpdateStatus();

        Debug.Log("설정 적용 완료");
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void UpdateStatus()
    {
        waveText.text = "Wave : " + wave.ToString("N0");

        SyrText.text = "Syringe : " + PlayerStat.SyringeLevel.ToString();
        MesText.text = "Mess : " + PlayerStat.MessLevel.ToString();
        DroText.text = "Drone : " + PlayerStat.DroneLevel.ToString();
        VacText.text = "Vaccine : " + PlayerStat.VaccineLevel.ToString();
        CapText.text = "Capsule : " + PlayerStat.CapsuleLevel.ToString();

        HeaText.text = "maxHP : " + PlayerStat.maxHP.ToString("N1");
        AraText.text = "Att Range : " + PlayerStat.AttackRange.ToString();
        SpeText.text = "Mov Speed : " + PlayerStat.PlayerMoveSpeed.ToString();
        AspText.text = "Att Speed : " + PlayerStat.AttackSpeed.ToString();
        AtpText.text = "maxHP : " + PlayerStat.AttackPower.ToString("N1");
    }
}
