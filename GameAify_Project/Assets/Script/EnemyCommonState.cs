using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemyCommonState : MonoBehaviour
{
    public EnemyStat enemyStat;
    private float VaccineFeildInTime = 0f;
    private GameObject playerObj; //플레이어 오브젝트
    private Vector2 direction; //플레이어쪽 방향
    private SpriteRenderer enemySR;
    public GameObject Square = null; //몬스터무기
    private SpriteRenderer SquareRender;
    private bool wait=false; // Virus2 대기 신호
    private float SpawnSquareTime; //Virus2 무기
    private float BossVirusWaitTime=0; //보스 바이러스 이동전 대기 시간ㄴ
    private float BossVirusSpeed = 5.0f;

    private Transform player;
    private Rigidbody2D rb;
    public float knockbackDuration = 0.2f;
    private bool isKnockback = false;
    private Coroutine knockCR;
    private bool MoveTo =false; //보스맵 바이러스 이동 사인 (이걸로 이동할 때 삭제 가능)

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // 태그로 플레이어 찾기
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }
    void Start()
    {
        playerObj = GameObject.Find("Player");
        enemySR = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        Spawn();
        Destroyobj();
        spriteFlip();
    }

    private void FixedUpdate()
    {
        PlayerFollow();
    }
    private void spriteFlip() // 스프라이트 방향 반전
    {
        if (playerObj.transform.position.x >= gameObject.transform.position.x) enemySR.flipX = true;
        else enemySR.flipX = false;
    }

    private void PlayerFollow() //플레이어 방향으로 따라가는 함수
    {
        float distance = Vector2.Distance(playerObj.transform.position, transform.position);
        
        if (gameObject.name==("Virus_BossMap(Clone)"))
        {
            BossVirusWaitTime += Time.deltaTime;
            if (BossVirusWaitTime > 1.0f)
            {
                direction = (playerObj.transform.position - transform.position).normalized;
                gameObject.transform.Translate(direction * BossVirusSpeed * Time.deltaTime);
                MoveTo=true;
            }
        }
        if (gameObject.name.StartsWith("Virus2") && distance <= 10f)
        {
            wait = true;
            return; // 거리가 3 이하이면 움직이지 않음
        }
        else
        {
            wait = false;

                direction = (playerObj.transform.position - transform.position).normalized;
                gameObject.transform.Translate(direction * enemyStat.EnemyMoveSpeed * Time.deltaTime);
           
        }
    }

    private void Spawn()
    {
        if(wait)
        {
            SpawnSquareTime += Time.deltaTime;
            if(SpawnSquareTime >=2.0f)
            {
                GameObject squareInstance = Instantiate(Square, transform.position, Quaternion.identity);
                SpawnSquareTime = 0.0f;
            }
        }
    }

    private void CheckVaccineState() //백신 상태 체력 감소 함수
    {
        switch (PlayerStat.VaccineLevel)
        {
            case 1:
                
                break;
            case 2:
                
                break;
            case 3:
                
                break;
            case 4:
               
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.name.StartsWith("Virus_BossMap")&& MoveTo)
        {
            if(collision.CompareTag("Player")) Destroy(gameObject);
            if (collision.CompareTag("Weapon"))
            {
                Destroy(gameObject);
            }
        }
        if(collision.gameObject.name.StartsWith("Drone"))
        {
            MoveBack(5f, 4f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) //백신 필드 1초뒤 삭제
    {
        if (gameObject.name.StartsWith("Virus_BossMap")) 
        {
            
        }
        else
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
    public void MoveBack(float force, float maxSpeed = 0f)
    {
        if (rb == null || player == null) return;

        if (knockCR != null) StopCoroutine(knockCR);
        knockCR = StartCoroutine(KnockbackRoutine(force, maxSpeed));
    }

    private IEnumerator KnockbackRoutine(float force, float maxSpeed)
    {
        isKnockback = true;

        // 넉백 방향: 플레이어 반대
        Vector2 dir = ((Vector2)transform.position - (Vector2)player.position).normalized;

        // 현재 추적 속도 제거 후 임펄스
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * force, ForceMode2D.Impulse);

        if (maxSpeed > 0f)
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);

        yield return new WaitForSeconds(knockbackDuration);

        // 넉백 잔여 속도 제거 후 추적 복귀
        rb.linearVelocity = Vector2.zero;
        isKnockback = false;
        knockCR = null;
    }
    private void Destroyobj() //정화 단계 100 달성시 모두 삭제
    {
        if (gameObject.name.StartsWith("Virus_BossMap")) { }
        else
        {
            if (PlayerStat.purificationClearposSign) Destroy(gameObject);
        }
    }
}
