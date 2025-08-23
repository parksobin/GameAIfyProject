using UnityEngine;

public class Laser : MonoBehaviour
{
    private bool PlayerHealthMinus = false; //플레이어 데미지 중복 방지
    private bool minus =false;
    private float delayTime = 0; //중복방지 딜레이타임
    private float FixTime = 1.5f; //고정 중복 방지 타임 

    private void Update()
    {
        if(PlayerHealthMinus)
        {
            // if(minus) PlayerStat.HP -= 30; -> 수정필요 ㅠㅠ
            delayTime += Time.deltaTime;
            if ((delayTime > FixTime))
            {
                Debug.Log("플레이어 피해");
                PlayerHealthMinus=!PlayerHealthMinus;  
                delayTime = 0;
                minus=false;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerHealthMinus = !PlayerHealthMinus;

        }
    }
}
