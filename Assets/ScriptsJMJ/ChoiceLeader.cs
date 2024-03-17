using CustomInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChoiceLeader : MonoBehaviour
{
    public Button button;
    public Bird bird;
    public LEADER_TYPE leaders_type;

    private void Start()
    {
        button.onClick.AddListener(SetLeader);
        button.onClick.AddListener(() => { RoundManager.Instance.roundSM.SetState(MASTATE_TYPE.BIRD_MORNING); }) ;
    }
    void SetLeader()
    {
        bird.NowLeader = leaders_type;
        gameObject.SetActive(false);
        Uimanager.Instance.birdUI.birdLeaderSelect.SetActive(false);
        //Uimanager.Instance.birdUI.birdCardBox.SetActive(true);
    }
}
