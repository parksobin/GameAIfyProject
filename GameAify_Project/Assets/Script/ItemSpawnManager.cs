using System;
using Unity.VisualScripting;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    public GameObject ItemPanel;
    public bool isChoose = false;
    private float[] ItemSelectOn =
    {
        10, 25, 50, 100, 250, 400, 500, 650, 850,
        1000, 1250, 1500, 1850, 2200, 2500, 3000,
        3500, 4100, 4650, 4950 
    };


    void Start()
    {
        
    }

    void Update()
    {
        Debug.Log("Count : " + PlayerStat.currentGauge);
        for(int i = 0; i < 20; i++)
        {
            if (PlayerStat.currentGauge == ItemSelectOn[i] && !isChoose)
            {
                ItemSelect();
                isChoose = true;
            }
        }
    }

    private void ItemSelect()
    {
        Time.timeScale = 0.0f;
        ItemPanel.SetActive(true);
        if(ClickChecker.SelectedItem)
        {
            Time.timeScale = 1.0f;
            ItemPanel.SetActive(false);
            isChoose = false;
        }
    }
}
