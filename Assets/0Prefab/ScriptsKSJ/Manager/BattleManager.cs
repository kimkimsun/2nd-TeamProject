using Cinemachine;
using sihyeon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : SingleTon<BattleManager>
{
    public string saveNodeTileName;
    public Player battleP1;
    public Player battleP2;
    public bool isInit;
    public int attackNum;
    public int defenseNum;
    public int diceP1Num;
    public int diceP2Num;

    public List<Soldier> battleP1Soldiers;
    public List<Soldier> battleP2Soldiers;

    public List<Building> battleP1Buildings;
    public List<Building> battleP2Buildings;

    public TextMeshProUGUI battleP1Text;
    public TextMeshProUGUI battleP2Text;
    public CinemachineVirtualCamera battleVirtualCamera;
    public Canvas battleOffUI;
    public Canvas battleOffUI2;
    public Canvas battleOffUI3;
    public BattleScene battleScene;

    private new void Awake()
    {
        base.Awake();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log(RoundManager.Instance.bird.NowLeader);
        }
        foreach(KeyValuePair<string,List<Soldier>> keyValuePair in RoundManager.Instance.bird.hasSoldierDic)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                Debug.Log(keyValuePair.Key);
            }
        }
        
    }
    public void InitBattle()
    {
        isInit = false;
        foreach (Button button in Uimanager.Instance.playerUI.buttons)
        {
            button.onClick.RemoveAllListeners();
        }
        RoundManager.Instance.cat.battleSoldierNum = 0;
        RoundManager.Instance.bird.battleSoldierNum = 0;
        RoundManager.Instance.wood.battleSoldierNum = 0;
        //초기화
        battleP1 = RoundManager.Instance.nowPlayer;

        int tempNum = 0;
        if (RoundManager.Instance.cat.isOver == true) 
        {
            Uimanager.Instance.playerUI.buttons[tempNum].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                RoundManager.Instance.nowPlayer.gameObject.name + " vs Cat"; 
            Uimanager.Instance.playerUI.buttons[tempNum].onClick.AddListener(() =>
            {
                if (RoundManager.Instance.cat.hasSoldierDic.ContainsKey(RoundManager.Instance.mapController.nowTile.nodeName)) //병사를 가지고 있는 타일이 있으면
                {
                    battleP2Soldiers = RoundManager.Instance.cat.
                    hasSoldierDic[RoundManager.Instance.mapController.nowTile.nodeName];
                    RoundManager.Instance.cat.battleSoldierNum = battleP2Soldiers.Count;
                }
                battleP2 = RoundManager.Instance.cat;
                if(!RoundManager.Instance.nowPlayer.craftedCards.Exists(card => card.skillType ==
                CustomInterface.CARD_SKILL_TYPE.BATTLE))
                    StartBattle();
                else
                {
                    Debug.Log("존재");
                    Uimanager.Instance.playerUI.battleCardsWindow.SetActive(true);
                }
            });
            tempNum++;
        }
        if (RoundManager.Instance.bird.isOver == true)
        {
            Uimanager.Instance.playerUI.buttons[tempNum].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                RoundManager.Instance.nowPlayer.gameObject.name + " vs Bird";

            Uimanager.Instance.playerUI.buttons[tempNum].onClick.AddListener(() =>
            {
                if (RoundManager.Instance.bird.hasSoldierDic.ContainsKey(RoundManager.Instance.mapController.nowTile.nodeName))
                {
                    battleP2Soldiers = RoundManager.Instance.bird.
                    hasSoldierDic[RoundManager.Instance.mapController.nowTile.nodeName];
                    RoundManager.Instance.bird.battleSoldierNum = battleP2Soldiers.Count;
                }
                battleP2 = RoundManager.Instance.bird;
                if (!RoundManager.Instance.nowPlayer.craftedCards.Exists(card => card.skillType ==
                CustomInterface.CARD_SKILL_TYPE.BATTLE))
                    StartBattle();
                else
                {
                    Debug.Log("존재");
                    Uimanager.Instance.playerUI.battleCardsWindow.SetActive(true);
                }
            });
            tempNum++;
        }
        if (RoundManager.Instance.wood.isOver == true)
        {
            Uimanager.Instance.playerUI.buttons[tempNum].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                RoundManager.Instance.nowPlayer.gameObject.name + " vs Wood";
            Uimanager.Instance.playerUI.buttons[tempNum].onClick.AddListener(() =>
            {
                if (RoundManager.Instance.wood.hasSoldierDic.ContainsKey(RoundManager.Instance.mapController.nowTile.nodeName))
                {
                    battleP2Soldiers = RoundManager.Instance.wood.
                    hasSoldierDic[RoundManager.Instance.mapController.nowTile.nodeName];
                    RoundManager.Instance.wood.battleSoldierNum = battleP2Soldiers.Count;
                }
                battleP2 = RoundManager.Instance.wood;
                if (!RoundManager.Instance.nowPlayer.craftedCards.Exists(card => card.skillType ==
                CustomInterface.CARD_SKILL_TYPE.BATTLE))
                    StartBattle();
                else
                {
                    Debug.Log("존재");
                    Uimanager.Instance.playerUI.battleCardsWindow.SetActive(true);
                }
            });
            tempNum++;
        }//현재 턴이 아닌애들
    }
    public void StartBattle()//주사위가 나온사용자가 그 값을 가지게 해놨음 세세한건 나중에 수정
    {
        battleVirtualCamera.Priority = 9;
        battleOffUI.gameObject.SetActive(false);
        battleOffUI2.gameObject.SetActive(false);
        battleOffUI3.gameObject.SetActive(true);
        battleScene.gameObject.SetActive(true);
        battleScene.StartBattle();
        if (!isInit)
        {
            attackNum = 0;
            defenseNum = 0;
            isInit = true;
        }
        RoundManager.Instance.SetOffAllEffect();
        battleP1Soldiers = RoundManager.Instance.nowPlayer.hasSoldierDic[RoundManager.Instance.mapController.nowTile.nodeName];
        Debug.Log("찍은 타일" + RoundManager.Instance.mapController.nowTile.nodeName);
        Debug.Log("나우 플레이어" + RoundManager.Instance.nowPlayer);
        foreach(KeyValuePair<string,List<Soldier>> keyValuePair in RoundManager.Instance.nowPlayer.hasSoldierDic)
        {
            if(keyValuePair.Key == RoundManager.Instance.mapController.nowTile.nodeName)
            {
                Debug.Log("제발 나와라" + keyValuePair.Value.Count);
                battleP1.battleSoldierNum = keyValuePair.Value.Count;
            }

        }
        battleP2.battleSoldierNum = battleP2Soldiers.Count;
        battleP1.battleBuildingNum = battleP1Buildings.Count;
        battleP2.battleBuildingNum = battleP2Buildings.Count;
        Uimanager.Instance.battlep1.ActionP1();
        Uimanager.Instance.battlep2.ActionP2();
        diceP1Num = Random.Range(0, 4);//p2가 나온숫자 p1의 병사가 죽어야하는 숫자
        diceP2Num = Random.Range(0, 4);//p1이 나온숫자 p2의 병사가 죽어야하는 숫자
        int tempNum;
        if (battleP1 is Wood)
        {            
            if(diceP1Num > diceP2Num)
            {
                tempNum = diceP1Num;
                diceP1Num = diceP2Num;
                diceP2Num = tempNum;
            }
        }
        if(battleP2 is Wood)
        {
            if (diceP2Num > diceP1Num)
            {
                tempNum = diceP1Num;
                diceP1Num = diceP2Num;
                diceP2Num = tempNum;
            }
        }

        if (battleP1 is Bird bird)
        {        
            if (RoundManager.Instance.bird.NowLeader == LEADER_TYPE.COMMANDER)
                diceP2Num++;
        }
        //더 큰 숫자가 dicep2num에 가야함. (공격)
        Debug.Log("배틀시작");
        diceP1Num = (diceP1Num > battleP2.battleSoldierNum) ? battleP2.battleSoldierNum : diceP1Num;//+ + battleP2.battleBuildingNum
        diceP1Num = (diceP1Num > battleP1.battleSoldierNum) ? battleP1.battleSoldierNum : diceP1Num;
        //여기에 플레이어 2번의 버드의 지도자가 데미지 1이면 다이스1num에 1추가

        for (int i = 0; i < diceP1Num; i++)
        {
            if (battleP2.battleSoldierNum == 0)
            {
                break;
            }
            GameObject tempObj = battleP2Soldiers[battleP2.battleSoldierNum - 1].gameObject;
            battleP2Soldiers.RemoveAt(battleP2.battleSoldierNum - 1);
            Destroy(tempObj);
            battleP2.battleSoldierNum--;
            if (battleP2 is Wood wood)
            {
                wood.RemainSoldierNum++;
            }
            if (battleP2 is Cat cat)
            {
                if (!cat.deadSoldierNum.ContainsKey(RoundManager.Instance.mapController.nowTile.nodeType))
                    cat.deadSoldierNum.Add(RoundManager.Instance.mapController.nowTile.nodeType, 0);
                cat.deadSoldierNum[RoundManager.Instance.mapController.nowTile.nodeType]++;
            }
            if (battleP2 is Bird birdTow)
            {
                Uimanager.Instance.birdUI.birdAlarm.gameObject.SetActive(false);
                bool isTargetBuilding = battleP2.hasBuildingDic.ContainsKey(RoundManager.Instance.mapController.nowTile.nodeName);
                bool isCheckBuilding = isTargetBuilding && battleP2.hasBuildingDic[RoundManager.Instance.mapController.nowTile.nodeName].Count > 0;
                if(RoundManager.Instance.bird.NowLeader == LEADER_TYPE.TYRANT ||(diceP2Num > battleP2Soldiers.Count && isCheckBuilding))
                {
                    List<GameObject> testObj = new List<GameObject>();
                    testObj.Add(battleP2.hasBuildingDic[RoundManager.Instance.mapController.nowTile.nodeName][0]);
                    Destroy(battleP2.hasBuildingDic[RoundManager.Instance.mapController.nowTile.nodeName][0]);
                    battleP2.hasBuildingDic.Remove(RoundManager.Instance.mapController.nowTile.nodeName);
                    RoundManager.Instance.bird.Score++;
                }

                else if(diceP2Num > battleP2Soldiers.Count && isCheckBuilding)
                {
                    List<GameObject> testObj = new List<GameObject>();
                    testObj.Add(battleP2.hasBuildingDic[RoundManager.Instance.mapController.nowTile.nodeName][0]);
                    Destroy(battleP2.hasBuildingDic[RoundManager.Instance.mapController.nowTile.nodeName][0]);
                    battleP2.hasBuildingDic.Remove(RoundManager.Instance.mapController.nowTile.nodeName);
                }
            }
        }
        diceP2Num = (diceP2Num > battleP1.battleSoldierNum) ? battleP1.battleSoldierNum : diceP2Num;// + battleP1.battleBuildingNum
        diceP2Num = (diceP2Num > battleP2.battleSoldierNum) ? battleP2.battleSoldierNum : diceP2Num;// + battleP1.battleBuildingNum
        //여기에 플레이어 1번의 버드의 지도자가 데미지 1이면 다이스2num에 1추가
        for (int i = 0; i < diceP2Num; i++)
        {
            if (battleP1.battleSoldierNum == 0)
            {
                break;
            }
            GameObject tempObj = battleP1Soldiers[battleP1.battleSoldierNum - 1].gameObject;
            battleP1Soldiers.RemoveAt(battleP1.battleSoldierNum - 1);
            Destroy(tempObj);
            battleP1.battleSoldierNum--;
            if (battleP1 is Wood wood)
            {
                wood.RemainSoldierNum++;
            }
            if (battleP1 is Cat cat)
            {
                if (!cat.deadSoldierNum.ContainsKey(RoundManager.Instance.mapController.nowTile.nodeType))
                    cat.deadSoldierNum.Add(RoundManager.Instance.mapController.nowTile.nodeType, 0);
                cat.deadSoldierNum[RoundManager.Instance.mapController.nowTile.nodeType]++;
            }

        }
        if (RoundManager.Instance.cat.deadSoldierNum.ContainsKey(RoundManager.Instance.mapController.nowTile.nodeType))
        {
            Debug.Log(RoundManager.Instance.cat.deadSoldierNum[RoundManager.Instance.mapController.nowTile.nodeType]);

        }
        Debug.Log("플레이어2가 죽어야하는 수" + diceP1Num);
        Debug.Log("플레이어1가 죽어야하는 수" + diceP2Num);
        Uimanager.Instance.playerUI.battleWindow.SetActive(false);
        if (battleP1 is Wood)
        {
            RoundManager.Instance.wood.BattleActionNum++;
        }
    }
}