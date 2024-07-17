using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScoreMove : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    Transform targetTrans;
    [SerializeField]
    Transform targetTrans2;
    Coroutine move;
    [SerializeField]
    Transform moveTrans;
    Color color;
    private void Start()
    {
        color = transform.GetComponent<Image>().color;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (move != null)
        {
            StopCoroutine(move);
        }
        move = StartCoroutine("MoveCo");
        transform.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("나갔음");
        if (move != null)
        {
            StopCoroutine(move);
            move = null;
        }
        move = StartCoroutine("MoveCo2");
        transform.GetComponent<Image>().color = color;
    }

    IEnumerator MoveCo()
    {
        while (targetTrans.position.x - Time.deltaTime * 8f > moveTrans.position.x)
        {
            moveTrans.position = Vector3.Lerp(moveTrans.position, targetTrans.position, Time.deltaTime * 8f);
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator MoveCo2()
    {
        while (targetTrans2.position.x + Time.deltaTime * 8f < moveTrans.position.x)
        {
            //Debug.Log("dd");
            moveTrans.position = Vector3.Lerp(moveTrans.position, targetTrans2.position, Time.deltaTime * 8f);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
