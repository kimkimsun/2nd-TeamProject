using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    public Player player;
    private new void Awake()
    {
        base.Awake();
    }
}