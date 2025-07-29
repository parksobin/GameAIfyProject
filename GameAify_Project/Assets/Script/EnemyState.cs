using TMPro;
using UnityEngine;

public class EnemyState : MonoBehaviour
{
    private int stamina=100;
    private float VaccineFeildInTime = 0f;

    private PlayerAttack playerAttack;
    private GameObject playerObj; //플레이어 오브젝트
    private Vector2 direction; //플레이어쪽 방향
    private float moveSpeed; //이동속도

    void Start()
    {
        playerObj = GameObject.Find("Player");
        playerAttack = playerObj.GetComponent<PlayerAttack>();
        moveSpeed = Random.Range(1, 4);
    }
    private void Update()
    {
        PlayerFollow();
    }


    private void PlayerFollow() //플레이어 방향으로 따라가는 함수
    {
        direction = (playerObj.transform.position - transform.position).normalized;
        gameObject.transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    private void CheckVaccineState() //백신 상태 체력 감소 함수
    {
        switch (PlayerAttack.VaccineLevel)
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
