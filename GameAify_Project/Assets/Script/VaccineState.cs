using UnityEngine;
using UnityEngine.VFX;

public class VaccineState : MonoBehaviour
{
    public float speed = 10f;
    public GameObject VaccineObj; // 떨어지는 백신
    public GameObject VaccineFeild;  // 백신 구역 
    private Vector3 destination;
    private PlayerAttack playerAttack;

    private bool downSign = false;
    private float timer = 0f;
    private Rigidbody2D rb;

    public SpriteRenderer VaccineRender;
    public SpriteRenderer VaccineFeildRender;
    public Sprite[] VaccineImg;
    public Sprite[] VaccineFeildImg;

    void Start()
    {
        VaccineObj.SetActive(true);
        VaccineFeild.SetActive(false);
        rb = GetComponent<Rigidbody2D>();

        playerAttack = GameObject.FindWithTag("Player").GetComponent<PlayerAttack>();
        Vector3 playerPos = GameObject.FindWithTag("Player").transform.position;
        float randX = Random.Range(-5f, 5f);
        float randY = Random.Range(-2f, 2f);
        destination = new Vector3(playerPos.x + randX, playerPos.y + randY, transform.position.z);
        //destination = new Vector3(playerPos.x , playerPos.y , transform.position.z);

        // 출발 위치는 y+10 위에서 수직으로 시작
        transform.position = new Vector3(destination.x, destination.y + 10f, transform.position.z);

        // 중력 off (옵션), 힘으로 떨어지게
        rb.gravityScale = 0f;

        // 낙하
        float forcePower = Random.Range(30,40);
        rb.AddForce(Vector2.down * forcePower, ForceMode2D.Impulse);

        if (PlayerStat.VaccineLevel == 4)
        {
            if (PlayerStat.VCFS < 3.3f) PlayerStat.VCFS = 3.0f; // 기본 수치의 백신 구역 생성
            VaccineFeild.transform.localScale = new Vector3(PlayerStat.VCFS, PlayerStat.VCFS, PlayerStat.VCFS);
            VaccineRender.sprite = VaccineImg[1];
            VaccineFeildRender.sprite = VaccineFeildImg[1];
        }
        else
        {
            if (PlayerStat.VCFS < 1.65f) PlayerStat.VCFS = 1.5f; // 기본 수치의 백신 구역 생성
            VaccineFeild.transform.localScale = new Vector3(PlayerStat.VCFS, PlayerStat.VCFS, PlayerStat.VCFS);
            VaccineRender.sprite = VaccineImg[0];
            VaccineFeildRender.sprite = VaccineFeildImg[0];
        }
    }


    void Update()
    {
        if (downSign)
        {
            timer += Time.deltaTime;
            if (timer > 3.0f)
            {
                Destroy(gameObject);
            }
        }

        MoveVaccine();
    }

    private void MoveVaccine()
    {
        Vector2 pos = transform.position;
        Vector2 dest = destination;

        // 도착지점 근처,  지나친 경우 정지
        if (Vector2.Distance(pos, dest) < 0.1f || pos.y <= dest.y)
        {
            VaccineObj.SetActive(false);
            VaccineFeild.SetActive(true);
            downSign = true;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            transform.position = destination;
        }
    }

}
