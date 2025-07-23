using System.Xml.Linq;
using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform player;

    private SpriteRenderer sr; //캐릭터 기본 스프라이트 렌더러
    public Sprite idleSprite; //기본 이미지
    public Sprite walkUpSprite; // 캐릭터 뒷모습 이미지
    private Rigidbody2D rb;
    private Vector2 movement;

    public static float HP = 5.0f;
    public TextMeshProUGUI hpText;

    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr =  this.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 입력 처리
        movement.x = Input.GetAxisRaw("Horizontal"); 
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize(); // 대각선 속도 보정

        //플레이어 입력 이동에 따른
        if (Input.GetKey(KeyCode.D))
        {
            sr.flipX = false;
            walkAni(true);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            sr.flipX = true;
            walkAni(true );
        }
        else if (Input.GetKey(KeyCode.W))
        {
            walkAni(false);
            sr.sprite = walkUpSprite;
        }
        else
        {
            walkAni(false);
            sr.sprite = idleSprite;
        }
        CheckHP();
        hpText.text = "HP : " + HP.ToString("N1");
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    //HP 체크
    void CheckHP()
    {
        if (HP >= 5.0f) HP = 5.0f;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            HP -= 1.0f;
        }
    }

    private void walkAni(bool state)
    {
        // 스프라이트 덮어씌우기 위한 애니메시터 상태제어
        if (state) animator.enabled = true;
        else animator.enabled = false;
        //좌우 걷기 애니메이션
        animator.SetBool("walk", state);
    }
}
