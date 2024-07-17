using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ScenesMove : MonoBehaviour, IPointerClickHandler
{
    public AudioClip clip;
    public string scenesname;
    public void OnPointerClick(PointerEventData eventData)
    {
        //SoundManager.instance.SFXPlay("Button", clip);
        GameScenes(scenesname);
    }

    public void GameScenes(string scenesname)
    {
        SceneManager.LoadScene(scenesname);
    }

}

