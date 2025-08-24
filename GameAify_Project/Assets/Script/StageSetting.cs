using UnityEngine;
using UnityEngine.UIElements;

public class StageSetting : MonoBehaviour
{
    public GameObject Player;
    public GameObject Boss; // 보스오브젝트
    public GameObject OriginalStage; //원래 타일맵
    public GameObject BossStage; //보스맵
    public GameObject BossStageDoor; // 보스 스테이지 출입문
    public GameObject BossVideo;

    public GameObject laserPreFab; //2단계 레이저 프리팹
    public GameObject VirusPreFab; //3단계 바이러스 프리팹

    public static bool InbossStage =false; //보스맵 스테이지에 들어간 상태인지 확인
    public bool bossVideoEnd =false; //보스 영상 끝난지 판단 -> 다른 스크립트에서 사용
    public bool BossLevel2 =false; 
    private float videoTime; //재생시간


    void Start()
    {
       Boss.SetActive(false);
        BossStageDoor.SetActive(false);
        BossVideo.SetActive(false);
    }

    void Update()
    {
        purificationClear();
        VideoStartTime();
        //BossLevel2_Rotate();
        if (PlayerStat.currentGauge >= 5000) PlayerStat.purificationGauge = 100;
    }

    private void purificationClear() //정화게이지 100달성 후 보스 문 표시
    {
        if (PlayerStat.purificationGauge >= 100 && !PlayerStat.purificationClearposSign)
        {
            Player = GameObject.Find("Player");
            Vector3 PYpos = Player.transform.position;
            BossStageDoor.transform.position = new Vector3(PYpos.x,PYpos.y+4,PYpos.z);
            BossStageDoor.SetActive(true);
            PlayerStat.purificationClearposSign = !PlayerStat.purificationClearposSign;
        }
    }

    public void InBossStage() //보스맵 들어가면 가장 기본 설정
    {
        BossVideo.SetActive(true) ;
        InbossStage = true;
        BossStageDoor.SetActive(false);
        BossStage.SetActive(true );
        OriginalStage.SetActive(false);
        Boss.transform.position = new Vector3(0,10,0); // 보스 초기위치
        Player.transform.position=new Vector3(0,-7.5f,0); //플레이어 초기위치
    }

    private void VideoStartTime() //영상길기 (7초) 뒤에 꺼지도록 설정
    {
        if (BossVideo.active==true)
        {
            videoTime += Time.deltaTime;
            if (videoTime > 7)
            {
                BossVideo.SetActive(false);
                Boss.SetActive(true);
                bossVideoEnd=true;
            }
        }
    }

}
