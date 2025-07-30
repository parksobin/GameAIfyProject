using UnityEngine;

public class AttackRange : MonoBehaviour
{
    private Vector3 startPosition;

    public void SetStartPosition(Vector3 pos)
    {
        startPosition = pos;
    }

    void Update()
    {
        float distance = Vector3.Distance(startPosition, transform.position);
        if (distance >= PlayerAttack.AttackRange)
        {
            Destroy(gameObject);
        }
    }
}
