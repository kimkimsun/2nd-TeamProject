using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCardInven : MonoBehaviour
{
    public List<BattleCardSlot> slots = new List<BattleCardSlot>();
    public List<Card> battleCards = new List<Card>();
    private List<Card> defenseCards = new List<Card>();
    private void OnEnable()
    {
        battleCards = RoundManager.Instance.nowPlayer.craftedCards.FindAll(
            card => card.skillType == CustomInterface.CARD_SKILL_TYPE.BATTLE);
        defenseCards = RoundManager.Instance.nowPlayer.craftedCards.FindAll(
            card => card.skillType == CustomInterface.CARD_SKILL_TYPE.DEFENSE);
        foreach(Card tempCard in defenseCards)
        {
            battleCards.Add(tempCard);
        }
        for (int i = 0; i< battleCards.Count; i++)
        {
            slots[i].gameObject.SetActive(true);
            slots[i].SetCard(battleCards[i]);
        }
    }
    private void OnDisable()
    {
        foreach(BattleCardSlot slot in slots)
        {
            slot.card = null;
            slot.image.sprite = null;
            slot.gameObject.SetActive(false);
        }
    }
}
