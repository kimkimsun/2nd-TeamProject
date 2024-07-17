using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.GridLayoutGroup;


public enum RULE_TYPE
{
    MUST_SPAWN,
    MUST_MOVE,
    MUST_BATTLE,
    MUST_BULID
    //2개의 규율은 무조건 수행
}



public class Discipline : MonoBehaviour
{
    public Bird bird;
    public Func<bool> IsBreakRule;
    public UnityEvent OnBreakingRule;
    public RULE_TYPE ruleType;
}