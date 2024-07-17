using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleP2 : MonoBehaviour
{
    public BattleManager battleManager;
    public Image[] dices;
    public Image[] soldirs;
    public TextMeshProUGUI playerName;

    public void ActionP2()
    {
        playerName.text = battleManager.battleP2.name;
        for (int i = 0; i < battleManager.battleP2Soldiers.Count; i++)
        {
            soldirs[i].gameObject.SetActive(true);
        }
        StartCoroutine(RandomDice());
    }

    IEnumerator RandomDice()
    {
        yield return new WaitForSeconds(17);
        switch (battleManager.diceP1Num)
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

        int dieSoldir = battleManager.battleP2Soldiers.Count - battleManager.diceP2Num;
        for (int i = battleManager.battleP2Soldiers.Count; i == dieSoldir; i--)
        {
            soldirs[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < dices.Length; i++)
        {
            dices[i].gameObject.SetActive(false);
        }
    }

}
