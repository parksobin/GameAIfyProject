using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class StatDebug : MonoBehaviour
{
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
    //public TMP_InputField AttackPower;

    public void ApplySettings()
    {
        if (int.TryParse(SyringeLev.text, out int Syr)) PlayerStat.SyringeLevel = Syr;
        if (int.TryParse(MessLev.text, out int Mes)) PlayerStat.MessLevel = Mes;
        if (int.TryParse(DroneLev.text, out int Dro)) PlayerStat.DroneLevel = Dro;
        if (int.TryParse(VaccineLev.text, out int Vac)) PlayerStat.VaccineLevel = Vac;
        if (int.TryParse(CapsuleLev.text, out int Cap)) PlayerStat.CapsuleLevel = Cap;

        if (float.TryParse(HealthPoint.text, out float Hea)) PlayerStat.HP = Hea;
        if (float.TryParse(AttackRange.text, out float Ara)) PlayerStat.AttackRange = Ara;
        if (float.TryParse(PlayerSpeed.text, out float Spe)) PlayerStat.PlayerMoveSpeed = Spe;
        if (float.TryParse(AttackSpeed.text, out float Asp)) PlayerStat.AttackSpeed = Asp;
        //if (float.TryParse(AttackPower.text, out float Apo)) PlayerStat.AttackPower = Apo;

        Debug.Log("설정 적용 완료");
        EventSystem.current.SetSelectedGameObject(null);
    }
}
