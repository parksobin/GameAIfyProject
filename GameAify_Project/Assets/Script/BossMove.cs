using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossMove : MonoBehaviour
{
    private Animator animator;
    public StageSetting StageSetting;
    private LaserSpawner spawner;
    private VirusSet virusSet;
    private SpriteRenderer sr;
    public  Sprite[] BossImg; // 0: boss idle이미지 / 1 :Die 이미지

    private int BossLevel = 1;
    public float DelayTime = 0f;
    public bool HitSign =true;
    private bool lv3PatternRunning = false;
    private float Lv3DelayTime = 10.0f;


    // 추가: Lv1 코루틴 중복 방지용
    private bool lv1PatternRunning = false;
    private bool OnVaccineDamage = false; //백신 공격 여부
    void Start()
    {
        StageSetting = GameObject.Find("MainManager").GetComponent<StageSetting>();
        spawner = GetComponent<LaserSpawner>();
        animator = gameObject.GetComponent<Animator>();
        virusSet = GetComponent<VirusSet>();
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        BossImgChange();
        if (OnVaccineDamage)
        {
            PlayerStat.BossStamina -= PlayerStat.VaccinePower * Time.deltaTime;
        }
    }

    private void BossImgChange()
    { 
        //보스 체력에 따른 애니메이터 사용여부와 기본 이미지 변경 && 체력이 있어야만 공격 함
        if(PlayerStat.BossStamina >0)
        {
            sr.sprite = BossImg[0];
            animator.enabled = true;
            BossLevelChange();
            BossHit();
        }
        else
        {
            sr.sprite = BossImg[1];
            animator.enabled = false;
        }
    }

    private void BossHit() //레벨별 보스 공격 실행
    {
        
        switch (BossLevel)
        {
            case 1:
                Level1_Hit();
                break;
            case 2:
                Level1_Hit();
                Level2_Hit();
                break;
            case 3:
                Level1_Hit();
                Level3_Hit();
                break;
            case 4:
                Level1_Hit();
                Level2_Hit();
                Level3_Hit();
                break;
            default:
                break;
        }
        
    }

    //  Level1 공격 패턴
    private void Level1_Hit()
    {
        // 이미 공격 패턴 수행 중이면 대기
        if (lv1PatternRunning) return;

        // 쿨타임 5초 누적
        DelayTime += Time.deltaTime;

        // 쿨타임 끝나면 공격 패턴 실행
        if (DelayTime >= 5f)
            StartCoroutine(Level1Pattern());
    }

    private IEnumerator Level1Pattern()
    {
        lv1PatternRunning = true;
        // 공격 시작: 애니 켜고 레이저 발사
        spawner.SponLevel1_Laser();
        animator.SetTrigger("Attack1");

        // 공격 연출 포함 대기 3초
        yield return new WaitForSeconds(3.3f);

        // 공격 종료: 애니 끔
        //animator.SetBool("Level1", false);
        animator.SetTrigger("Ide");

        // 쿨타임 타이머 리셋
        DelayTime = 0f;
        lv1PatternRunning = false;
    }

    private void Level2_Hit()
    {
        spawner.BossLevel2_Rotate();
    }

    private void Level3_Hit()
    {
        animator.SetTrigger("Attack2");
        // 쿨타임 축적
        if (Lv3DelayTime < 10f)
            Lv3DelayTime += Time.deltaTime;
        // 패턴1 실행중이면 기다림 -> 패턴1 실행 후 실행
        if (lv1PatternRunning) return;
        // 쿨타임 끝났고 아직 패턴 실행 중이 아니면 시작
        if (!lv3PatternRunning && Lv3DelayTime >= 10f)
            StartCoroutine(Level3Pattern());
    }

    private IEnumerator Level3Pattern()
    {
        lv3PatternRunning = true;
        Lv3DelayTime = 0f; // 쿨타임 리셋

        // 1초 간격으로 5번
        for (int i = 0; i < 5; i++)
        {
            virusSet.SpawnCenterBoss3_Hit();   // 실행
            if (i < 4)                          // 마지막에는 대기 없음
                yield return new WaitForSeconds(1f);
        }
        lv3PatternRunning = false; // 다음 쿨타임 후 다시 가능
    }

    public void DelayTimeReset() 
    {
        animator.SetBool("Level1", false);
        DelayTime = 0f;
    }

    private void BossLevelChange()
    {
        if (PlayerStat.BossStamina > 7500) BossLevel = 1;
        else if (PlayerStat.BossStamina > 5000)
        {
            BossLevel = 2;
            spawner.RotateSpeed = 30f;
        }
        else if (PlayerStat.BossStamina > 2500) BossLevel = 3;
        else
        { BossLevel = 4; spawner.RotateSpeed = 36f; }  //4페이즈 회전 속도 올라감
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.StartsWith("VaccineFeild")) OnVaccineDamage = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name.StartsWith("VaccineFeild")) OnVaccineDamage = false;
    }

}
