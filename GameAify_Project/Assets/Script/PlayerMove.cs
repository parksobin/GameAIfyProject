using System.Xml.Linq;
using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform player;
    public Orbit orbitSpawner;

    private SpriteRenderer sr;
    public Sprite idleSprite;
    public Sprite walkUpSprite;
    private Rigidbody2D rb;
    private Vector2 movement;
    public TextMeshProUGUI xText;
    public TextMeshProUGUI yText;

    public static float HP = 5.0f;
    public TextMeshProUGUI hpText;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // �Է� ó��
        movement.x = Input.GetAxisRaw("Horizontal"); 
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize(); // �밢�� �ӵ� ����

        if (player != null && xText != null && yText != null )
        {
            float x = player.position.x;
            float y = player.position.y;
            xText.text = $"X : {x:F2}";
            yText.text = $"Y : {y:F2}";
        }

        if (Input.GetKey(KeyCode.W)) sr.sprite = walkUpSprite;
        else sr.sprite = idleSprite;

        CheckHP();
        hpText.text = "HP : " + HP.ToString("N1");
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void CheckHP()
    {
        if (HP >= 5.0f) HP = 5.0f;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        /*if (other.CompareTag("Guarditem"))
        {
            orbitSpawner.AddOrbitingObject(); // �ϳ� �� �߰� ��û
            Destroy(other.gameObject); // ������ ����
        }*/
        if (other.CompareTag("Enemy"))
        {
            HP -= 1.0f;
        }
    }
}
