using TMPro;
using UnityEngine;

public class EnemyCommonState : MonoBehaviour
{
    public EnemyStat enemyStat;
    private int stamina = 100;
    private float VaccineFeildInTime = 0f;
    private GameObject playerObj; //플레이어 오브젝트
    private Vector2 direction; //플레이어쪽 방향
    private SpriteRenderer enemySR;

    void Start()
    {
        playerObj = GameObject.Find("Player");
        enemySR = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        spriteFlip();
        PlayerFollow();
    }
    private void spriteFlip()
    {
        if (playerObj.transform.position.x >= gameObject.transform.position.x) enemySR.flipX = true;
        else enemySR.flipX = false;
    }

    private void PlayerFollow() //플레이어 방향으로 따라가는 함수
    {
        float distance = Vector2.Distance(playerObj.transform.position, transform.position);
        if (gameObject.name.StartsWith("Virus2") && distance <= 10f) return; // 거리가 3 이하이면 움직이지 않음

        direction = (playerObj.transform.position - transform.position).normalized;
        gameObject.transform.Translate(direction * enemyStat.EnemyMoveSpeed * Time.deltaTime);
    }

    private void CheckVaccineState() //백신 상태 체력 감소 함수
    {
        switch (PlayerStat.VaccineLevel)
        {
            case 1:
                stamina -= 1;
                break;
            case 2:
                stamina -= 2;
                break;
            case 3:
                stamina -= 3;
                break;
            case 4:
                stamina -= 4;  
                break;
            default:
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Vaccine"))
        {
            VaccineFeildInTime += Time.deltaTime;
            if(VaccineFeildInTime >=1.0f)
            {
                CheckVaccineState();
                VaccineFeildInTime = 0.0f;
            }
        }
    }

}
