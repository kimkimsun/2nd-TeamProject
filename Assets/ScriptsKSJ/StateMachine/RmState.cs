using CustomInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RmState : MaState
{
    protected RoundManager rm;
    protected Cat cat;
    protected Bird bird;
    protected Wood wood;
    
    public RmState()
    {

    }
    public override void Enter()
    {
    }
    public override void Update()
    {
    }
    public override void Exit()
    {
    }
    public override void Init(IStateMachine sm)
    {
        this.sm = sm;
        rm = (RoundManager)sm.GetOwner();
        cat = rm.cat;
        bird = rm.bird;
        wood = rm.wood;
    }
}
