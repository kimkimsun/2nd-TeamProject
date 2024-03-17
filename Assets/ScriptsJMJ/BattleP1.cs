using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleP1 : MonoBehaviour
{
    public BattleManager battleManager;
    public Image[] dices;
    public Image[] soldirs;
    public TextMeshProUGUI playerName;

    
    public void ActionP1()
    {
        playerName.text = battleManager.battleP1.name;
        for (int i = 0; i < battleManager.battleP1.battleSoldierNum; i++)
        {
            soldirs[soldirs.Length - (1 + i)].gameObject.SetActive(true);
        }
        StartCoroutine(RandomDice());
    }

    IEnumerator RandomDice()
    {
        yield return new WaitForSeconds(17);
        switch (battleManager.diceP2Num)
        {
            case 0:
                dices[0].gameObject.SetActive(true);
                break;

            case 1:
                dices[1].gameObject.SetActive(true);
                break;

            case 2:
                dices[2].gameObject.SetActive(true);
                break;

            case 3:
                dices[3].gameObject.SetActive(true);
                break;
        }

        yield return new WaitForSeconds(3);

        int dieSoldir = battleManager.battleP1Soldiers.Count - battleManager.diceP1Num;
        for (int a = battleManager.battleP1Soldiers.Count; a == dieSoldir; a--)
        {
            soldirs[a].gameObject.SetActive(false);
        }

        for (int i = 0; i < dices.Length; i++)
        {
            dices[i].gameObject.SetActive(false);
        }

        for(int j = 0; j < soldirs.Length; j++)
        {
            soldirs[j].gameObject.SetActive(false);
        }
    }

}
