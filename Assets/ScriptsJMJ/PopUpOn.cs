using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopUpOn : MonoBehaviour, IPointerClickHandler
{
    public AudioClip clip;
    public GameObject popUp;
    public void OnPointerClick(PointerEventData eventData)
    {
        //SoundManager.instance.SFXPlay("Button", clip); SoundManager만들고 주석풀기
        SetOnPopUp(popUp);
    }

    public void SetOnPopUp(GameObject popUpName)
    {
        popUp.SetActive(true);
    }

    public void SetOffPopUp(GameObject popUp)
    {
        popUp.SetActive(false);
    }
}
