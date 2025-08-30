using System.ComponentModel.Design;
using UnityEngine;

public class CapsuleState : MonoBehaviour
{
    private GameObject Player;
    private PlayerAttack playerAttack;
    public static bool CapsuleActive = true; // 캡슐 활성화 상태
    public static int collisionCount=0; //타격 횟수
    public static int maxCollisionCount=1;
    public static SpriteRenderer SpriteRenderer;

    void Awake()
    {
        CapsuleActive = true;
        collisionCount = 0;
        maxCollisionCount = 1;
    }

    private void Start()
    {
        Player = GameObject.Find("Player");
        playerAttack = Player.GetComponent<PlayerAttack>();
        SpriteRenderer= gameObject.GetComponent<SpriteRenderer>();
        ActiveDesignerEventArgs(1.0f);
    }
    private void Update()
    {
        transform.position = Player.transform.position;
        CapsuleLevelCheck();
    }
    /*
    private void OnMouseDown() // 테스트용
    {
        collisionCount++;
        if (collisionCount >= maxCollisionCount)
        {
            collisionCount = 0;
            CapsuleActive = false;
            PlayerStat.CapsuleState = false;
            ActiveDesignerEventArgs(0f);
        }
    }
    */
    private void CapsuleLevelCheck() //레벨별 캡슐타격 홋수
    {
        switch (PlayerStat.CapsuleLevel)
        {
            case 1:
                maxCollisionCount = 1;
                break;
            case 2:
                maxCollisionCount = 2; 
                break;
            case 3:
                maxCollisionCount = 2;
                break;
            case 4:
                maxCollisionCount = 3;
                break;
        }
    }

    private void CapsuleHide(int max)
    {
        collisionCount++;
        if (collisionCount >= max)
        {
            collisionCount = 0;
            CapsuleActive = false;
            //gameObject.SetActive(false);
            ActiveDesignerEventArgs(0f);
        }
    }

    public static void CapsuleControl()
    {
        collisionCount++;
        if (collisionCount >= maxCollisionCount)
        {
            collisionCount = 0;
            CapsuleActive = false;
            PlayerStat.CapsuleState = false;
            ActiveDesignerEventArgs(0f);
        }
    }

    public static void ActiveDesignerEventArgs(float set)
    {
        // 콜라이더와 스프라이트 껐다키며 setactive 가 껐다 켜지는 것처럼 보이게 하기
        // 이렇게 안 하면 다른 스크립트에서 null값 오류 장난 아님
        SpriteRenderer.color = new Color(1f, 1f, 1f, set); //투명하게 =0f / 다시선명하게는 1f
    }
}
