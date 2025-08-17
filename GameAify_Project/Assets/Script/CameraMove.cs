using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public StageSetting StageSetting;
    public Transform target;
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (!StageSetting.InbossStage) //보스맵 아닐 때만 카메라 이동
        {
            transform.position = target.position + offset;
        }
        else
        {
            gameObject.transform.position = new Vector3(0,0, -10);    
        }
    }
}
