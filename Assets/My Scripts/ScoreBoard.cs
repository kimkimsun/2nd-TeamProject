using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    public Player bird;
    public Player wood;
    public Player cat;
    public GameObject[] scoreOBJs = new GameObject[30];



    Dictionary<Player, int> playerDic = new Dictionary<Player, int>();
    int score = 1;
    int catCode = 0;
    int birdCode = 1;
    int woodCode = 2;


    private void Start()
    {
        playerDic.Add(cat, catCode);
        playerDic.Add(bird, birdCode);
        playerDic.Add(wood, woodCode);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetScore(score, catCode);
            score++;
        }
    }

    public void SetScore(int score, int playerCode)
    {
        for (int i = 0; i <= scoreOBJs.Length - 1; i++)
        {
            transform.GetChild(i).GetChild(playerCode).gameObject.SetActive(false);
        }
        if(score > 0)
        {
            transform.GetChild(scoreOBJs.Length - score).GetChild(playerCode).gameObject.SetActive(true);
        }   

    }
}