using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class StatDebug : MonoBehaviour
{
    public TMP_InputField SyringeLev;
    public TMP_InputField MessLev;
    public TMP_InputField DroneLev;
    public TMP_InputField VaccineLev;
    public TMP_InputField CapsuleLev;

    public void ApplySettings()
    {
        if (int.TryParse(SyringeLev.text, out int Syr))
            PlayerAttack.SyringeLevel = Syr;
        if (int.TryParse(MessLev.text, out int Mes))
            PlayerAttack.MessLevel = Mes;
        if (int.TryParse(DroneLev.text, out int Dro))
            PlayerAttack.DroneLevel = Dro;
        if (int.TryParse(VaccineLev.text, out int Vac))
            PlayerAttack.VaccineLevel = Vac;
        if (int.TryParse(CapsuleLev.text, out int Cap))
            PlayerAttack.CapsuleLevel = Cap;

        Debug.Log("설정 적용 완료");
        EventSystem.current.SetSelectedGameObject(null);
    }
}
