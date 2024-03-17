using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Sequence : MonoBehaviour
{
    public Image[] pointImage;

    Color color;
    Color originColor;
    IEnumerator pointCo;
    bool isCheck;

    private void Start()
    {
        originColor = new Color(1f, 0.96f, 0f, 1f);
        color = new Color(1f, 1f, 1f, 0f);
        for (int j = 0; j < 4; j++)
        {
            pointImage[j].color = originColor;
        }
    }
    public void CoroutineMethod(int i)
    {
        for(int j  = 0; j < 4; j++)
        {
            pointImage[j].color = originColor;
        }
        if (pointCo != null)
            StopCoroutine(pointCo);
        pointCo = PointCo(i);
            StartCoroutine(pointCo);
    }
    public IEnumerator PointCo(int i)
    {
        isCheck = RoundManager.Instance.nowPlayer == RoundManager.Instance.bird;
        while (isCheck)
        {
            pointImage[i].GetComponent<Image>().color = originColor;
            yield return new WaitForSeconds(1);
            pointImage[i].GetComponent<Image>().color = color;
            yield return new WaitForSeconds(1);
        }
    }

}