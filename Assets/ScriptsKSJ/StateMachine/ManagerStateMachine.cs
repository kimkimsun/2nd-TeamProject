using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInterface;

public class ManagerStateMahchine<T> : IStateMachine where T : Component
{
    public T manager = null;
    public MaState curState;
    public Dictionary<MASTATE_TYPE, MaState> stateDic = new Dictionary<MASTATE_TYPE, MaState>();

    public ManagerStateMahchine(T manager)
    {
        this.manager = manager;
    }

    public void AddStateDic(MASTATE_TYPE type, MaState state)
    {
        if (stateDic.ContainsKey(type) == true)
        {
            return;
        }
        stateDic.Add(type, state);
        state.Init(this);
    }
    public object GetOwner()
    {
        return manager;
    }
    public void SetState(MASTATE_TYPE type)
    {
        if (stateDic.ContainsKey(type) == true)
        {
            if (curState != null)
            {
                curState.Exit();
            }

            curState = stateDic[type];
            curState.Enter();
        }
    }
    public void Update()
    {
        curState?.Update();
    }
}

