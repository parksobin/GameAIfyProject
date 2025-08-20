using UnityEngine;

public class Laser : MonoBehaviour
{
    private bool PlayerHealthMinus = false; //�÷��̾� ������ �ߺ� ����
    private float delayTime = 0; //�ߺ����� ������Ÿ��

    private void Update()
    {
        if(PlayerHealthMinus)
        {
            PlayerStat.HP -= 30;
            delayTime += Time.deltaTime;
            if ((delayTime > 1.0f))
            {
                Debug.Log("������������");
                PlayerHealthMinus=!PlayerHealthMinus;  
                delayTime = 0;
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
