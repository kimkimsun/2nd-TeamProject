using sihyeon;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WoodUi : MonoBehaviour
{
    [Header("[버튼들]")]
    public Button agreeBtn;//동조
    public Button revoitBtn;//반란
    public Button craftBtn;//제작
    public Button officerBtn;//장교
    public Button supporterBtn;//지지자결집

    public Button emotionBtn;
    [Header("[프로필]")]
    public GameObject profileWindow;
    public GameObject profile;
    public GameObject foxBuildImage;
    public GameObject ratBuildImage;
    public GameObject rabbitBuildImage;

    //public GameObject remainSolImage;
    //public GameObject officerImage;
    //public GameObject craftCardImage;

    [Header("[값]")]
    public TextMeshProUGUI remainSolText;
    public TextMeshProUGUI officerText;
    public TextMeshProUGUI drawCardText;
    public TextMeshProUGUI foxSupportNumText;
    public TextMeshProUGUI ratSupportNumText;
    public TextMeshProUGUI rabbitSupportNumText;
    public TextMeshProUGUI birdSupportNumText;
    public TextMeshProUGUI woodActionNumText;

    public GameObject woodAlarm;
    [Header("[카드 체크]")]
    public List<Card> cards = new List<Card>();
    public enum CardUseType 
    {
        NONE,
        CRAFT,
        SUPPORT,
        OFFICER,
        BATTLE,
        BIRDUSE,
        HOSPITAL
    }
    public CardUseType cardUseType;

    void Start()
    {
        
    }
    public void SetActionNum(int actionNum, int officerNum)
    {
        woodActionNumText.text = "남은횟수\n   " + actionNum + "/" + officerNum;
    }
    public void SetAgreeBtn()
    {
        agreeBtn.onClick.RemoveAllListeners();
        agreeBtn.onClick.AddListener(() =>
        {
            RoundManager.Instance.SetOffAllEffect();
            if (RoundManager.Instance.wood.tokenNum < 10)
            {
                RoundManager.Instance.wood.SetTileEffectAgree();
                BuildingManager.Instance.selectedBuilding
                = BuildingManager.Instance.BuildingDics[Building_TYPE.WOOD_TOKEN];
                RoundManager.Instance.testType = RoundManager.SoldierTestType.Build;
            }            
            //지을 수 있는 곳 표시 추가하기
        });
    }
    public void SetRevoitBtn()
    {
        revoitBtn.onClick.RemoveAllListeners();
        revoitBtn.onClick.AddListener(() =>
        {
            RoundManager.Instance.SetOffAllEffect();
            RoundManager.Instance.wood.SetTileEffectRevoit();
            RoundManager.Instance.testType = RoundManager.SoldierTestType.Revoit;
            //지을 수 있는 곳 표시 추가하기
        });
    }
    public void SetOfficerBtn()
    {
        officerBtn.onClick.RemoveAllListeners();
        officerBtn.onClick.AddListener(() =>
        {
            //장교추가 카드사용해서 하는듯
            cardUseType = CardUseType.OFFICER;
        });
    }
    public void SetCraftBtn()
    {
        craftBtn.onClick.RemoveAllListeners();
        craftBtn.onClick.AddListener(() =>
        {
            cardUseType = CardUseType.CRAFT;
        });
    }    
    public void SetSupportBtn()
    {
        supporterBtn.onClick.RemoveAllListeners();
        supporterBtn.onClick.AddListener(() =>
        {
            cardUseType = CardUseType.SUPPORT;
        });
    }

    public void SetWoodMorning1()
    {
        revoitBtn.gameObject.SetActive(true);
    }
    public void SetWoodMorning2()
    {
        revoitBtn.gameObject.SetActive(false);
        agreeBtn.gameObject.SetActive(true);
    }
    public void SetAfternoon(bool onOff)
    {
        agreeBtn.gameObject.SetActive(false);
        craftBtn.gameObject.SetActive(onOff);
        officerBtn.gameObject.SetActive(onOff);
        supporterBtn.gameObject.SetActive(onOff);
    }
    public void SetDinner(bool onOff)
    {
        Uimanager.Instance.playerUI.moveBtn.gameObject.SetActive(onOff);
        Uimanager.Instance.playerUI.spawnBtn.gameObject.SetActive(onOff);
        Uimanager.Instance.playerUI.battleBtn.gameObject.SetActive(onOff);
        woodActionNumText.gameObject.SetActive(onOff);
    }
}
