using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alarm : MonoBehaviour
{
    Coroutine tempCo;
    public Image image1;
    public Image image2;

    private void OnEnable()
    {
        tempCo = StartCoroutine("AlarmCo");
    }
    IEnumerator AlarmCo()
    {
        Image image = transform.GetComponent<Image>();
        while(image.color.a > 0)
        {
            Color color = image.color;
            color.a -= Time.deltaTime/2.5f;
            image.color = color;
            image1.color = color;
            image2.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
        Color tempcolor = new Color(image.color.r, image.color.g, image.color.b, 1);
        image.color = tempcolor;
        image1.color = tempcolor;
        image2.color = tempcolor;
        yield return null;
    }
}
