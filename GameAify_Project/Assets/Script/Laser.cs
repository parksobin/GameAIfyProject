using UnityEngine;

public class Laser : MonoBehaviour
{
    private bool PlayerHealthMinus = false; //�÷��̾� ������ �ߺ� ����
    private bool minus =false;
    private float delayTime = 0; //�ߺ����� ������Ÿ��
    private float FixTime = 1.5f; //���� �ߺ� ���� Ÿ�� 

    private void Update()
    {
        if(PlayerHealthMinus)
        {
            // if(minus) PlayerStat.HP -= 30; -> �����ʿ� �Ф�
            delayTime += Time.deltaTime;
            if ((delayTime > FixTime))
            {
                Debug.Log("�÷��̾� ����");
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
