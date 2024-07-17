using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropableUI : MonoBehaviour, IDropHandler
{
    private PointerEventData ped = new PointerEventData(null);
    private GraphicRaycaster gr = null;
    private List<RaycastResult> results;

    public Image cardInfoWindow;
    public bool isMove = false;

    private void Start()
    {
        gr = GetComponent<GraphicRaycaster>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        results = new List<RaycastResult>();
        ped.position = Input.mousePosition;
        gr.Raycast(ped, results);
        if (results.Count <= 0 || results[1].gameObject == null)
            return;
        if (results[1].gameObject.name == "Frame")
            isMove = true;
        else
            isMove = false;
        if (eventData.pointerDrag != null)
        {
            if (isMove)
            {
                if (RoundManager.Instance.bird.inputCard < 2)
                {
                    results[1].gameObject.transform.GetChild(0).GetComponent<BirdCardAction>().
                    AddCard(eventData.pointerDrag.GetComponentInParent<Slot>().card);
                    eventData.pointerDrag.GetComponentInParent<Slot>().UseCard();
                }
                else
                    return;
            }
            else if (Uimanager.Instance.woodUi.cardUseType != WoodUi.CardUseType.BATTLE)
            {
                eventData.pointerDrag.GetComponentInParent<Slot>().UseCard();
            }
            cardInfoWindow.gameObject.SetActive(false);
        }
    }
}
