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

    private float Lv3DelayTime = 10f;

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
        // 한번에 5번
        // 쿨타임 10초
        Lv3DelayTime += Time.deltaTime;
        if(Lv3DelayTime > 10f)
        {
            count++;
            StartCoroutine(Wait1s());
            virusSet.SpawnCenterBoss3_Hit();
            if(count>=5)
            {
                count = 0;
                Lv3DelayTime = 0;
            }
        }
    }

    public static IEnumerator Wait1s()
    {
        yield return new WaitForSeconds(1f);
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
