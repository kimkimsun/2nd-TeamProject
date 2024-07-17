using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleScene : MonoBehaviour
{
    private bool isMove = true;
    private IEnumerator saveCameraCo;
    
    public CinemachineVirtualCamera[] cinemachines = new CinemachineVirtualCamera[6];
    public Canvas uiCanvas1;
    public Canvas uiCanvas2;
    public Canvas uiCanvas3;
    public Image battleUI;
    public Image battleCardImage;
    public BattleP1 battlep1;
    public BattleP2 battlep2;
    public CinemachineVirtualCamera cinemachineCamera;
    public float player1StopNum = 0;

    private void Awake()
    {
        saveCameraCo = CameraCo();
    }

    public void StartBattle()
    {
        player1StopNum = 0;
        switch (BattleManager.Instance.battleP1.name)
        {
            case "Cat":
                transform.GetChild(0).gameObject.SetActive(true);
                break;
            case "Bird":
                transform.GetChild(1).gameObject.SetActive(true);
                break;
            case "Wood":
                transform.GetChild(2).gameObject.SetActive(true);
                break;
        }
        switch (BattleManager.Instance.battleP2.name)
        {
            case "Cat":
                transform.GetChild(3).gameObject.SetActive(true);
                break;
            case "Bird":
                transform.GetChild(4).gameObject.SetActive(true);
                break;
            case "Wood":
                transform.GetChild(5).gameObject.SetActive(true);
                break;
        }
        StartCoroutine(CameraCo());
        LookAtCamera();
    }
    private void LookAtCamera()
    {
        for(int i = 0; i < 3; i++)
        {
            if(transform.GetChild(i).gameObject.activeSelf == true)
            {
                cinemachines[0].LookAt = transform.GetChild(i).gameObject.transform;
                cinemachines[1].LookAt = transform.GetChild(i).gameObject.transform;
                cinemachines[2].LookAt = transform.GetChild(i).gameObject.transform;
            }
        }
        for(int j = 3; j < 6; j++)
        {
            if (transform.GetChild(j).gameObject.activeSelf == true)
            {
                cinemachines[3].LookAt = transform.GetChild(j).gameObject.transform;
                cinemachines[4].LookAt = transform.GetChild(j).gameObject.transform;
                cinemachines[5].LookAt = transform.GetChild(j).gameObject.transform;
            }
        }
    }
    
    public void UION()
    {
        Debug.Log("제발 들어와라 예");
        cinemachineCamera.Priority = 11;
        uiCanvas1.gameObject.SetActive(true);
        uiCanvas2.gameObject.SetActive(true);
        uiCanvas3.gameObject.SetActive(false);
        battleCardImage.gameObject.SetActive(false);
        battleUI.gameObject.SetActive(false);
        for(int i = 0;  i < 6; i++) 
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        for(int i = 0; i < 3; i++)
        {
            battlep1.dices[i].gameObject.SetActive(false);
            battlep2.dices[i].gameObject.SetActive(false);
        }
    }
    IEnumerator CameraCo()
    {
        while(player1StopNum < 19.5f)
        {
            player1StopNum += Time.deltaTime;
            Debug.Log(player1StopNum);
            yield return null;
        }
        UION();
        player1StopNum = 0;
    }
}