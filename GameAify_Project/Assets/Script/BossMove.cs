using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossMove : MonoBehaviour
{
    private Animator animator;
    public StageSetting StageSetting;
    private LaserSpawner spawner;
    private VirusSet virusSet;
    private int BossLevel = 1;
    public float DelayTime=0f;
    public bool HitSign =true;
    int count = 0;
    private bool lv3PatternRunning = false;
    private float Lv3DelayTime = 10.0f;
    // 추가: Lv1 코루틴 중복 방지용
    private bool lv1PatternRunning = false;
    void Start()
    {
        StageSetting = GameObject.Find("MainManager").GetComponent<StageSetting>();
        spawner = GetComponent<LaserSpawner>();
        animator = GetComponent<Animator>();
        virusSet = GetComponent<VirusSet>();
    }

    void Update()
    {
        BossLevelChange();
        BossHit();
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
                Level2_Hit();
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
        animator.SetBool("Level1", true);
        spawner.SponLevel1_Laser();

        // 공격 연출 포함 대기 3초
        yield return new WaitForSeconds(5f);

        // 공격 종료: 애니 끔
        animator.SetBool("Level1", false);

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
        // 쿨타임 축적
        if (Lv3DelayTime < 10f)
            Lv3DelayTime += Time.deltaTime;

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
        else if (PlayerStat.BossStamina > 5000) BossLevel = 2;
        else if (PlayerStat.BossStamina > 2500) BossLevel = 3;
        else BossLevel = 4;
    }


}
