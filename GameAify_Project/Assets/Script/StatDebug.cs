using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class StatDebug : MonoBehaviour
{
    // �÷��̾� ������ ����
    public TMP_InputField SyringeLev;
    public TMP_InputField MessLev;
    public TMP_InputField DroneLev;
    public TMP_InputField VaccineLev;
    public TMP_InputField CapsuleLev;
    // �÷��̾� ���� ����
    public TMP_InputField HealthPoint;
    public TMP_InputField AttackRange;
    public TMP_InputField PlayerSpeed;
    public TMP_InputField AttackSpeed;
    //public TMP_InputField AttackPower;

    public void ApplySettings()
    {
        if (int.TryParse(SyringeLev.text, out int Syr)) PlayerAttack.SyringeLevel = Syr;
        if (int.TryParse(MessLev.text, out int Mes)) PlayerAttack.MessLevel = Mes;
        if (int.TryParse(DroneLev.text, out int Dro)) PlayerAttack.DroneLevel = Dro;
        if (int.TryParse(VaccineLev.text, out int Vac)) PlayerAttack.VaccineLevel = Vac;
        if (int.TryParse(CapsuleLev.text, out int Cap)) PlayerAttack.CapsuleLevel = Cap;

        if (float.TryParse(HealthPoint.text, out float Hea)) PlayerAttack.HP = Hea;
        if (float.TryParse(AttackRange.text, out float Ara)) PlayerAttack.AttackRange = Ara;
        if (float.TryParse(PlayerSpeed.text, out float Spe)) PlayerAttack.PlayerMoveSpeed = Spe;
        if (float.TryParse(AttackSpeed.text, out float Asp)) PlayerAttack.AttackSpeed = Asp;
        //if (float.TryParse(AttackPower.text, out float Apo)) PlayerAttack.AttackPower = Apo;

        Debug.Log("���� ���� �Ϸ�");
        EventSystem.current.SetSelectedGameObject(null);
    }
}
