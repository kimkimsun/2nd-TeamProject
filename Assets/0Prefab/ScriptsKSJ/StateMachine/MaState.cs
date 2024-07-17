using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInterface;

public abstract class MaState
{
    public IStateMachine sm;
    public string name;
    public MaState()
    {

    }
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
    public abstract void Init(IStateMachine sm);
}