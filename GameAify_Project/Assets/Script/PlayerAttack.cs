using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    
    public CollisionHandler collisionHandler;
    // 주사기 관련 멤버 변수
    public GameObject SyringePrefab; // 주사기 프리팹
    private float SyringeSpeed = 10f; // 주사기 속도
    private float shootInterval = 1.5f; // 주사기 발사 간격
    private float SyringeLifetime = 1f; // 주사기 수명
    private float timer = 0f; // 발사 시간 초기화
    private int SyringeMemberCount = 1; // 주사기 갯수
    private float SyringeInterval = 0.15f; // 주사기 간 발사 간격

    // 메스 관련 멤버 변수
    public GameObject ScalpelPrefab; // 메스 프리팹
    public GameObject ScalpelBulletPrefab; // 메스 발사체 프리팹
    private float rotationDuration = 0.5f; // 0도에서 최대각도까지 도는 데 걸리는 시간
    private bool ScalpelRotating = false; // 메스가 생성 중인지 여부
    private float ScalpelBulletSpeed = 10f; // 메스 발사체 속도

    public int SyringeStep = 1; // 주사기 단계
    public int ScalpelStep = 1; // 메스 단계

    // 게임 시스템 관련 멤버 변수
   // public TextMeshProUGUI ScoreText;
    public static int Score = 0;
    
    


    //백신 관련 변수
    public GameObject vaccine;  //백신 투하 영역 프리팹 오브젝트
    public int  vaccineLevel=1;  //백신 투하 단계
    private int vaccineMaxCount; //백신 단계당 초당 생성 개수
    private float vaccineWaitSeconds; //백신 생성주기 초수


    //캡슐 관련 변슈
    private GameObject capsuleObj; //캡슐 오브젝트(플레이어 내에있음)
    private float capsuleTimer;  //캡슐 쿨타임
    private CapsuleState capsuleState;
    public int capsuleLevel = 1;


    void Start()
    {
        //StartCoroutine(VaccineInject());
        capsuleObj = GameObject.Find("CapsuleiTem");
        capsuleState = capsuleObj.GetComponent<CapsuleState>();
    }

    void Update()
    {
        MakeVaccine();
        CapsuleActiveOn();
        timer += Time.deltaTime;
        if (timer >= shootInterval)
        {
            StartCoroutine(ShootSyringe());
            timer = 0f;
        }
        if (Input.GetKeyDown(KeyCode.Space) && !ScalpelRotating)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float direction = mouseWorld.x < transform.position.x ? 1f : -1f;
            StartCoroutine(SpawnScalpelRotate(direction));
        }
    }

    IEnumerator ShootSyringe() // 주사기 발사 함수
    {
        switch(SyringeStep)
        {
            case 1:
                SyringeMemberCount = 1;
                break;
            case 2:
                SyringeMemberCount = 3;
                break;
            case 3:
                SyringeMemberCount = 5;
                break;
        }
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 direction = (mousePos - transform.position).normalized; // 방향 계산
        for(int i = 0; i < SyringeMemberCount; i++)
        {
            GameObject proj = Instantiate(SyringePrefab, transform.position, Quaternion.identity); // 발사체 생성
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            proj.transform.rotation = Quaternion.Euler(0, 0, angle + 90f); // 마우스 커서에 맞춰서 프리팹 각도 변경
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = direction * SyringeSpeed;
            yield return new WaitForSeconds(SyringeInterval);
            Destroy(proj, SyringeLifetime); // 일정 시간 뒤 발사체 제거
        }
    }
    private IEnumerator SpawnScalpelRotate(float direction)
    {
        ScalpelRotating = true;
        GameObject obj = Instantiate(ScalpelPrefab, transform.position, Quaternion.identity);
        float elapsed = 0f;
        float startAngle = 0f;
        float endAngle = 0f;
        bool isBulletShoot = false;
        switch (ScalpelStep)
        {
            case 1:
                endAngle = 90f * direction;
                break;
            case 2:
                endAngle = 180f * direction;
                break;
            case 3:
                endAngle = 180f * direction;
                isBulletShoot = true;
                break;
        }
        
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
                ScalpelBulletShoot(direction);
                isBulletShoot = false;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
        // 마지막 각도 보정
        obj.transform.rotation = Quaternion.Euler(0f, 0f, endAngle);
        Destroy(obj);
        ScalpelRotating = false;
    }
    void ScalpelBulletShoot(float direction)
    {
        GameObject proj = Instantiate(ScalpelBulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 fireDir = direction > 0 ? Vector2.left : Vector2.right;
            rb.linearVelocity = fireDir * ScalpelBulletSpeed;
        }
    }
    // 백신 생성 함수
    IEnumerator VaccineInject()
    {
        vaccineWaitSeconds = 8f;
       // yield return new WaitForSeconds(vaccineWaitSeconds); // 각 백신마다 간격
        int vaccineCount = 0 ; //백신 단계당 생성된 개수
        while (true)
        {
            switch (vaccineLevel)
            {
                case 1: //1단계
                    vaccineWaitSeconds = 8f;
                    vaccineMaxCount = 1;
                    break;
                case 2: //2단계
                    vaccineWaitSeconds = 8f;
                    vaccineMaxCount = 3;
                    break;
                case 3: //3단계
                    vaccineWaitSeconds = 5f;
                    vaccineMaxCount = 3;
                    break;
                case 4: //유니크 단계
                    vaccineWaitSeconds = 5f;
                    vaccineMaxCount = 3;
                    break;
                default:
                    yield break;
            }
            for (int i = 0; i < vaccineMaxCount; i++)
            {
                VaccineCoordinate();
            }
            // yield return new WaitForSeconds(vaccineWaitSeconds); // 각 백신마다 간격
        }
    }
    
    private void MakeVaccine()
    {
        if(Input.GetKeyDown(KeyCode.F))
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

        Instantiate(vaccine, new Vector3(randomX, randomY, -0.5f), Quaternion.identity);
    }


    
    //캡슐 활성화 함수
    private void CapsuleActiveOn()
    {
        if (!capsuleState.CapsuleActive)
        {
            capsuleTimer += Time.deltaTime;

            switch (capsuleLevel)
            {
                case 1:
                    //  타격 감소 관련 작성 예정
                    CapsuleTimerOn(20);
                    break;
                case 2:
                    CapsuleTimerOn(20);
                    break;
                case 3:
                    CapsuleTimerOn(20);
                    break;
                case 4:
                    CapsuleTimerOn(15);
                    break;
            }
        }
    }
    
    private void CapsuleTimerOn(float sec) //캡슐 재생성 쿨타임
    {
        if (capsuleTimer > sec)
        {
            capsuleTimer = 0;
            capsuleObj.SetActive(true);
        }
    }
}
