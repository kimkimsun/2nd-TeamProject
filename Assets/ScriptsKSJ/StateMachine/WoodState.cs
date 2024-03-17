using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInterface;

public class WoodWaitState : RmState
{
    public WoodWaitState()
    {
        name = "우드대기";
    }
    public override void Enter()
    {
        Uimanager.Instance.playerUI.nextBtn.gameObject.SetActive(true);
        Uimanager.Instance.playerUI.catRecruitBtn.gameObject.SetActive(false);
        Uimanager.Instance.playerUI.catFieldHospitalBtn.gameObject.SetActive(false);
        Uimanager.Instance.playerUI.catExtraBtn.gameObject.SetActive(false);
        rm.SetOffAllEffect();
        Uimanager.Instance.woodUi.SetAgreeBtn();
        Uimanager.Instance.woodUi.SetRevoitBtn();
        Uimanager.Instance.woodUi.SetCraftBtn();
        Uimanager.Instance.woodUi.SetSupportBtn();
        Uimanager.Instance.woodUi.SetOfficerBtn();
        wood.BattleBtnsOnOff(true);

        Uimanager.Instance.playerUI.SetTurnTexts("우드랜드\n대기");
        Uimanager.Instance.playerUI.SetNextBtn(MASTATE_TYPE.WOOD_MORNING1);
        Uimanager.Instance.woodUi.profileWindow.SetActive(true);
    }
    public override void Update()
    {
        if (wood.isOver == false)
        {
        }
    }
    public override void Exit()
    {
        rm.nowPlayer = wood;
        Uimanager.Instance.playerUI.SpawnSoldier();
        BattleManager.Instance.InitBattle();
    }
}
public class WoodMorning1State : RmState
{
    public WoodMorning1State()
    {
        name = "우드아침";
    }
    public override void Enter()
    {
        rm.SetOffAllEffect();
        Uimanager.Instance.playerUI.SetTurnTexts("우드랜드 \n아침1");
        Uimanager.Instance.playerUI.SetNextBtn(MASTATE_TYPE.WOOD_MORNING2);
        Uimanager.Instance.woodUi.SetWoodMorning1();
        Uimanager.Instance.woodInven.SetActive(true);
    }
    public override void Update()
    {

    }
    public override void Exit()
    {

    }
}
public class WoodMorning2State : RmState
{
    public WoodMorning2State()
    {
        name = "우드아침";
    }
    public override void Enter()
    {
        rm.SetOffAllEffect();
        Uimanager.Instance.playerUI.SetTurnTexts("우드랜드 \n아침2");
        Uimanager.Instance.playerUI.SetNextBtn(MASTATE_TYPE.WOOD_AFTERNOON);
        Uimanager.Instance.woodUi.SetWoodMorning2();
    }
    public override void Update()
    {

    }
    public override void Exit()
    {

    }
}
public class WoodAfternoonState : RmState
{
    public WoodAfternoonState()
    {
        name = "우드점심";
    }
    public override void Enter()
    {
        rm.SetOffAllEffect();
        wood.SetOfficerBtnOnoff();
        Uimanager.Instance.playerUI.SetTurnTexts("우드랜드 \n점심");
        Uimanager.Instance.playerUI.SetNextBtn(MASTATE_TYPE.WOOD_DINNER);
        Uimanager.Instance.woodUi.SetAfternoon(true);
    }
    public override void Update()
    {

    }
    public override void Exit()
    {
        Uimanager.Instance.woodUi.SetAfternoon(false);
    }
}
public class WoodDinnerState : RmState
{
    public WoodDinnerState()
    {
        name = "우드저녁";
    }
    public override void Enter()
    {
        wood.BattleActionNum = 0;
        rm.SetOffAllEffect();
        Uimanager.Instance.playerUI.SetTurnTexts("우드랜드 \n저녁");
        Uimanager.Instance.playerUI.SetNextBtn(MASTATE_TYPE.CAT_MORNING);
        Uimanager.Instance.woodUi.SetDinner(true);
        //Uimanager.Instance.playerUI.ResetBtn(true);
    }
    public override void Update()
    {

    }
    public override void Exit()
    {
        wood.DrawCard();
        wood.BattleActionNum = 0;
        RoundManager.Instance.wood.isOver = true;
        RoundManager.Instance.cat.isOver = false;
        rm.nowPlayer = null;
        Uimanager.Instance.woodUi.SetDinner(false);
        //Uimanager.Instance.playerUI.ResetBtn(false);
        Uimanager.Instance.woodUi.profileWindow.SetActive(false);
        Uimanager.Instance.woodInven.SetActive(false);
    }
}