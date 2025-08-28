using System;
using System.Collections;
using System.Security.Cryptography;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public Transform player;
    private SpriteRenderer sr; // 캐릭터 기본 스프라이트 렌더러
    public Sprite[] PlayerSprite; //0 기본 이미지 / 1 Die IMG
    private Rigidbody2D rb;
    private Vector2 movement;
    public GameObject MainCanvas;

    public TextMeshProUGUI hpText;
    private Animator animator;
    public Image fillImage; // HP바 오브젝트
    public Image gaugeFillImage; // 정화게이지 오브젝트

    public StageSetting stageSetting;
    public Image PlayerFace;
    public Sprite[] PlayerFaces;
    public static bool isInvincible; // 무적 상태 여부
    private float invincibleDuration = 1.0f; // 무적 시간
    private float blinkInterval = 0.1f; // 깜빡임 간격
    private string playerLayerName = "Player";
    private string enemyLayerName = "Enemy";
    private int playerLayer;
    private int enemyLayer;
    private bool VaccineSign =false;

    private bool running; // 게임이 진행중인가?
    public float duration = 1.2f;    // 연출 시간(초) — Unscaled
    public AnimationCurve curve = null; // 부드러운 가감속용 (없으면 기본 생성)
    public GameObject fadeOverlay;          // 화면 전체를 덮는 검은색 오브젝트 (알파 0으로 시작)
    public GameObject gameOverUI;    // 게임오버 패널 (처음엔 비활성)
    public GameObject CapsuleItem; // 캡슐 끄기용
    public GameObject ResetBtn; // 게임 다시 하기 버튼
    public GameObject BossGameOver; // 보스 맵에서 게임 오버
    public GameObject ClearUI; // 보스 맵 클리어 패널

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr =  this.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerLayer = LayerMask.NameToLayer(playerLayerName);
        enemyLayer = LayerMask.NameToLayer(enemyLayerName);
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
        else if(PlayerStat.BossStamina <= 0) // 보스 스태미나 0 이하시 Clear 애니메이션 재생 및 시간 정지
        {
            // 플레이어 크기를 0.3으로 설정
            transform.localScale = Vector3.one * 0.3f;
            transform.position = new Vector2(0, 3f);
            // 5초간 무적 상태 시작
            StartCoroutine(BossClearInvincibleRoutine());
            // Clear 애니메이션 재생
            walkAni("Clear", true, false, false, false);
            StartCoroutine(MonitorClearAnimationUnscaled());
            
            // 플레이어 이동 및 입력 비활성화
            enabled = false;
        }
        else
        {
            //플레이어 입력 이동에 따른
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                sr.flipX = true;
                walkAni("walk", true,true,false,false);
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                sr.flipX = false;
                walkAni("walk", true, true, false, false);
            }
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                walkAni("walkUp", true,false,true,false);
            }
            else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {

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
            float hpPercent = ratio * 100;
            fillImage.fillAmount = ratio;
            if (hpPercent >= 75f) PlayerFace.sprite = PlayerFaces[0]; // 100% ~ 75%
            else if (hpPercent >= 30f) PlayerFace.sprite = PlayerFaces[1]; // 74% ~ 30%
            else PlayerFace.sprite = PlayerFaces[2]; // 29% ~ 0%
        }
        if (PlayerStat.HP <= 0) PlayerDead();
        if (PlayerStat.HP >= 750) PlayerStat.HP = 750;
    }

    public void PlayerDead()
    {
        if (running) return;
        if (StageSetting.InbossStage) BossMapDead();
        else StartCoroutine(DeathRoutine());
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * PlayerStat.PlayerMoveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UpdateHPBar();
        if((collision.CompareTag("Enemy") || collision.CompareTag("Spear") || 
            collision.CompareTag("RunningDog") || collision.CompareTag("Laser") || 
            collision.CompareTag("Virus_BossMap")) && PlayerStat.HP >= 0)
        {
            CapsuleState.CapsuleControl();
            
            // 보스 스태미나가 0 이하일 때는 깜빡임 효과 없이 데미지만 적용
            if (PlayerStat.BossStamina > 0)
            {
                StartCoroutine(InvincibleRoutine());
            }
        }
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
        /*
        if(collision.CompareTag("Vaccine")&&PlayerStat.VaccineLevel==4&&!VaccineSign)
        {
            PlayerStat.AttackPower += 10;
            Debug.Log("AttackPower : " + PlayerStat.AttackPower);
            VaccineSign =!VaccineSign;
        }
        */
    }
    /*
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Vaccine" && PlayerStat.VaccineLevel == 4 && VaccineSign)
        {
            PlayerStat.AttackPower -= 10;
            Debug.Log("AttackPower : " + PlayerStat.AttackPower);
            VaccineSign = !VaccineSign;
        }
    }
    */
    private IEnumerator InvincibleRoutine()
    {
        isInvincible = true;
        if (playerLayer != -1 && enemyLayer != -1)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        float elapsed = 0f;

        while (elapsed < invincibleDuration)
        {
            // 깜빡임 (투명/불투명 반복)
            sr.enabled = !sr.enabled;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        // 깜빡임 끝난 뒤 원래대로
        sr.enabled = true;
        if (playerLayer != -1 && enemyLayer != -1)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        isInvincible = false;
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
            if(aniName == "Clear")
            {
                animator.SetBool("walk", false);
                animator.SetBool("walkUp", false);
                animator.SetBool("walkDown", false);
            }
        }
        else animator.enabled = false; //애니메이터 비활성화 -> hold상태
    }
    IEnumerator DeathRoutine()
    {
        running = true;

        MainCanvas.SetActive(false);
        CapsuleItem.SetActive(false);

        Time.timeScale = 0f;
        if (curve == null) curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        // 시작/목표 스케일
        Vector3 start = player ? player.localScale : Vector3.one * 0.3f;
        Vector3 target = Vector3.one * 1.0f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            float k = curve.Evaluate(Mathf.Clamp01(t));

            // 1) 화면 페이드: fadeOverlay의 알파를 직접 증가
            if (fadeOverlay)
            {
                float a = Mathf.Lerp(0f, 1f, k);

                // CanvasGroup 우선
                var cg = fadeOverlay.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    cg.alpha = a;
                }
                else
                {
                    // Image
                    var img = fadeOverlay.GetComponent<Image>();
                    if (img != null)
                    {
                        var c = img.color;
                        c.a = a;
                        img.color = c;
                    }
                    else
                    {
                        // SpriteRenderer
                        var sr = fadeOverlay.GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            var c = sr.color;
                            c.a = a;
                            sr.color = c;
                        }
                    }
                }
            }

            // 2) 플레이어 스케일 업
            if (player) player.localScale = Vector3.Lerp(start, target, k);

            yield return null;
        }

        yield return null;                   // 한 프레임 넘겨 파괴 적용(선택)

        if (gameOverUI)
        {
            gameOverUI.SetActive(true);
            ResetBtn.SetActive(true);
        }
        EventSystem.current?.SetSelectedGameObject(null);
        // timeScale은 0 유지(게임오버 상태)
    }

    void BossMapDead()
    {
        MainCanvas.SetActive(false);
        BossGameOver.SetActive(true);   
    }

    private IEnumerator MonitorClearAnimation()
    {
        // 3초 대기 후 ClearUI 활성화
        yield return new WaitForSeconds(5f);
        
        // 3초 후 ClearUI 활성화
        if (ClearUI != null)
        {
            ClearUI.SetActive(true);
        }
    }

    private IEnumerator MonitorClearAnimationUnscaled()
    {
        // 5초 대기 후 ClearUI 활성화 (UnscaledTime 사용)
        yield return new WaitForSecondsRealtime(5f);
        
        // 5초 후 ClearUI 활성화
        if (ClearUI != null)
        {
            ClearUI.SetActive(true);
        }
    }

    private IEnumerator BossClearInvincibleRoutine()
    {
        // 기존 무적 상태 설정
        isInvincible = true;
        
        // 레이어 충돌 무시 설정
        if (playerLayer != -1 && enemyLayer != -1)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        
        // 5초간 무적 상태 유지 (깜빡임 효과 없음)
        yield return new WaitForSecondsRealtime(5f);
        
        // 레이어 충돌 복원
        if (playerLayer != -1 && enemyLayer != -1)
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        
        // 무적 상태 해제
        isInvincible = false;
    }

}
