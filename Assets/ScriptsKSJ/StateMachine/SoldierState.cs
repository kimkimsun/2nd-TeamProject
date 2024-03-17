using CustomInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SoldierState
{
    public SoldierStateMachine sm; //이 상태의 소유자
    public string testString;
    public SoldierState()
    {

    }
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
public class IdleState : SoldierState
{
    public IdleState()
    {
        testString = "Idle";
    }
    public override void Enter()
    {
        //Debug.Log(sm.curState.testString + "진입");
        sm.soldier.animator.SetFloat("speed", 0);
    }
    public override void Update()
    {
        if (sm.soldier.agentSpeed > 0)
        {
            //Debug.Log("진입할거야");
            sm.SetState(STATE_TYPE.MOVE);
        }
        
        //Debug.Log("대기상태");
    }
    public override void Exit()
    {
        //Debug.Log(sm.curState.testString + "나감");
    }
}
public class MoveState : SoldierState
{
    public MoveState()
    {
        testString = "Auto";
    }
    public override void Enter()
    {
        //Debug.Log(sm.curState.testString + "진입");
    }
    public override void Update()
    {
        //Debug.Log("추적중");
        if (sm.soldier.agentSpeed == 0)
        {
            sm.SetState(STATE_TYPE.IDLE);
        }
        sm.soldier.animator.SetFloat("speed", sm.soldier.agentSpeed);
    }
    public override void Exit()
    {
        //Debug.Log(sm.curState.testString + "나감");
        sm.soldier.agent.enabled = false;
    }
}