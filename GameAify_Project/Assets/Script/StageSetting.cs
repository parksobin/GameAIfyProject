using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
using System.Collections;

public class StageSetting : MonoBehaviour
{
    public GameObject Player;
    public GameObject Boss; // 보스오브젝트
    public GameObject OriginalStage; //원래 타일맵
    public GameObject BossStage; //보스맵
    public GameObject BossStageDoor; // 보스 스테이지 출입문
    public GameObject BossVideo; //보스 출현 동영상
    public GameObject SwitchMapCanvas; // 영상 준비 완료 전까지 가림용 캔버스
    public GameObject BossHpFill;
    public GameObject BossHpFrame;
    public GameObject Option;// 옵션창 오브젝트

    public GameObject laserPreFab; //2단계 레이저 프리팹
    public GameObject VirusPreFab; //3단계 바이러스 프리팹

    private bool OptionState = false;    //옵션창 닫고 열기 활성화용
    public static bool InbossStage =false; //보스맵 스테이지에 들어간 상태인지 확인
    public bool bossVideoEnd =false; //보스 영상 끝난지 판단 -> 다른 스크립트에서 사용
    public static bool gameplayUnpausedAfterVideo = false; // 보스 영상 7초 경과 후 기능 재개 플래그
    public bool BossLevel2 =false; 
    private float videoTime; //재생시간
    public VideoPlayer bossVideoPlayer; // 보스 영상 플레이어
    private bool videoPrepared = false; // 준비 완료 여부

    private void Awake()
    {
        InbossStage = false;
    }

    void Start()
    {
        Option.SetActive(OptionState);
       Boss.SetActive(false);
        BossStageDoor.SetActive(false);
        BossVideo.SetActive(false);
        BossHpFill.SetActive(false);
        BossHpFrame.SetActive(false);

        // 비디오 플레이어 초기화
        if (bossVideoPlayer != null)
        {
            bossVideoPlayer.playOnAwake = false;
            bossVideoPlayer.skipOnDrop = false;
            bossVideoPlayer.prepareCompleted += OnBossVideoPrepared;
            bossVideoPlayer.loopPointReached += OnBossVideoEnded;
        }
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
        // AudioManager.instance.SFXVolumeControl(false);
        OriginalStage.SetActive(false);
        Boss.transform.position = new Vector3(0,10,0); // 보스 초기위치
        Player.transform.position = new Vector3(0,-7.5f,0); //플레이어 초기위치
        Time.timeScale = 0f;

        // 영상 준비 전까지 캔버스 표시
        if (SwitchMapCanvas != null) SwitchMapCanvas.SetActive(true);
        videoPrepared = false;
        videoTime = 0f;
        gameplayUnpausedAfterVideo = false;
        bossVideoEnd = false;

        // 영상 준비 시작
        if (bossVideoPlayer != null)
        {
            bossVideoPlayer.Stop();
            bossVideoPlayer.Prepare();
            // 이미 준비된 경우 대비
            if (bossVideoPlayer.isPrepared)
            {
                OnBossVideoPrepared(bossVideoPlayer);
            }
        }
        else
        {
            // 비디오 플레이어가 없으면 캔버스를 끄고 바로 진행
            if (SwitchMapCanvas != null) SwitchMapCanvas.SetActive(false);
        }
    }

    private void VideoStartTime()
{
    // 일시정지면 아무것도 하지 않음
    if (Time.timeScale == 0f) return;

    if (BossVideo.active == true)
    {
        bool isPlaying = bossVideoPlayer != null ? bossVideoPlayer.isPlaying : true;

        if (isPlaying && SwitchMapCanvas != null && SwitchMapCanvas.activeSelf)
            SwitchMapCanvas.SetActive(false);

        if (!videoPrepared && bossVideoPlayer != null && bossVideoPlayer.isPrepared)
            OnBossVideoPrepared(bossVideoPlayer);

        if (isPlaying)
        {
            videoTime += Time.unscaledDeltaTime;
            if (videoTime > 7f)
            {
                // 여기 도달해도, 혹시 다른 곳에서 pause 중이면 되돌리지 않도록 한 번 더 보호
                if (Time.timeScale != 0f)
                {
                    Time.timeScale = 1f;
                    BossStage.SetActive(true);
                    BossVideo.SetActive(false);
                    Boss.SetActive(true);
                    BossHpFill.SetActive(true);
                    BossHpFrame.SetActive(true);
                    bossVideoEnd = true;
                    gameplayUnpausedAfterVideo = true;
                }
            }
        }
    }
}

    private void OnBossVideoPrepared(VideoPlayer source)
    {
        videoPrepared = true;
        if (SwitchMapCanvas != null) SwitchMapCanvas.SetActive(false);
        if (bossVideoPlayer != null && !bossVideoPlayer.isPlaying)
        {
            bossVideoPlayer.Play();
        }
    }

    private void OnBossVideoEnded(VideoPlayer source)
    {
        // 비디오가 끝난 경우에도 캔버스는 반드시 꺼진 상태여야 함
        if (SwitchMapCanvas != null && SwitchMapCanvas.activeSelf)
        {
            SwitchMapCanvas.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (bossVideoPlayer != null)
        {
            bossVideoPlayer.prepareCompleted -= OnBossVideoPrepared;
            bossVideoPlayer.loopPointReached -= OnBossVideoEnded;
        }
    }

    public void OptionBtn()
    {
        OptionState = !OptionState;
        Option.SetActive(OptionState);

    }
}
