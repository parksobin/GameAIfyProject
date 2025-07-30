using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    public CollisionHandler collisionHandler;
    // 주사기 관련 멤버 변수
    public GameObject SyringePrefab; // 주사기 프리팹
    private float SyringeSpeed = 10f; // 주사기 속도
    private float timer = 0f; // 발사 시간 초기화

    // 메스 관련 멤버 변수
    public GameObject MessPrefab; // 메스 프리팹
    public GameObject MessBulletPrefab; // 메스 발사체 프리팹
    private SpriteRenderer MessRend; // 메스 스프라이트 참조용
    private float rotationDuration = 0.25f; // 0도에서 최대각도까지 도는 데 걸리는 시간
    private bool MessRotating = false; // 메스가 생성 중인지 여부
    private float MessBulletSpeed = 15f; // 메스 발사체 속도

    //백신 관련 변수
    public GameObject vaccine;  //백신 투하 영역 프리팹 오브젝트
    private int vaccineMaxCount; //백신 단계당 초당 생성 개수
    private float vaccineWaitSeconds; //백신 생성주기 초수

    //캡슐 관련 변슈
    private GameObject capsuleObj; //캡슐 오브젝트(플레이어 내에있음)
    private float capsuleTimer;  //캡슐 쿨타임
    private CapsuleState capsuleState;

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
        if (timer >= PlayerStat.AttackSpeed)
        {
            StartCoroutine(ShootSyringe());
            timer = 0f;
        }
        if (Input.GetKeyDown(KeyCode.Space) && !MessRotating && timer >= PlayerStat.AttackSpeed)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float direction = mouseWorld.x < transform.position.x ? 1f : -1f;
            StartCoroutine(SpawnMessRotate(direction));
        }
    }

    IEnumerator ShootSyringe()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        int countPerRow = 1;
        int rows = 1;

        switch (PlayerStat.SyringeLevel)
        {
            case 1:
                countPerRow = 1;
                rows = 1;
                break;
            case 2:
                countPerRow = 3;
                rows = 1;
                break;
            case 3:
                countPerRow = 5;
                rows = 1;
                break;
            default:
                countPerRow = 5;
                rows = 2;
                break;
        }
        float spacing = 1.5f; // 주사기 간의 위치 간격
        for (int r = 0; r < rows; r++)
        {
            for (int i = 0; i < countPerRow; i++)
            {
                // 좌우로 간격을 둔 위치 계산
                float xOffset = (-(countPerRow - 1) / 2f + i) * spacing;
                float yOffset = (rows == 2) ? (r == 0 ? 0.3f : -0.3f) : 0f;

                // 발사 방향에 맞춰 오프셋 회전
                Vector3 offset = Quaternion.Euler(0, 0, angle) * new Vector3(xOffset, yOffset, 0);
                Vector3 spawnPos = transform.position + offset;

                GameObject proj = Instantiate(SyringePrefab, spawnPos, Quaternion.Euler(0, 0, angle + 90f));
                Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = direction * SyringeSpeed;
                AttackRange AR = proj.GetComponent<AttackRange>();
                if (AR != null) AR.SetStartPosition(transform.position);

            }
        }
        yield return null; // 또는 생략
    }

    private IEnumerator SpawnMessRotate(float direction)
    {
        MessRotating = true;
        GameObject obj = Instantiate(MessPrefab, transform.position, Quaternion.identity);
        Transform MessTarget = obj.transform.Find("Square");
        MessRend = MessTarget.GetComponent<SpriteRenderer>();
        if (direction > 0f) MessRend.flipX = true;
        else MessRend.flipX = false;
        float elapsed = 0f;
        float startAngle = 0f;
        float endAngle = 0f;
        bool isBulletShoot = false;
        switch (PlayerStat.MessLevel)
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
                MessBulletShoot();
                isBulletShoot = false;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
        // 마지막 각도 보정
        obj.transform.rotation = Quaternion.Euler(0f, 0f, endAngle);
        Destroy(obj);
        MessRotating = false;
    }
    void MessBulletShoot()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 fireDir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(fireDir.y, fireDir.x) * Mathf.Rad2Deg;
        
        GameObject proj = Instantiate(MessBulletPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

        if (rb != null) rb.linearVelocity = fireDir * MessBulletSpeed;
        AttackRange AR = proj.GetComponent<AttackRange>();
        if (AR != null) AR.SetStartPosition(transform.position);
    }
    // 백신 생성 함수
    IEnumerator VaccineInject()
    {
        vaccineWaitSeconds = 8f;
       // yield return new WaitForSeconds(vaccineWaitSeconds); // 각 백신마다 간격
        int vaccineCount = 0 ; //백신 단계당 생성된 개수
        while (true)
        {
            switch (PlayerStat.VaccineLevel)
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

            switch (PlayerStat.CapsuleLevel)
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
