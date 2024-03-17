using CustomInterface;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CardStrategy
{
    public Card card;
    public bool isUse;
    public CardStrategy(Card card)
    {
        this.card = card;
        card.isUse = isUse;
    }
    public abstract void UseCard();
}
public class BattleCard : CardStrategy
{
    private int damage;
    private int defense;
    private ANIMAL_COST_TYPE costType;
    public BattleCard(Card card) : base(card)
    {
        this.damage = card.damage;
        this.defense = card.defense;
        this.costType = card.costType;
    }
    public override void UseCard()
    {
        if (Uimanager.Instance.woodUi.cardUseType == WoodUi.CardUseType.BATTLE)
        {
            Debug.Log("공격카드 사용");
            BattleManager.Instance.isInit = true;
            BattleManager.Instance.attackNum += 2;
            card.isUse = true;
            RoundManager.Instance.nowPlayer.cardDecks[costType].Remove(card);
            
        }
        else if (Uimanager.Instance.woodUi.cardUseType == WoodUi.CardUseType.CRAFT)
        {
            Debug.Log(card.cardName + "제작");
            RoundManager.Instance.nowPlayer.craftedCards.Add(card);
            card.isUse = true;
            RoundManager.Instance.nowPlayer.cardDecks[costType].Remove(card);
        }
        else
        {
            Debug.Log("사용못함");            
        }
    }
}
public class DefenseCard : CardStrategy
{
    private int cost;
    private ANIMAL_COST_TYPE costType;
    public DefenseCard(Card card) : base(card)
    {
        this.cost = card.cost;
        this.costType = card.costType;
    }

    public override void UseCard()
    {
        if (Uimanager.Instance.woodUi.cardUseType == WoodUi.CardUseType.BATTLE)
        {
            Debug.Log("방어카드 사용");
            BattleManager.Instance.isInit = true;
            BattleManager.Instance.defenseNum += 1;
            RoundManager.Instance.nowPlayer.cardDecks[costType].Remove(card);
            card.isUse = true;
        }
        else if (Uimanager.Instance.woodUi.cardUseType == WoodUi.CardUseType.CRAFT)
        {
            Debug.Log(card.cardName + "제작");
            RoundManager.Instance.nowPlayer.craftedCards.Add(card);
            card.isUse = true;
            RoundManager.Instance.nowPlayer.cardDecks[costType].Remove(card);
        }
        else
        {
            Debug.Log("사용못함");

        }
    }
}
public class GetScoreCard : CardStrategy
{
    private int cost;
    private ANIMAL_COST_TYPE costType;
    public GetScoreCard(Card card) : base(card)
    {
        this.cost = card.cost;
        this.costType = card.costType;
    }

    public override void UseCard()
    {
        card.isUse = true;
        RoundManager.Instance.nowPlayer.cardDecks[costType].Remove(card);
        RoundManager.Instance.nowPlayer.Score++;
    }
}

public class Card : MonoBehaviour
{
    public Action onActive;
    public CardStrategy cardStrategy;
    public Sprite sprite;
    public bool isUse;
    public string cardName;
    public string cardInfo;
    public int damage;
    public int defense;
    public int cost;
    public ANIMAL_COST_TYPE costType;

    public CARD_SKILL_TYPE skillType;


    private void Start()
    {
        switch (skillType)
        {
            case CARD_SKILL_TYPE.BATTLE:
                cardStrategy = new BattleCard(this);
                break;
            case CARD_SKILL_TYPE.DEFENSE:
                cardStrategy = new DefenseCard(this);
                break;
            case CARD_SKILL_TYPE.GETSCORE:
                cardStrategy = new GetScoreCard(this);
                break;
        }
    }

    public void Active()//카드 사용하면 사라지고 패에서 소트
    {
        if (Uimanager.Instance.woodUi.cardUseType != WoodUi.CardUseType.NONE)
        {
            CardManager.Instance.cardDeck.Add(this);
        }
        if(Uimanager.Instance.woodUi.cardUseType == WoodUi.CardUseType.BATTLE)
        {
            cardStrategy.UseCard();
            Uimanager.Instance.woodUi.cardUseType = WoodUi.CardUseType.NONE;
            Uimanager.Instance.playerUI.battleCardsWindow.SetActive(false);
        }
        else//배틀중이 아닐 때
        {
            if (RoundManager.Instance.nowPlayer is Cat cat)
            {
                switch (Uimanager.Instance.woodUi.cardUseType)
                {
                    case WoodUi.CardUseType.NONE:
                        break;
                    case WoodUi.CardUseType.CRAFT:
                        cardStrategy.UseCard();
                        break;
                    case WoodUi.CardUseType.HOSPITAL:
                        cat.FieldHospital(this);
                        RoundManager.Instance.nowPlayer.cardDecks[costType].Remove(this);
                        isUse = true;
                        Uimanager.Instance.woodUi.cardUseType = WoodUi.CardUseType.NONE;
                        break;
                }
            }
            if (RoundManager.Instance.nowPlayer is Bird)
            {

                switch (Uimanager.Instance.woodUi.cardUseType)
                {
                    case WoodUi.CardUseType.NONE:                        
                        break;
                    case WoodUi.CardUseType.BIRDUSE:
                        RoundManager.Instance.nowPlayer.cardDecks[costType].Remove(this);
                        isUse = true;
                        break;
                    case WoodUi.CardUseType.CRAFT:
                        cardStrategy.UseCard();
                        break;
                }
            }
            else if (RoundManager.Instance.nowPlayer is Wood)
            {
                switch (Uimanager.Instance.woodUi.cardUseType)
                {
                    case WoodUi.CardUseType.NONE:
                        cardStrategy.UseCard();
                        break;
                    case WoodUi.CardUseType.CRAFT:                        
                        cardStrategy.UseCard();
                        break;
                    case WoodUi.CardUseType.SUPPORT:
                        RoundManager.Instance.wood.supportVal[costType]++;//지지자추가      
                        RoundManager.Instance.wood.SetSupportUI(costType);
                        RoundManager.Instance.nowPlayer.cardDecks[costType].Remove(this);
                        isUse = true;
                        Debug.Log(RoundManager.Instance.wood.supportVal[costType]);
                        break;
                    case WoodUi.CardUseType.OFFICER:
                        RoundManager.Instance.wood.OfficerNum++;//장교추가
                        RoundManager.Instance.nowPlayer.cardDecks[costType].Remove(this);
                        isUse = true;                        
                        break;
                }
            }
        }       

        onActive?.Invoke();
    }
}