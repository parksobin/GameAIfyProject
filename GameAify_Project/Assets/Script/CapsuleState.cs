using System.ComponentModel.Design;
using UnityEngine;

public class CapsuleState : MonoBehaviour
{
    private GameObject Player;
    private PlayerAttack playerAttack;
    public static bool CapsuleActive = true; // 캡슐 활성화 상태
    private int count=0; //타격 횟수
    private int maxCount=1;
    private SpriteRenderer SpriteRenderer;
    private CircleCollider2D collider2D;

    private void Start()
    {
        Player = GameObject.Find("Player");
        playerAttack = Player.GetComponent<PlayerAttack>();
        SpriteRenderer= gameObject.GetComponent<SpriteRenderer>();
        collider2D = gameObject.GetComponent<CircleCollider2D>();
        ActiveDesignerEventArgs(1.0f);
    }
    private void Update()
    {
        transform.position = Player.transform.position;
        CapsuleLevelCheck();
    }
    private void OnMouseDown() // 테스트용
    {
        count++;
        if (count >= maxCount)
        {
            count = 0;
            CapsuleActive = false;
            PlayerStat.CapsuleState = false;
            ActiveDesignerEventArgs(0f);
        }
    }
    private void CapsuleLevelCheck() //레벨별 캡슐타격 홋수
    {
        switch (PlayerStat.CapsuleLevel)
        {
            case 1:
                maxCount = 1;
                break;
            case 2:
                maxCount = 2; 
                break;
            case 3:
                maxCount = 2;
                break;
            case 4:
                maxCount = 3;
                break;
        }
    }

    private void CapsuleHide(int max)
    {
        count++;
        if (count >= max)
        {
            count = 0;
            CapsuleActive = false;
            //gameObject.SetActive(false);
            ActiveDesignerEventArgs(0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            count++;
            if (count >= maxCount)
            {
                count = 0;
                CapsuleActive = false;
                PlayerStat.CapsuleState = false;
                ActiveDesignerEventArgs(0f);
            }
        }
    }

    public void ActiveDesignerEventArgs(float set)
    {
        // 콜라이더와 스프라이트 껐다키며 setactive 가 껐다 켜지는 것처럼 보이게 하기
        // 이렇게 안 하면 다른 스크립트에서 null값 오류 장난 아님
        SpriteRenderer.color = new Color(1f, 1f, 1f, set); //투명하게 =0f / 다시선명하게는 1f
        collider2D.enabled = set == 1f;  //1이면 true 아니면 false
    }
}
