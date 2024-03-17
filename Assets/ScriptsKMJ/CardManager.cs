// using SJ;
using CustomInterface;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : SingleTon<CardManager>
{
    public PlayerInventory playerInventory;
    List<Card> cards = new List<Card>();//카드의 저장소
    public List<Card> cardDeck = new List<Card>();//카드들 덱 버린거 추가
    public List<Card> shuffledCards = new List<Card>();//꺼내오는 거
    [SerializeField]
    private Transform deckParent;

    private new void Awake()
    {
        base.Awake();
        cards = Resources.LoadAll<Card>("CardPrefabs").ToList();
        //Debug.Log(cards[0].damage);
    }
    private void Start()
    {


        //randomCard = Random.Range(0, 1);
        foreach (Card cardTemp in cards)//같은거 건들여서 같은곳 참조해서 3번씩 써짐
        {
            Card card1 = Instantiate(cardTemp, deckParent);
            Card card2 = Instantiate(cardTemp, deckParent);
            Card card3 = Instantiate(cardTemp, deckParent);
            Card card4 = Instantiate(cardTemp, deckParent);
            cardDeck.Add(card1);
            cardDeck.Add(card2);
            cardDeck.Add(card3);
            cardDeck.Add(card4);
        }
        ShuffleList(cardDeck);
        foreach (Card card in cardDeck)
        {
            shuffledCards.Add(card);
        }
        cardDeck.Clear();
        DrawCard(3, RoundManager.Instance.cat);
        DrawCard(3, RoundManager.Instance.bird,ANIMAL_COST_TYPE.BIRD);
        DrawCard(3, RoundManager.Instance.wood);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            DrawCard(1, RoundManager.Instance.wood, ANIMAL_COST_TYPE.FOX);
        }
    }
    private List<T> ShuffleList<T>(List<T> list)
    {
        int random1, random2;
        T temp;

        for (int i = 0; i < list.Count; ++i)
        {
            random1 = Random.Range(0, list.Count);
            random2 = Random.Range(0, list.Count);

            temp = list[random1];
            list[random1] = list[random2];
            list[random2] = temp;
        }

        return list;
    }
    public void DrawCard(int cardNum, Player getPlayer, ANIMAL_COST_TYPE type = ANIMAL_COST_TYPE.None)
    {
        for(int i= 0; i < cardNum; ++i)
        {
            if (shuffledCards.Count == 0)//뺼카드가 비었으면
            {
                ShuffleList(cardDeck);
                foreach (Card card in cardDeck)
                {
                    shuffledCards.Add(card);
                }
                cardDeck.Clear();
            }            
            if(type == ANIMAL_COST_TYPE.None)
            {
                Card tempCard = shuffledCards[shuffledCards.Count - 1];
                shuffledCards.RemoveAt(shuffledCards.Count - 1);
                getPlayer.AddCard(tempCard, tempCard.costType);
            }
            else
            {
                int tempNum = 1;
                bool foundCard = false;
                Card tempCard = null;
                while (foundCard == false)
                {
                    //Debug.Log(tempNum);
                    tempCard = shuffledCards[shuffledCards.Count - tempNum];
                    if (tempCard.costType == type || tempCard.costType == ANIMAL_COST_TYPE.BIRD)
                    {
                        //Debug.Log(tempCard.costType);
                        //Debug.Log(type);
                        getPlayer.AddCard(tempCard, tempCard.costType);
                        //cardDeck.Add(tempCard);
                        shuffledCards.RemoveAt(shuffledCards.Count - tempNum);
                        foundCard = true;
                    }
                    else
                    {
                        if(tempNum == shuffledCards.Count)//다찾았는데도 없으면
                        {                            
                            if(cardDeck.Count > 0)
                            {
                                foreach (Card card in cardDeck)
                                {
                                    shuffledCards.Add(card);
                                }
                                ShuffleList(shuffledCards);
                                tempNum = 0;
                            }
                            //이랬는데도 없으면 새로 생성해줘야할듯
                        }
                        tempNum++;
                    }
                        
                }
                //shuffledCards.Sort();
            }
            
        }        
    }
}