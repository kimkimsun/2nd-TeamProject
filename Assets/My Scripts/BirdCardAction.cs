using CustomInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CARDSLOT_TYPE
{
    MOVE,
    SPAWN,
    BATTLE,
    BULID
}

public class BirdCardAction : MonoBehaviour
{
    public Button resetButton;
    public TextMeshProUGUI[] countAnimals;

    [SerializeField] private CARDSLOT_TYPE cardUse_type;
    public List<Card> copySlot;
    public List<Card> birdCards;
    public List<bool> isOver = new List<bool>();
    public List<NodeMember> tiles;

    public int curNum;
    int foxCard = 0;
    int rabbitCard = 0;
    int ratCard = 0;
    int birdCard = 0;
    int curCard = 0;

    bool isBreakRule;
    bool isEnd;
    Coroutine actionCo;
    private void Awake()
    {
        countAnimals = GetComponentsInChildren<TextMeshProUGUI>();
        countAnimals[0].text = "x " + foxCard.ToString();
        countAnimals[1].text = "x " + rabbitCard.ToString();
        countAnimals[2].text = "x " + ratCard.ToString();
        countAnimals[3].text = "x " + birdCard.ToString();
        curNum = 0;
    }

    private void Start()
    {
        resetButton.onClick.AddListener(CopyCardAdd);
        tiles = new List<NodeMember>();
        copySlot = new List<Card>();
        Uimanager.Instance.birdUI.nextButton.onClick.AddListener(() => { copySlot.Clear(); });
    }
    public int CurCard
    {
        get { return curCard; }
        set
        {
            curCard = value;
            if (curCard <= birdCards.Count - 1)
                curCard = birdCards.Count - 1;
        }
    }
    public void AddBirdCard(Card card)
    {
        birdCards.Add(card);
        CountAnimals(card.costType);
        isOver.Add(false);
    }
    public void AddCard(Card card)
    {
        Bird bird = RoundManager.Instance.bird;
        birdCards.Add(card);
        CountAnimals(card.costType);
        isOver.Add(false);
        bird.inputCard++;
        if(bird.inputCard > 0)
            Uimanager.Instance.birdUI.nextButton.gameObject.SetActive(true);
        copySlot.Add(card);
        resetButton.gameObject.SetActive(true);
    }    

    IEnumerator ActionCoroutine()
    {
        curNum = 0;
        while(curNum <= birdCards.Count - 1)
        {
            if(curNum > birdCards.Count)
            {
                break;
            }
            Debug.Log(curNum + " " + isOver.Count);
            while (true)
            {
                if(isOver[curNum])
                {
                    break;
                }

                yield return null;
            }
            curNum++;
            Debug.Log(cardUse_type);
            switch (cardUse_type)
            {
                case CARDSLOT_TYPE.SPAWN:
                    {
                        SetSpawnNode();
                        Uimanager.Instance.birdUI.sequence.CoroutineMethod(0);
                    }
                    break;
                case CARDSLOT_TYPE.MOVE:
                    {
                        SetMoveNode();
                        Uimanager.Instance.birdUI.sequence.CoroutineMethod(1);
                    }
                    break;

                case CARDSLOT_TYPE.BATTLE:
                    {
                        SetBattleNode();
                        Uimanager.Instance.birdUI.sequence.CoroutineMethod(2);
                    }
                    break;
                case CARDSLOT_TYPE.BULID:
                    {
                        SetBulidNode();
                        Uimanager.Instance.birdUI.sequence.CoroutineMethod(3);
                    }
                    break;
            }
            yield return null;
            
        }
        yield return null;
    }
    public void StartActionCo()
    {
        actionCo = RoundManager.Instance.StartCoroutine(ActionCoroutine());
    }
    public void SetBattleNode()
    {
        tiles.Clear();
        isBreakRule = false;
        Uimanager.Instance.birdUI.birdAlarm.SetActive(true);
        Uimanager.Instance.birdUI.alarmText.text = "전투를 시작할 적을 선택하세요.";
        foreach (KeyValuePair<string, List<Soldier>> battleTileCheck in RoundManager.Instance.bird.hasSoldierDic)
        {
            
            NodeMember tile = RoundManager.Instance.mapExtra.mapTiles.Find(node => node.nodeName == battleTileCheck.Key);
            // 내 병사가 위치한 타일들
            bool isbattlePlayer = RoundManager.Instance.cat.hasSoldierDic.ContainsKey(tile.nodeName) || RoundManager.Instance.wood.hasSoldierDic.ContainsKey(tile.nodeName);
            // 고양이 병사들이 위치한 타일들이 내 병사가 위치한 타일들이랑 같거나,
            // 우드 병사들이 //
            bool isBattletile = birdCards[CurCard].costType == tile.nodeType;
            // 내가 가진 카드의 타입이 내 병사가 위치한 타일의 타입과 같다면

            if (birdCards[CurCard].costType == ANIMAL_COST_TYPE.BIRD && isbattlePlayer)
            // 내 카드가 새면서 상대방이 있다면
            {
                tile.isTileCheck = true;
                RoundManager.Instance.SetEffect(tile);
                RoundManager.Instance.testType = RoundManager.SoldierTestType.Battle;
                isBreakRule = true;

            }
            else if (isBattletile && isbattlePlayer)
                // 내가 가진 카드의 타입이 같다면 그 카드는 tiles에 add됨
            {
                tiles.Add(tile);
            }
        }
        if (tiles.Count > 0) // 만약 tiles에 내용이 채워져 있다면
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                RoundManager.Instance.SetEffect(tiles[i]);
                tiles[i].isTileCheck = true;
                RoundManager.Instance.testType = RoundManager.SoldierTestType.Battle;
                isBreakRule = true;
            if (tiles[i] == RoundManager.Instance.mapController.nowTile)
            {
                RoundManager.Instance.testType = RoundManager.SoldierTestType.Battle;
                isBreakRule = true;
            }
            }

        }
        else if (!isBreakRule)
        {
            RoundManager.Instance.bird.NowLeader = LEADER_TYPE.NONE;
            RoundManager.Instance.bird.BreakingRule();
        }
    }
    public void SetBulidNode()
    {
        Uimanager.Instance.birdUI.birdAlarm.SetActive(true);
        Uimanager.Instance.birdUI.alarmText.text = "요새를 지을 공터를 선택하세요.";
        tiles.Clear();
        isBreakRule = false;
        foreach (KeyValuePair<string, List<Soldier>> soldierTileCheck in RoundManager.Instance.bird.hasSoldierDic)
        {
            NodeMember tile = RoundManager.Instance.mapExtra.mapTiles.Find(node => node.nodeName == soldierTileCheck.Key);
            if (tile.nodeType.Equals(birdCards[CurCard].costType))
            {
                tiles.Add(tile);
                isBreakRule = true;
            }
            if (birdCards[CurCard].costType == ANIMAL_COST_TYPE.BIRD)
            {
                RoundManager.Instance.testType = RoundManager.SoldierTestType.Build;
                RoundManager.Instance.SetEffect(tile);
                tile.isTileCheck = true;
                isBreakRule = true;
            }
        }
        
        if (!isBreakRule)
        {
            RoundManager.Instance.bird.NowLeader = LEADER_TYPE.NONE;
            RoundManager.Instance.bird.BreakingRule();
        }
        for (int j = 0; j < tiles.Count; j++)
        {
            RoundManager.Instance.testType = RoundManager.SoldierTestType.Build;
            RoundManager.Instance.SetEffect(tiles[j]);
            tiles[j].isTileCheck = true;

        }

    }
    public void SetMoveNode()
    {
        Uimanager.Instance.birdUI.birdAlarm.SetActive(true);
        Uimanager.Instance.birdUI.alarmText.text = "이동할 병사를 선택하세요.";
        tiles.Clear();
        isBreakRule = false;
        foreach (KeyValuePair<string, List<Soldier>> soldierTileCheck in RoundManager.Instance.bird.hasSoldierDic)
        {
            NodeMember tile = RoundManager.Instance.mapExtra.mapTiles.Find(node => node.nodeName == soldierTileCheck.Key);
            if (tile.nodeType.Equals(birdCards[CurCard].costType))
            {
                tiles.Add(tile);
            }
            if (birdCards[CurCard].costType == ANIMAL_COST_TYPE.BIRD)
            {
                RoundManager.Instance.testType = RoundManager.SoldierTestType.MoveSelect;
                RoundManager.Instance.SetEffect(tile);
                tile.isTileCheck = true;
                isBreakRule = true;
            }
            else if (birdCards[CurCard].costType == tile.nodeType)
            {
                for (int j = 0; j < tiles.Count; j++)
                {
                    RoundManager.Instance.testType = RoundManager.SoldierTestType.MoveSelect;
                    RoundManager.Instance.SetEffect(tiles[j]);
                    tiles[j].isTileCheck = true;
                    isBreakRule = true;
                }
            }
        }
        if (!isBreakRule)
        {
            RoundManager.Instance.bird.NowLeader = LEADER_TYPE.NONE;
            RoundManager.Instance.bird.BreakingRule();
        }
    }

    public void SetSpawnNode()
    {
        Uimanager.Instance.birdUI.birdAlarm.SetActive(true);
        Uimanager.Instance.birdUI.alarmText.text = "모병할 공터를 선택하세요 선택하세요.";
        foreach (KeyValuePair<string, List<GameObject>> buildingTileCheck in RoundManager.Instance.bird.hasBuildingDic)
        {
            NodeMember tile1 = RoundManager.Instance.mapExtra.mapTiles.Find(node => node.nodeName == buildingTileCheck.Key);
            if (tile1.nodeType.Equals(birdCards[CurCard].costType))
            {
                tiles.Add(tile1);
            }
            if (birdCards[CurCard].costType == ANIMAL_COST_TYPE.BIRD)
            {
                RoundManager.Instance.testType = RoundManager.SoldierTestType.BirdSpawn;
                RoundManager.Instance.SetEffect(tile1);
                tile1.isTileCheck = true;
                isBreakRule = true;
            }
            else if (birdCards[CurCard].costType == tile1.nodeType)
            {
                for (int j = 0; j < tiles.Count; j++)
                {
                    RoundManager.Instance.testType = RoundManager.SoldierTestType.BirdSpawn;
                    RoundManager.Instance.SetEffect(tiles[j]);
                    tiles[j].isTileCheck = true;
                    isBreakRule = true;
                }
            }
        }
        if (!isBreakRule)
        {
            RoundManager.Instance.bird.NowLeader = LEADER_TYPE.NONE;
            RoundManager.Instance.bird.BreakingRule();
        }
    }

    public void CardReset()
    {
        for (int i = 0; i <= birdCards.Count - 1; i++)
        {
            birdCards.RemoveAt(i);
            isOver.Clear();

        }
        foxCard = 0;
        birdCard = 0;
        rabbitCard = 0;
        ratCard = 0;

        countAnimals[0].text = "x "+foxCard;
        countAnimals[1].text = "x "+ birdCard;
        countAnimals[2].text = "x "+ rabbitCard;
        countAnimals[3].text = "x "+ ratCard;
    }

    public void CopyCardAdd()
    {
        for (int i = 0; i < copySlot.Count; i++)
        {
            RoundManager.Instance.bird.inven.AddCard(copySlot[i]);
            switch (copySlot[i].costType)
            {
                case ANIMAL_COST_TYPE.FOX:
                    foxCard--;
                    countAnimals[0].text = "x " + foxCard.ToString();
                    break;
                case ANIMAL_COST_TYPE.RABBIT:
                    rabbitCard--;
                    countAnimals[1].text = "x " + rabbitCard.ToString();
                    break;
                case ANIMAL_COST_TYPE.RAT:
                    ratCard--;
                    countAnimals[2].text = "x " + ratCard.ToString();
                    break;
                case ANIMAL_COST_TYPE.BIRD:
                    birdCard--;
                    countAnimals[3].text = "x " + birdCard.ToString();
                    break;

            }
        }
        for (int i = 0; i < birdCards.Count; i++)
        {
            for (int j = 0; j < copySlot.Count; j++)
            if (copySlot[j] == birdCards[i])
            {
                birdCards.Remove(birdCards[i]);
            }
        }
        copySlot.Clear();
        resetButton.gameObject.SetActive(false);
        RoundManager.Instance.bird.inputCard = 0;
    }

    public void CountAnimals(ANIMAL_COST_TYPE cost)
    {

        switch (cost)
        {
            case ANIMAL_COST_TYPE.FOX:
                foxCard++;
                countAnimals[0].text = "x " + foxCard.ToString();
                break;
            case ANIMAL_COST_TYPE.RABBIT:
                rabbitCard++;
                countAnimals[1].text ="x " + rabbitCard.ToString();
                break;
            case ANIMAL_COST_TYPE.RAT:
                ratCard++;
                countAnimals[2].text = "x " + ratCard.ToString();
                break;
            case ANIMAL_COST_TYPE.BIRD:
                birdCard++;
                countAnimals[3].text = "x " + birdCard.ToString();
                break;

        }

    }
    
}
