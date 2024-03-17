using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleCardSlot : MonoBehaviour, IPointerDownHandler
{
    public Card card;
    public Image image;

    public void OnPointerDown(PointerEventData eventData)
    {
        UseCard(card);
    }
    public void SetCard(Card card)
    {
        this.card = card;
        image.sprite = card.sprite;
    }
    public void UseCard(Card card)
    {
        card.onActive = null;
        card.Active();
        //배틀 카드 인벤끄기
        BattleManager.Instance.StartBattle();
        Uimanager.Instance.playerUI.battleCardsWindow.SetActive(false);
    }

}
