using UnityEngine;

public class CapsuleState : MonoBehaviour
{
    private GameObject Player;
    private PlayerAttack playerAttack;
    public bool CapsuleActive = true; // 캡슐 활성화 상태
    private int count=0; //타격 횟수
    private void Start()
    {
        Player = GameObject.Find("Player");
        playerAttack = Player.GetComponent<PlayerAttack>();
    }
    private void Update()
    {
        transform.position = Player.transform.position; 
    }
    private void OnMouseDown()  //이거 나중ㅇㅔ 변경
    {
        CapsuleLevelCheck();
    }
    private void CapsuleLevelCheck() //레벨별 캡슐타격 홋수
    {
        int maxCount;
        switch (PlayerAttack.CapsuleLevel)
        {
            case 1:
                maxCount = 1;
                CapsuleHide(maxCount);
                break;
            case 2:
                maxCount = 2; 
                CapsuleHide(maxCount);
                break;
            case 3:
                maxCount = 2;
                CapsuleHide(maxCount);
                break;
            case 4:
                maxCount = 3;
                CapsuleHide(maxCount);
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
            gameObject.SetActive(false);
        }
    }
}
