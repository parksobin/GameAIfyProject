using UnityEngine;

public class EnemyState : MonoBehaviour
{
    private GameObject playerObj; //플레이어 오브젝트
    private Vector2 direction; //플레이어쪽 방향
    private float moveSpeed; //이동속도

    void Start()
    {
        playerObj = GameObject.Find("Player");
        moveSpeed = Random.Range(1, 4);
    }
    private void Update()
    {
        PlayerFollow();
    }
    private void PlayerFollow() //플레이어 방향으로 따라가는 함수
    {
        direction = (playerObj.transform.position - transform.position).normalized;
        gameObject.transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
}
