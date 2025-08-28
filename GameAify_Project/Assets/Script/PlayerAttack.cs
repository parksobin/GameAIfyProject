using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Timeline;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    // 주사기 관련 멤버 변수
    public GameObject SyringePrefab; // 주사기 프리팹
    private float SyringeSpeed = 20.0f; // 주사기 속도
    public static float SyringeTimer = 0f; // 발사 시간 초기화
    private float spawnDistance; // 주사기 생성 위치 조절
    private float SyringeSpacing = 1f;    // 가로 간격
    private float SyringeRowGap = 0.3f;     // 두 줄 세로 간격(위/아래)
    private float SyringSpawnTick = 0.15f;     // 간격(요구사항)

    // 메스 관련 멤버 변수
    public GameObject[] MessPrefab; // 메스 프리팹
    public GameObject[] MessBulletPrefab; // 메스 발사체 프리팹
    //public GameObject UniqueBulletPrefab; // 유니티 발사체 프리팹
    private SpriteRenderer MessRend; // 메스 스프라이트 참조용
    private float rotationDuration = 0.25f; // 0도에서 최대각도까지 도는 데 걸리는 시간
    private float MessTimer = 0f;
    private bool MessRotating = false; // 메스가 생성 중인지 여부
    private float MessBulletSpeed = 15f; // 메스 발사체 속도
    private float ShootDelay = 5f;
    private int radian; // 메스 발사체 각도 설정
    private int totalRadian; // 메스 발사체 최종 각도

    //백신 관련 변수
    public GameObject VaccinePrefab;  //백신 투하 영역 프리팹 오브젝트
    private int VaccineMaxCount; //백신 단계당 초당 생성 개수
    public static float VaccineWaitSeconds = 8f; // 기본 백신 생성주기 초수
    private float VaccineTimer = 0f;

    //캡슐 관련 변슈
    private bool GetCapsule =false; //캡슐 획득 판정 -> tre시 재생성 쿨타임 시작
    public GameObject Capsule; // 생성되는 캡슐 프리팹
    public GameObject capsuleObj; //캡슐 오브젝트(플레이어 내에있음)
    public static float CapsuleTime; // 캡슐 쿨타임
    private float capsuleTimer;  //캡슐 쿨타임 계산을 위한 변수

    //모든 무기유니크 단계 이미지 
    //public Sprite[] UniqueImg;  //  (0 매스, 1 매스 총알 )  / 백신은 따로 프리팹 안에 되어 잇음
    public static int NowCount = 0; // 아이템 업그레이드 가능 횟수

    void Start()
    {
        //StartCoroutine(VaccineInject());
        capsuleObj = GameObject.Find("CapsuleiTem");
        capsuleObj = null;
    }

    void Update()
    {
        // 정화 게이지 100 이상 시 보스 연출(영상) 종료 전까지(또는 보스 체력이 0이 되면) 모든 공격/생성 기능 일시 중지
        if ((PlayerStat.purificationGauge >= 100 && 
        !StageSetting.gameplayUnpausedAfterVideo) || PlayerStat.BossStamina <= 0)
        {
            return;
        }

        CapsuleActiveOn();
        AttackSyringeAndMess();
        CheckDamage();
    }

    void AttackSyringeAndMess()
    {
        TimerCount();
        if (SyringeTimer >= PlayerStat.AttackSpeed)
        {
            StartCoroutine(ShootSyringe());
            SyringeTimer = 0f;
        }
        if (MessTimer >= PlayerStat.AttackSpeed && !MessRotating)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float direction = mouseWorld.x < transform.position.x ? 1f : -1f;
            StartCoroutine(SpawnMessRotate(direction));
            MessTimer = 0f;
        }
        if (VaccineTimer >= VaccineWaitSeconds)
        {
            MakeVaccine();
            VaccineTimer = 0f;
        }
    }
    void TimerCount()
    {
        SyringeTimer += Time.deltaTime;
        MessTimer += Time.deltaTime;
        VaccineTimer += Time.deltaTime;
    }
    void CheckDamage()
    {
        if (PlayerStat.SyringeLevel == 4) PlayerStat.SyringePower = PlayerStat.AttackPower / 2.0f;
        else PlayerStat.SyringePower = PlayerStat.AttackPower / 4.0f;
        if (PlayerStat.VaccineLevel == 4) PlayerStat.VaccinePower = PlayerStat.AttackPower / 5.0f;
        else PlayerStat.VaccinePower = PlayerStat.AttackPower / 10.0f;
        PlayerStat.MessPower = PlayerStat.AttackPower / 2.0f;
    }

    // 주사기 발사 함수
    IEnumerator ShootSyringe()
    {
        int countPerRow;
        int rows;

        switch (PlayerStat.SyringeLevel)
        {
            case 1: 
                countPerRow = 1; rows = 1; spawnDistance = 1.25f; break;
            case 2: 
                countPerRow = 3; rows = 1; spawnDistance = 2.0f; break; // 한 발씩 0.2초 간격
            case 3: 
                countPerRow = 5; rows = 1; spawnDistance = 3.0f; break; // 한 발씩 0.2초 간격
            default: 
                countPerRow = 5; rows = 2; spawnDistance = 3.0f; break; // 두 줄(위/아래)을 한 세트로 0.2초 간격
        }
        
        // === 1단계: 즉시 1발 ===
        if (rows == 1 && countPerRow == 1)
        {
            SpawnOne(0f, 0f);
            
            yield break;
        }
        // === 2·3단계: 한 줄에서 "한 발씩" 0.2s 간격 ===
        if (rows == 1 && countPerRow > 1)
        {
            for (int i = 0; i < countPerRow; i++)
            {
                float xOffset = (-(countPerRow - 1) / 2f) * SyringeSpacing;
                SpawnOne(xOffset, 0f);             // 매번 SpawnOne 내부에서 마우스 방향 재계산
                if (i < countPerRow - 1) yield return new WaitForSeconds(SyringSpawnTick);
            }
            yield break;
        }
        // === 4단계: 두 줄을 '한 세트'로 컬럼(열) 단위 0.2s 간격 ===
        // i열마다 위/아래 2발 동시 생성 → 0.2s → 다음 열 …
        if (rows == 2)
        {
            for (int i = 0; i < countPerRow; i++)
            {
                float xOffset = (-(countPerRow - 1) / 2f) * SyringeSpacing;
                // 한 세트(위/아래) 동시 생성
                SpawnOne(xOffset, +SyringeRowGap);
                SpawnOne(xOffset, -SyringeRowGap);

                if (i < countPerRow - 1) yield return new WaitForSeconds(SyringSpawnTick);
            }
        }
        // ---- 지역 함수: 매 호출 시점의 마우스 방향으로 1발 생성 ----
        void SpawnOne(float xOffset, float yOffset)
        {
            Vector3 originPos = transform.position;

            // 마우스 방향을 "지금 시점"에 다시 계산
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            Vector3 direction = (mousePos - originPos).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // 플레이어 앞쪽으로 약간 띄워서 생성
            Vector3 spawnOrigin = originPos + direction * spawnDistance;
            // 오프셋을 발사 각도에 맞춰 회전
            Vector3 offset = Quaternion.Euler(0, 0, angle) * new Vector3(xOffset, yOffset, 0f);
            Vector3 spawnPos = spawnOrigin + offset;
            // 생성 및 초기 설정
            GameObject proj = Instantiate(SyringePrefab, spawnPos, Quaternion.Euler(0, 0, angle + 90f));
            AudioManager.instance.SyringeSound.Play();
            var rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = direction * SyringeSpeed;
            var AR = proj.GetComponent<AttackRange>();
            if (AR != null) AR.SetStartPosition(originPos);
        }
    }

    // 메스 휘두르기 함수
    private IEnumerator SpawnMessRotate(float direction)
    {
        float elapsed = 0f;
        float startAngle = 0f;
        float endAngle;
        bool isBulletShoot = false;
        switch (PlayerStat.MessLevel)
        { 
            case 2: 
                endAngle = 180f * direction; isBulletShoot = true; break;
            case 3: 
                endAngle = 180f * direction; isBulletShoot = true; radian = 90; totalRadian = 270; break;
            case 4: 
                endAngle = 180f * direction; isBulletShoot = true; radian = 45; totalRadian = 315; break;
            default:
                endAngle = 180f * direction; break;
        }
        MessRotating = true;
        GameObject obj = Instantiate(MessPrefab[ChangeUniqueImg(PlayerStat.MessLevel)], transform.position, Quaternion.identity);
        Transform MessTarget = obj.transform.Find("Square");
        MessRend = MessTarget.GetComponent<SpriteRenderer>();
        if (direction > 0f) MessRend.flipX = true;
        else MessRend.flipX = false;
        AudioManager.instance.MessSound.Play();
              
        while (elapsed < rotationDuration)
        {
            // 1. 플레이어 위치로 이동 (이 스크립트가 플레이어에 붙어있다고 가정)
            obj.transform.position = transform.position;

            // 2. Z축 회전 적용
            float t = elapsed / rotationDuration;
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            obj.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // 발사 조건: 각도 지나침 체크
            if (isBulletShoot && Mathf.Abs(angle) >= 90f)
            {
                if (PlayerStat.MessLevel == 2) MessBulletShoot();
                else if (PlayerStat.MessLevel >= 3) Explosion();
                isBulletShoot = false;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        // 마지막 각도 보정
        obj.transform.rotation = Quaternion.Euler(0f, 0f, endAngle);
        Destroy(obj);
        yield return new WaitForSeconds(PlayerStat.AttackSpeed);
        MessRotating = false;
    }
    // 메스 총알 발사 함수1
    void MessBulletShoot()
    { 
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 fireDir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(fireDir.y, fireDir.x) * Mathf.Rad2Deg;

        GameObject proj = Instantiate(MessBulletPrefab[0], transform.position, Quaternion.Euler(0, 0, angle));
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

        if (rb != null) rb.linearVelocity = fireDir * MessBulletSpeed;
        AttackRange AR = proj.GetComponent<AttackRange>();
        if (AR != null) AR.SetStartPosition(transform.position);
    }

    // 메스 총알 발사 함수2
    void Explosion()
    {
        for (int i = 0; i <= totalRadian; i += radian)
        {
            float rad = i * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            GameObject proj = Instantiate(MessBulletPrefab[1], transform.position, Quaternion.Euler(0, 0, i));
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = direction * MessBulletSpeed;
            AttackRange AR = proj.GetComponent<AttackRange>();
            if (AR != null) AR.SetStartPosition(transform.position);
        }
    }

    // 백신 생성 함수
    private void MakeVaccine()
    {
        // VaccineWaitSeconds = 8f;
        // yield return new WaitForSeconds(VaccineWaitSeconds); // 각 백신마다 간격
        int vaccineCount = 0; //백신 단계당 생성된 개수
        switch (PlayerStat.VaccineLevel)
        {
            case 1: 
                VaccineWaitSeconds = 8f; VaccineMaxCount = 1; break;// 1단계          
            case 2: 
                VaccineWaitSeconds = 8f; VaccineMaxCount = 3; break;// 2단계
            case 3: 
                VaccineWaitSeconds = 5f; VaccineMaxCount = 3;  break;// 3단계         
            case 4: 
                VaccineWaitSeconds = 5f; VaccineMaxCount = 3; break;// 유니크 단계
            default: break;
        }
        for (int i = 0; i < VaccineMaxCount; i++)
        {
            VaccineCoordinate();
        }
    }
    private void VaccineCoordinate()  // 백신 위치 함수
    {
        float playerX = transform.position.x;
        float playerY = transform.position.y;
        // X축: -5 ~ +5 사이 랜덤
        float randomX = playerX + Random.Range(-5f, 5f);
        // Y축: +10 고정
        float randomY = playerY + 10f;
        Instantiate(VaccinePrefab, new Vector3(randomX, randomY, -0.5f), Quaternion.identity);
    }

    //캡슐 활성화 함수
    private void CapsuleActiveOn()
    {
        if (capsuleObj == null) capsuleObj = GameObject.Find("CapsuleiTem"); // 이름 확인 필요: "CapsuleItem"이면 오타

        // 캡슐이 '꺼져 있을 때'만 쿨타임 증가
        if (!CapsuleState.CapsuleActive)
            capsuleTimer += Time.deltaTime;

        // 레벨별 재생성 대기시간 계산
        switch (PlayerStat.CapsuleLevel)
        {
            case 1:
            case 2:
            case 3:
                CapsuleTime = 20f - (20f * HitSpeedPercent());
                break;
            case 4:
                CapsuleTime = 15.0f - (15.0f * HitSpeedPercent());
                break;
            default:
                CapsuleTime = 20.0f;
                break;
        }

        // 모든 레벨에서 재생성 체크 호출
        CapsuleTimerOn(CapsuleTime);
    }

    private float HitSpeedPercent() //공격속도단계에 따른 쿨타임 감소
    {
        switch (PlayerStat.AttSpeedLevel)
        {
            case 1:
                return 0.1f;
            case 2:
                return 0.2f;
            case 3:
                return 0.3f;
            case 4:
                return 0.4f;
            case 5:
                return 0.5f;
            default:
                return 0;
        }

    }
    private void CapsuleTimerOn(float sec)
    {
        if (!CapsuleState.CapsuleActive && capsuleObj != null && capsuleTimer >= sec)
        {
            // 상태 일치시키기: 재생성 = 둘 다 true
            PlayerStat.CapsuleState = true;
            CapsuleState.CapsuleActive = true;

            var cs = capsuleObj.GetComponent<CapsuleState>();
            if (cs != null) CapsuleState.ActiveDesignerEventArgs(1.0f);

            GetCapsule = false;
            capsuleTimer = 0f;
        }
    }


    static int ChangeUniqueImg(int level)
    {
       int value = 0;
       if (level == 4) value = 1;
       return value;
    }
}
