using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


    public class SoldierChoice : MonoBehaviour
    {

        public Button plusButton;
        public Button minusButton;
        public TextMeshProUGUI curSol;
        public TextMeshProUGUI maxSol;

        public int maxSoldier;
        int curSoldier;

    int CurSoldier
    {
        get { return curSoldier; }
        set
        {
            curSoldier = value;
            if (curSoldier <= 0)
                curSoldier = 0;
            if (curSoldier > maxSoldier)
                curSoldier = maxSoldier;
            curSol.text = CurSoldier.ToString();
            RoundManager.Instance.mapController.soldierNum = curSoldier;
        }
    }

    private void Start()
        {
            curSoldier = 0;
            plusButton.onClick.AddListener(() => { CurSoldier++; });
            minusButton.onClick.AddListener(() => { CurSoldier--; });
            curSol.text = CurSoldier.ToString();
            
        }


    }



