using System.Xml.Linq;
using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform player;
    public Orbit orbitSpawner;

    private Rigidbody2D rb;
    private Vector2 movement;
    public TextMeshProUGUI xText;
    public TextMeshProUGUI yText;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Guarditem"))
        {
            orbitSpawner.AddOrbitingObject(); // �ϳ� �� �߰� ��û
            Destroy(other.gameObject); // ������ ����
        }
    }
}
