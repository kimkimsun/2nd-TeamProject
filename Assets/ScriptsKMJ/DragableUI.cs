using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform canvas;

    private Transform previousParent;
    private RectTransform rectTrans;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        previousParent = transform.parent;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTrans.position = eventData.position;
        canvasGroup.alpha = 0.2f;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("a");
        rectTrans.position = previousParent.GetComponent<RectTransform>().position;
        canvasGroup.alpha = 1f;
    }
}
