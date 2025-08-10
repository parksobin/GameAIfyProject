using UnityEngine;

public class PlayerFlow : MonoBehaviour
{
    private Vector2 moveDir; // 발사 방향 
    public float speed = 7f; // 이동 속도   
    private SpriteRenderer SquareRender;
    private float OutoHideTime = 0; //자동으로 n초뒤 사라지게 하기 위한 초수
    void Start()
    {

        GameObject playerObj = GameObject.Find("Player");

        playerObj = GameObject.Find("Player");
        if (gameObject.name.StartsWith("Square"))
        {
            SquareRender = gameObject.GetComponent<SpriteRenderer>();
            SquareRender.enabled = true;
        }
        // 처음 생성될 때 플레이어  무기 방향 계산
        moveDir = (playerObj.transform.position - transform.position).normalized;

        // 각도 회전 적용
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Update()
    {
        // 계속 같은 방향으로만 이동
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
        OutoHideTime += Time.deltaTime;
        if(OutoHideTime >2f) //n초뒤 사라지기 -> 나중에 수정
        { Destroy(gameObject); }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Destroy(gameObject);
        }
    }
}