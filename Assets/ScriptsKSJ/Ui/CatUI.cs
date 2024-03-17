using sihyeon;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatUI : MonoBehaviour
{
    public Button emotionBtn;
    [Header("[프로필]")]
    public GameObject profileWindow;
    public GameObject profile;


    [Header("[값]")]
    public TextMeshProUGUI remainSolText;
    public TextMeshProUGUI woodProductText;
    public TextMeshProUGUI craftCardText;
    [Header("[행동 횟수]")]
    public GameObject actionNum;
    public TextMeshProUGUI actionNumText;
    [Header("[프로필 남은 건물 잔량]")]
    public TextMeshProUGUI remainSawmillsText;
    public TextMeshProUGUI remainWorkshopsText;
    public TextMeshProUGUI remainBarracksText;
    [Header("[프로필 제작비용]")]
    public TextMeshProUGUI sawmillCostText;
    public TextMeshProUGUI workshopCostText;
    public TextMeshProUGUI barracksCostText;

    [Header("[건설창 이미지]")]
    public GameObject bulidSectionWindow;
    public Button bulidSectionSawmill;
    public Button bulidSectionWorkshop;
    public Button bulidSectionBarracks;
    public Button bulidSectionWindowExit;
    [Header("[건설창 제작보너스]")]
    public TextMeshProUGUI bulidSectionSawmillBonusText;
    public TextMeshProUGUI bulidSectionWorkshopBonusText;
    public TextMeshProUGUI bulidSectionBarracksBonusText;
    public GameObject bulidSectionBonusDraw;
    [Header("[건설창 제작비용]")]
    public TextMeshProUGUI bulidSectionSawmillCostText;
    public TextMeshProUGUI bulidSectionWorkshopCostText;
    public TextMeshProUGUI bulidSectionBarracksCostText;

    private void Start()
    {
        SetSawmillBtn();
        SetWorkShopBtn();
        SetBarrackBtn();
        ExitbulidSectionWindowBtn();
    }

    // 고양이 건물 종류 선택
    public void SetSawmillBtn()
    {
        Uimanager.Instance.catUI.bulidSectionSawmill.onClick.RemoveAllListeners();
        Uimanager.Instance.catUI.bulidSectionSawmill.onClick.AddListener(() => {
            BuildingManager.Instance.selectedBuilding
            = BuildingManager.Instance.BuildingDics[Building_TYPE.CAT_SAWMILL];
            bulidSectionWindow.SetActive(false);
        });
    }
    public void SetWorkShopBtn()
    {
        Uimanager.Instance.catUI.bulidSectionWorkshop.onClick.RemoveAllListeners();
        Uimanager.Instance.catUI.bulidSectionWorkshop.onClick.AddListener(() => {
            BuildingManager.Instance.selectedBuilding
            = BuildingManager.Instance.BuildingDics[Building_TYPE.CAT_WORKSHOP];
            bulidSectionWindow.SetActive(false);
        });
    }
    public void SetBarrackBtn()
    {
        Uimanager.Instance.catUI.bulidSectionBarracks.onClick.RemoveAllListeners();
        Uimanager.Instance.catUI.bulidSectionBarracks.onClick.AddListener(() => {
            BuildingManager.Instance.selectedBuilding
            = BuildingManager.Instance.BuildingDics[Building_TYPE.CAT_BARRACKS];
            bulidSectionWindow.SetActive(false);
        });
    }

    public void ExitbulidSectionWindowBtn()
    {
        bulidSectionWindowExit.onClick.RemoveAllListeners();
        bulidSectionWindowExit.onClick.AddListener(() =>
        {
            RoundManager.Instance.testType = RoundManager.SoldierTestType.Select;
            bulidSectionWindow.SetActive(false);
        });
    }



  
}
