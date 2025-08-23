using System;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public Transform player;
    private SpriteRenderer sr; // 캐릭터 기본 스프라이트 렌더러
    public Sprite[] PlayerSprite; //0 기본 이미지 / 1 Die IMG
    private Rigidbody2D rb;
    private Vector2 movement;

    public TextMeshProUGUI hpText;
    private Animator animator;
    public Image fillImage; // HP바 오브젝트
    public Image gaugeFillImage; // 정화게이지 오브젝트

    public StageSetting stageSetting;
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

        if(PlayerStat.HP <=0) // 플레이어 체력 0시 죽는 스프라이트로 변경 && 애니메이터, 이동 불가
        {
            sr.sprite = PlayerSprite[1];
            animator.enabled = false;
        }
        else
        {
            //플레이어 입력 이동에 따른
            if (Input.GetKey(KeyCode.D))
            {
                sr.flipX = true;
                walkAni("walk", true,true,false,false);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                sr.flipX = false;
                walkAni("walk", true, true, false, false);
            }
            else if (Input.GetKey(KeyCode.W))
            {
                walkAni("walkUp", true,false,true,false);
            }
            else if(Input.GetKey(KeyCode.S))
            {
                walkAni("walkDown", true,false,false,true);
            }
            else
            {
                walkAni(null,false,false,false,false);
                sr.sprite = PlayerSprite[0];
            }
        }

        //스킬 사용 애니 작성칸입니다
        /*
        if()
        {
            walkAni("skill", true,false,false,true);
        }
        */
        hpText.text = PlayerStat.HP.ToString("N0"); 
        UpdateHPBar();
        UpdateGauge();
    }
    public void UpdateGauge()
    {
        if (gaugeFillImage != null)
            gaugeFillImage.fillAmount = (float)PlayerStat.currentGauge / PlayerStat.maxGauge;
    }
    void UpdateHPBar()
    {
        if (fillImage != null)
        {
            float ratio = (float)PlayerStat.HP / PlayerStat.maxHP;
            fillImage.fillAmount = ratio;
        }
        if (PlayerStat.HP <= 0) PlayerDead();
        if (PlayerStat.HP >= 450) PlayerStat.HP = 450;
    }

    private void PlayerDead()
    {
        
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * PlayerStat.PlayerMoveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UpdateHPBar();
        if (collision.CompareTag("Apple"))
        {
            PlayerStat.HP += 100f;
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("AppleDmg")) 
        {
            PlayerStat.HP -= 50f;
            Destroy(collision.gameObject);
        }
        if(collision.CompareTag("Spear"))
        {
            PlayerStat.HP -= 30f;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.name== "BossDoor") //보스맵으로 이동
        {
            stageSetting.InBossStage();
        }
    }

    private void walkAni(string aniName, bool state,bool ani1, bool ani2,bool ani3)
    {
        // 스프라이트 덮어씌우기 위한 애니메시터 상태제어
        if (state)
        {
            animator.enabled = true;
            animator.SetBool(aniName, state); //애니메이션재생
            
            //다른 애니메이션 상태제어
            animator.SetBool("walk", ani1);
            animator.SetBool("walkUp", ani2);
            animator.SetBool("walkDown", ani3);
            animator.SetBool("skill", false);
            
            // 스킬애니메이션 작동시 다른 애니 중복 제어
            if(aniName == "skill")
            {
                animator.SetBool("walk", false);
                animator.SetBool("walkUp", false);
                animator.SetBool("walkDown", false);
            }
        }
        else animator.enabled = false; //애니메이터 비활성호ㅓ -> hold상태
    }
}
