using UnityEngine;
using System.Collections.Generic;

public class AutoRemove : MonoBehaviour
{
    private List<GameObject> container;
    private GameObject self;

    public void Init(List<GameObject> list, GameObject obj)
    {
        container = list;
        self = obj;
    }

    void OnDestroy()
    {
        if (container != null && self != null)
        {
            container.Remove(self);
        }
    }
}
