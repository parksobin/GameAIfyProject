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
    private float DelayTime=0f;
    public bool HitSign =true;
    int count = 0;
    private bool lv3PatternRunning = false;
    private float Lv3DelayTime = 10.0f;

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

    private void BossHit()
    {
        
        switch (BossLevel)
        {
            case 1:
                Level1_Hit();
                break;
            case 2:
                break;
            case 3:
                Level3_Hit();
                break;
            case 4:
                break;
            default:
                break;
        }
        
    }

    private void Level1_Hit()
    {
        DelayTime += Time.deltaTime;
        if(DelayTime >=5f&& HitSign)
        {
            animator.SetBool("Level1",true);
            spawner.SponLevel1_Laser();
            HitSign = !HitSign;
        }
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
