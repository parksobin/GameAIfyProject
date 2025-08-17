using UnityEngine;

public class StageSetting : MonoBehaviour
{
    public GameObject Player;
    public GameObject Boss; // 보스오브젝트
    public GameObject OriginalStage; //원래 타일맵
    public GameObject BossStage; //보스맵
    public GameObject BossStageDoor; // 보스 스테이지 출입문
    public bool InbossStage =false; //보스맵 스테이지에 들어간 상태인지 확인
    
    void Start()
    {
        Boss.SetActive(false);
        BossStageDoor.SetActive(false);
    }

    void Update()
    {
        purificationClear();
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

    public void InBossStage()
    {
        InbossStage = true;
        BossStageDoor.SetActive(false );
        BossStage.SetActive(true );
        OriginalStage.SetActive(false);
        Boss.transform.position = new Vector3(0,0,0);
        Player.transform.position=new Vector3(0,-4,0);
    }
}
