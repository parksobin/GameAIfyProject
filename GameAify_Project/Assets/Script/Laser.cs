using UnityEngine;

public class Laser : MonoBehaviour
{
    private bool PlayerHealthMinus = false; //ÇÃ·¹ÀÌ¾î µ¥¹ÌÁö Áßº¹ ¹æÁö
    private float delayTime = 0; //Áßº¹¹æÁö µô·¹ÀÌÅ¸ÀÓ

    private void Update()
    {
        if(PlayerHealthMinus)
        {
            PlayerStat.HP -= 30;
            delayTime += Time.deltaTime;
            if ((delayTime > 1.0f))
            {
                Debug.Log("À¸°¼°¼°¼¤Á°¼");
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
