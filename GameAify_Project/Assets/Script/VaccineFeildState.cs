using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
public class VaccineFeildState : MonoBehaviour
{
    private float appliedBonus = 0f; // 현재 적용한 보너스(0이면 미적용)

    private static int s_Counter = 0;  // 전역 공유 카운터
    public int baseOrder = 1;          // 1부터 시작

    void OnEnable()
    {
        var sg = GetComponent<SortingGroup>();

        // 1 → 2 → 3 → 다시 1 …
        int order = baseOrder + (s_Counter % 3);

        sg.sortingOrder = order;

        s_Counter++;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && PlayerStat.VaccineLevel == 4 && appliedBonus == 0f)
        {
            PlayerStat.AttackPower += 10f;
            appliedBonus = 10f;
            Debug.Log("AttackPower : " + PlayerStat.AttackPower);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            RemoveBonusOnce();
        }
    }

    private void OnDestroy()
    {
        RemoveBonusOnce();
    }

    // SetActive(false)로 비활성화하는 경우까지 커버하고 싶으면 추가
    private void OnDisable()
    {
        RemoveBonusOnce();
    }

    private void RemoveBonusOnce()
    {
        if (appliedBonus != 0f)
        {
            PlayerStat.AttackPower -= appliedBonus;
            Debug.Log("AttackPower  종료 : " + PlayerStat.AttackPower);
            appliedBonus = 0f; // 재진입 대비 초기화
        }
    }

}
