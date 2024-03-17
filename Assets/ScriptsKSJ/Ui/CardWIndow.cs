using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardWIndow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    Transform targetTrans;
    [SerializeField]
    Transform targetTrans2;
    Coroutine move;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (move != null)
        {
            StopCoroutine(move);
        }
        move = StartCoroutine("MoveCo");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("나갔음");
        if(move != null)
        {
            StopCoroutine(move);
            move = null;
        }
        move = StartCoroutine("MoveCo2");
    }

    IEnumerator MoveCo()
    {
        while (targetTrans.position.y - Time.deltaTime * 8f > transform.position.y)
        {
            transform.position = Vector3.Lerp(transform.position, targetTrans.position, Time.deltaTime * 8f);
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator MoveCo2()
    {
        while (targetTrans2.position.y + Time.deltaTime * 8f < transform.position.y)
        {
            //Debug.Log("dd");
            transform.position = Vector3.Lerp(transform.position, targetTrans2.position, Time.deltaTime * 8f);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
