using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInterface;
using UnityEngine.SocialPlatforms.Impl;

public class BirdWaitState : RmState
{
    public BirdWaitState()
    {
        name = "새대기";
    }
    public override void Enter()
    {
        Uimanager.Instance.playerUI.SetNextBtn(MASTATE_TYPE.BIRD_MORNING);
        Uimanager.Instance.playerUI.spawnBtn.gameObject.SetActive(false);
        Uimanager.Instance.playerUI.battleBtn.gameObject.SetActive(false);
        Uimanager.Instance.playerUI.buildBtn.gameObject.SetActive(false);
        Uimanager.Instance.playerUI.moveBtn.gameObject.SetActive(false);
        Uimanager.Instance.playerUI.nextBtn.gameObject.SetActive(false);
        Uimanager.Instance.playerUI.catExtraBtn.gameObject.SetActive(false);
        Uimanager.Instance.playerUI.catRecruitBtn.gameObject.SetActive(false);
        Uimanager.Instance.playerUI.catFieldHospitalBtn.gameObject.SetActive(false);
        Uimanager.Instance.birdUI.birdInfo.SetActive(true);
        Uimanager.Instance.birdUI.curCard.text = RoundManager.Instance.bird.DrawCardNum.ToString();
        int tempNum = 0;
        foreach (var tempDic in RoundManager.Instance.bird.hasSoldierDic)
        {
            tempNum += tempDic.Value.Count;
        }
        Uimanager.Instance.birdUI.solCount.text = tempNum.ToString();
        Uimanager.Instance.birdUI.scoreUp.text = tempNum.ToString();

        rm.SetOffAllEffect();
        rm.nowPlayer = bird;
        Uimanager.Instance.playerUI.SetTurnTexts("이어리 왕조 \n대기");
        if (RoundManager.Instance.bird.NowLeader == LEADER_TYPE.NONE)
            Uimanager.Instance.birdUI.birdLeaderSelect.SetActive(true);
        else
        {
            Uimanager.Instance.birdUI.birdAlarm.SetActive(true);
            Uimanager.Instance.birdUI.alarmText.text = "할 수 있는 행동이 없습니다.";
            Uimanager.Instance.birdUI.birdLeaderSelect.SetActive(false);
            Uimanager.Instance.playerUI.nextBtn.gameObject.SetActive(true);
        }
    }
    public override void Update()
    {

    }
    public override void Exit()
    {
    }
}
public class BirdMorningState : RmState
{
    public BirdMorningState()
    {
        name = "새아침";
    }
    public override void Enter()
    {
        RoundManager.Instance.bird.inputCard = 0;
        Uimanager.Instance.playerUI.nextBtn.gameObject.SetActive(true);
        Uimanager.Instance.playerUI.AlarmWindow.SetActive(true);
        Uimanager.Instance.playerUI.turnAlarmText.text = "규율 제정 턴";
        Uimanager.Instance.woodUi.cardUseType = WoodUi.CardUseType.CRAFT;
        Uimanager.Instance.playerUI.SetTurnTexts("이어리 왕조 \n아침");
        Uimanager.Instance.playerUI.SetNextBtn(MASTATE_TYPE.BIRD_MORNING2);
        Uimanager.Instance.birdInven.SetActive(true);
    }
    public override void Update()
    {
    }
    public override void Exit()
    {
        Uimanager.Instance.playerUI.nextBtn.gameObject.SetActive(false);
        Uimanager.Instance.playerUI.SpawnSoldier();
    }
}
public class BirdMorning2State : RmState
{
    public BirdMorning2State()
    {
        name = "새아침2";
    }
    public override void Enter()
    {
        Uimanager.Instance.playerUI.AlarmWindow.SetActive(true);
        Uimanager.Instance.birdUI.birdCardBox.SetActive(true);
        Uimanager.Instance.playerUI.turnAlarmText.text = "카드 제작 턴";
        Uimanager.Instance.woodUi.cardUseType = WoodUi.CardUseType.BIRDUSE;
        Uimanager.Instance.playerUI.SetTurnTexts("이어리 왕조 \n아침2");
        Uimanager.Instance.playerUI.SetNextBtn(MASTATE_TYPE.BIRD_AFTERNOON);
    }
    public override void Update()
    {
    }
    public override void Exit()
    {
        Uimanager.Instance.woodUi.cardUseType = WoodUi.CardUseType.NONE;
    }
}
public class BirdAfternoonState : RmState
{
    public BirdAfternoonState()
    {
        name = "새점심";
    }
    public override void Enter()
    {
        Uimanager.Instance.playerUI.SetTurnTexts("이어리 왕조 \n점심");
        Uimanager.Instance.birdUI.SequenceBox.SetActive(true);
        Uimanager.Instance.birdUI.BirdInventory.UseSlot();
        Uimanager.Instance.playerUI.ResetBtn(true);
        Uimanager.Instance.playerUI.SetNextBtn(MASTATE_TYPE.BIRD_DINNER);
        BattleManager.Instance.InitBattle();
    }
    public override void Update()
    {

    }
    public override void Exit()
    {
        Uimanager.Instance.playerUI.nextBtn.gameObject.SetActive(false);
        Uimanager.Instance.birdUI.SequenceBox.SetActive(false);
        Uimanager.Instance.playerUI.ResetBtn(false);
    }
}
public class BirdDinnerState : RmState
{
    public BirdDinnerState()
    {
        name = "새저녁";
    }
    public override void Enter()
    {
        //CardManager.Instance.DrawCard(bird.getCards, bird);
        bird.DrawCard();
        Uimanager.Instance.playerUI.nextBtn.gameObject.SetActive(true);
        Uimanager.Instance.birdUI.birdInfo.SetActive(false);
        foreach (BirdCardAction temp in Uimanager.Instance.birdUI.BirdInventory.birdCardSlot)
        {
            for (int i = 0; i < temp.isOver.Count; i++)
            {
                {
                    temp.isOver[i] = false;
                }
            }
        }
        Uimanager.Instance.birdUI.scoreBord.SetActive(true);
        Uimanager.Instance.birdUI.inputCardCount.text = RoundManager.Instance.bird.DrawCardNum.ToString();
        int tempNum = 0;
        foreach (var tempDic in RoundManager.Instance.bird.hasBuildingDic)
        {
            tempNum += tempDic.Value.Count;
        }

        Uimanager.Instance.birdUI.scoreUp.text = tempNum.ToString();
        bird.Score += bird.hasBuildingDic.Count - 1;
        Debug.Log(bird.Score);
        Uimanager.Instance.playerUI.SetTurnTexts("이어리 왕조 \n저녁");
        Uimanager.Instance.playerUI.SetNextBtn(MASTATE_TYPE.WOOD_WAIT);
    }
    public override void Update()
    {

    }
    public override void Exit()
    {

        Uimanager.Instance.birdUI.BirdInventory.firstSlotCheck = false;
        bird.isOver = true;
        wood.isOver = false;
        rm.nowPlayer = null;
        Uimanager.Instance.birdInven.SetActive(false);
        Uimanager.Instance.playerUI.nextBtn.gameObject.SetActive(true);
        Uimanager.Instance.playerUI.catExtraBtn.gameObject.SetActive(true);
        Uimanager.Instance.playerUI.catRecruitBtn.gameObject.SetActive(true);
        Uimanager.Instance.playerUI.catFieldHospitalBtn.gameObject.SetActive(true);
        Uimanager.Instance.birdUI.scoreBord.SetActive(false);
    }

}