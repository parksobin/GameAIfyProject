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

    public bool InbossStage =false; //보스맵 스테이지에 들어간 상태인지 확인
    public bool BossLevel2 =false; //보스맵 스테이지에 들어간 상태인지 확인
    private float videoTime; //재생시간
    private float RotateSpeed = 30f; // 보스 2차 회전 속도
    Vector3 bosspos;

    private GameObject laser1;
    private GameObject laser2;
    private GameObject[] virus;

    void Start()
    {
        Boss.SetActive(false);
        BossStageDoor.SetActive(false);
        BossVideo.SetActive(false);
        bosspos = Boss.transform.position;
    }

    void Update()
    {
        purificationClear();
        VideoStartTime();
        BossLevel2_Rotate();
    }

    private void purificationClear() //정화게이지 100달성 후 보스 문 표시
    {
        if(PlayerStat.purificationGauge ==100 && !PlayerStat.purificationClearposSign)
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
        BossStageDoor.SetActive(false );
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
            }
        }
    }

    private void StartBossAnimation() //보스맵 도입 보스 애니메이션
    {

    }


    private void BossLevel2_Rotate() //보스 레벨 2 시계방향 레이저 회전
    {
        //보스 좌표 기준 추가 해야함
        if (BossLevel2 == false && Input.GetKeyDown(KeyCode.K))
        {
            laser1 = Instantiate(laserPreFab, Vector3.zero, Quaternion.identity);
            // (0,0,0) 위치, 회전 없음
            laser2 = Instantiate(laserPreFab, Vector3.zero, Quaternion.Euler(0, 0, 90));
            // (0,0,0) 위치, Z축으로 90도 회전
            BossLevel2 = true;
        }
        else if( BossLevel2)
        {
            //레이저 두 개 생성 후 시계방향 회전 초당 30도
            laser1.transform.Rotate(0, 0, RotateSpeed * Time.deltaTime);
            laser2.transform.Rotate(0, 0, RotateSpeed * Time.deltaTime);
        }

    }
}
