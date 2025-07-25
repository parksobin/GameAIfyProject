using UnityEngine;

public class VaccineState : MonoBehaviour
{
    public float speed = 10f;
    public GameObject VaccineObj; //떨어지는 백신
    public GameObject VaccineFeild;  //백신 구역
    private Vector3 destination;  
    private Vector3 direction; //방향 (대각선)

    private Rigidbody2D rb;

    void Start()
    {
        VaccineObj.SetActive(true);
        VaccineFeild.SetActive(false);
        rb = GetComponent<Rigidbody2D>();

        float randX = Random.Range(2, 5);
        float randY = Random.Range(1, 3);

        Vector3 departure = transform.position; //출발지점
        destination = new Vector3(departure.x - randX, departure.y - randY, departure.z); //도착지 - 좌측 하단으로 낙하되도록
        direction = (destination - departure).normalized;  //방향

        rb.linearVelocity = direction * speed;
       
    }

    void Update()
    {
        MoveVaccine();
    }

    private void MoveVaccine() //백신 낙하 함수
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Vector2 dest = new Vector2(destination.x, destination.y);
        Vector2 travelDir = (dest - pos).normalized; 

        //낙하 지점 근처에 오면 정지하여 백신 물체 숨기고, 구역 보이게 함
        if (Vector2.Distance(pos, dest) < 0.1f || Vector2.Dot(direction, travelDir) < 0f)
        {
            VaccineObj.SetActive(false );
            VaccineFeild.SetActive(true );
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f; // 중력 제거로 멈춤 유지
            transform.position = gameObject.transform.position; // 정확한 위치 고정
            rb.bodyType = RigidbodyType2D.Kinematic;   // 정지 상태로 전환
            enabled = false;

        }
    }
}
