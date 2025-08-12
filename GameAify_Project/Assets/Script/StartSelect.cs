using UnityEngine;

public class StartSelect : MonoBehaviour
{
    public GameObject StartSelectPannel;
    public GameObject StartBtn;
    public static int Weapon=0; //미선택 0,1~5 순서대로 저장
    public static int Active=0; //미선택 0, 1~2 순서대로 저장
               

    void Start()
    {
        StartSelectPannel.SetActive(false);
        StartBtn.SetActive(false);
        //Time.timeScale = 0.0f;
    }

    void Update()
    {
        
    }

    public void WeaponClick(int input)  //무기 입력값 받아 선택여부 확인
    {
        Weapon = input;
        ViewStartBtn();
    }

    public void ActiveClick(int input)  //무기 입력값 받아 선택여부 확인
    {
        Active = input;
        ViewStartBtn();
    }

    private void ViewStartBtn() // 무기와 액티브스킬 모두 0이 아니어야 시작버튼 활성화
    {
        if(Weapon !=0 && Active !=0)
        {
            StartBtn.SetActive(true);
        }
        else
        {
            StartBtn.SetActive(false);
        }
    }

    public void StartBtnClick()
    {
        StartSelectPannel.SetActive(false);
        StartBtn.SetActive(false);
        Time.timeScale = 1.0f;
    }



}
