using CustomInterface;
using JetBrains.Annotations;
using sihyeon;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cat : Player
{
    public bool isInit;
    public int actionPoint;
    public bool firstMove = false;
    public bool secondMove = false;
    public bool isSpawn = false;
    private int MaxActionPoint = 4;
    public bool isWorked = false;

    public int catSawMillCost = 1;
    public int catBarrackCost = 1;
    public int catWorkShopCost = 1;



    // 각 행동은 1의 액션포인트 소모 기본적으로 매턴 2를 가지고 들어간다
    // 예외적으로 카드 하나를 소모후 매고용으로 1포인트 더 
    public NodeMember baseNode;
    public int turnAddWoodToken = 0;

    [SerializeField]
    public int woodProductNum;
    //public int WoodProductNum
    //{
    //    get { return remainSoldierNum; }
    //    set
    //    {
    //        woodProductNum = value;
    //        Uimanager.Instance.catUI.woodProductText.text = woodProductNum.ToString();
    //    }
    //}

    public int soldierMaxNum;//병사 최대 명수
    private int remainSoldierNum;
    public int RemainSoldierNum
    {
        get { return remainSoldierNum; }
        set
        {
            remainSoldierNum = value;
            Uimanager.Instance.catUI.remainSolText.text = remainSoldierNum.ToString();
        }
    }

    private int remainSawmillNum;
    public int RemainSawmillNum
    {
        get { return remainSawmillNum; }
        set
        {
            remainSawmillNum = value;
            Uimanager.Instance.catUI.remainSawmillsText.text = remainSawmillNum.ToString();
        }
    }

    private int remainworkshopsNum;
    public int RemainWorkshopsNum
    {
        get { return remainworkshopsNum; }
        set
        {
            remainworkshopsNum = value;
            Uimanager.Instance.catUI.remainWorkshopsText.text = remainworkshopsNum.ToString();
        }
    }
    private int remainbarracksNum;
    public int RemainBarracksNum
    {
        get { return remainbarracksNum; }
        set
        {
            remainbarracksNum = value;
            Uimanager.Instance.catUI.remainBarracksText.text = remainbarracksNum.ToString();
        }
    }

    public Dictionary<ANIMAL_COST_TYPE, int> deadSoldierNum = new Dictionary<ANIMAL_COST_TYPE, int>();
    public bool isDisposable = true;
    IEnumerator flashCo;
    Color originColor1;
    Color originColor2;
    Color originColor3;
    Color originColor4;

    private new void Start()
    {
        base.Start();
        isInit = false;
        //Debug.Log(animator.GetLayerIndex("Idle"));
        isOver = false;
        roundManager.cat = this;
        roundManager.nowPlayer = this;
        //hasNodeNames.Add("생쥐3");
        ColorSetting();
        flashCo = FlashCoroutine();

        woodProductNum = 8;

        remainSawmillNum = 6;
        remainworkshopsNum = 6;
        remainbarracksNum = 6;

        soldierMaxNum = 25;
        remainSoldierNum = soldierMaxNum;

        RoundManager.Instance.mapController.catOnAction += () => { UseActionPoint(); };
        RoundManager.Instance.mapController.catEmploy += () => { Employment(); };


        actionPoint = 3;

    }

    private void ColorSetting()
    {
        originColor1 = RoundManager.Instance.mapExtra.mapTiles[0].transform.GetComponent<Renderer>().material.color;
        originColor2 = RoundManager.Instance.mapExtra.mapTiles[2].transform.GetComponent<Renderer>().material.color;
        originColor3 = RoundManager.Instance.mapExtra.mapTiles[8].transform.GetComponent<Renderer>().material.color;
        originColor4 = RoundManager.Instance.mapExtra.mapTiles[11].transform.GetComponent<Renderer>().material.color;
    }
    public void FlashTile()
    {
        StartCoroutine(flashCo);
    }
    IEnumerator FlashCoroutine()
    {
        while (RoundManager.Instance.cat.isDisposable)
        {
            RoundManager.Instance.mapExtra.mapTiles[0].transform.GetComponent<Renderer>().material.color = Color.green;
            RoundManager.Instance.mapExtra.mapTiles[2].transform.GetComponent<Renderer>().material.color = Color.green;
            RoundManager.Instance.mapExtra.mapTiles[8].transform.GetComponent<Renderer>().material.color = Color.green;
            RoundManager.Instance.mapExtra.mapTiles[11].transform.GetComponent<Renderer>().material.color = Color.green;
            yield return new WaitForSeconds(1f);
            RoundManager.Instance.mapExtra.mapTiles[0].transform.GetComponent<Renderer>().material.color = originColor1;
            RoundManager.Instance.mapExtra.mapTiles[2].transform.GetComponent<Renderer>().material.color = originColor2;
            RoundManager.Instance.mapExtra.mapTiles[8].transform.GetComponent<Renderer>().material.color = originColor3;
            RoundManager.Instance.mapExtra.mapTiles[11].transform.GetComponent<Renderer>().material.color = originColor4;
            yield return new WaitForSeconds(1f);
            yield return null;
        }
    }
    public override GameObject SpawnSoldier(string tileName, Transform targetTransform)
    {
        Vector3 tempVec = Vector3.zero;
        if (hasSoldierDic.ContainsKey(tileName))//병사가 존재하는지 체크
        {
            tempVec = new Vector3(hasSoldierDic[tileName].Count, 0, 0);//명수에 따라 소환하는 위치를 바꿔야해서
        }
        GameObject addedSoldier = Instantiate(prefabSoldier, targetTransform.position + tempVec, Quaternion.identity);
        //더해줄 병사를 임의로 저장해주고
        SetHasNode(tileName, addedSoldier.GetComponent<Soldier>());//그타일에 방금 만든 병사를 저장해줌.

        RemainSoldierNum--;
        return addedSoldier;//생성한 병사를 return시킴
    }
    public override void SpawnBuilding(string tileName, Transform targetTransform, GameObject building)
    {
        Building newBuilding = building.GetComponent<Building>();
        if (hasBuildingDic.ContainsKey(tileName) == false)
        {
            hasBuildingDic.Add(tileName, new List<GameObject>());
        }
        if (!hasBuildingDic[tileName].Exists(temp => temp.GetComponent<Building>().type == newBuilding.type))
        {
            SetHasBuildingNode(tileName, targetTransform, building); // 리스트에 넣고


            if (newBuilding.type == Building_TYPE.CAT_SAWMILL)
            {
                turnAddWoodToken++;
                newBuilding.onDestroy += () => { turnAddWoodToken--; };
                Debug.Log(newBuilding.type);
            }            
        }
        else
        {
            Debug.Log("이미 건설됨");
        }
    }

    public List<NodeMember> RuleTile() //타일 판별해서 리턴해주기.
    {
        List<NodeMember> ruleNodeMemberList = new List<NodeMember>();

        foreach (var keyValue in RoundManager.Instance.cat.hasSoldierDic)
        {
            Debug.Log(keyValue.Value.Count);
            if(keyValue.Value.Count > 0)
            {
                bool isCatRule = true;
                int curNodeCatCount = keyValue.Value.Count;
                int curNodeWoodCount = 0;
                int curNodeBirdCount = 0;

                if (RoundManager.Instance.wood.hasSoldierDic.ContainsKey(keyValue.Key))
                    curNodeWoodCount = RoundManager.Instance.wood.hasSoldierDic[keyValue.Key].Count;
                if (RoundManager.Instance.bird.hasSoldierDic.ContainsKey(keyValue.Key))
                    curNodeBirdCount = RoundManager.Instance.bird.hasSoldierDic[keyValue.Key].Count;
                isCatRule &= (curNodeCatCount > curNodeWoodCount);
                isCatRule &= (curNodeCatCount > curNodeBirdCount);
                if (isCatRule)
                {
                    Debug.Log(keyValue.Key);
                    NodeMember nm = RoundManager.Instance.mapExtra.mapTiles.Find(node => node.nodeName == keyValue.Key);
                    Debug.Log(nm.nodeName);
                    ruleNodeMemberList.Add(nm);
                }
            }            
        }
        return ruleNodeMemberList;
    }
    public void SetDrawNumFunc()
    {
        int tempNum = 0;
        foreach (var tempDic in hasBuildingDic)
        {
            List<GameObject> tempList = tempDic.Value.FindAll(gobj => gobj.GetComponent<Building>().type == Building_TYPE.CAT_WORKSHOP);
            tempNum += tempList.Count;
        }
        if (tempNum >= 4)
        {
            DrawCardNum = 3;
        }
        else if (tempNum >= 2)
        {
            DrawCardNum = 2;
        }
        else
        {
            DrawCardNum = 1;
        }
    }
    public void costBuilding()// 코스트 설정해주기 건물에 , 건물에 어쩌구저쩌구 추가
    {
        int costSaw = 0;
        int costBarrack = 0;
        int costWork = 0;
        foreach (KeyValuePair<string, List<GameObject>> kv in RoundManager.Instance.nowPlayer.hasBuildingDic)
        {
            for (int i = 0; i < kv.Value.Count; i++)
            {

                if (kv.Value[i].GetComponent<Building>().type == Building_TYPE.CAT_BARRACKS)
                {
                    costSaw++;
                }
                if (kv.Value[i].GetComponent<Building>().type == Building_TYPE.CAT_WORKSHOP)
                {
                    costBarrack++;
                }
                if (kv.Value[i].GetComponent<Building>().type == Building_TYPE.CAT_SAWMILL)
                {
                    costWork++;
                }
            }
        }
        if (costSaw == 2)
        {
            RoundManager.Instance.cat.catSawMillCost = 2;
        }
        if (costSaw == 3 || costSaw == 4)
        {
            RoundManager.Instance.cat.catSawMillCost = 3;
        }
        if (costSaw == 4)
        {
            RoundManager.Instance.cat.catSawMillCost = 5;
        }
        if (costWork == 2)
        {
            RoundManager.Instance.cat.catWorkShopCost = 2;
        }
        if (costWork == 3 || costWork == 4)
        {
            RoundManager.Instance.cat.catWorkShopCost = 3;
        }
        if (costWork == 4)
        {
            RoundManager.Instance.cat.catWorkShopCost = 5;
        }
        if (costBarrack == 2)
        {
            RoundManager.Instance.cat.catBarrackCost = 2;
        }
        if (costBarrack == 3 || costBarrack == 4)
        {
            RoundManager.Instance.cat.catBarrackCost = 3;
        }
        if (costBarrack == 4)
        {
            RoundManager.Instance.cat.catBarrackCost = 5;
        }      
        
    }
    public void UseActionPoint()
    {
        if (actionPoint > 0)
        {
            actionPoint--;
            Uimanager.Instance.catUI.actionNumText.text = RoundManager.Instance.cat.actionPoint.ToString();
            Debug.Log("남은 포인트" + actionPoint);

        }
        else
        {
            Debug.Log("포인트없음");
            Debug.Log("남은 포인트" + actionPoint);

        }
    }

    public void Employment() // 매 고용
    {
        if (RoundManager.Instance.cat.actionPoint <= 3)
        {
            RoundManager.Instance.cat.actionPoint++;
            Uimanager.Instance.catUI.actionNumText.text = RoundManager.Instance.cat.actionPoint.ToString();
            Debug.Log("남은 포인트 : " + actionPoint);
        }
        else
        {
            Debug.Log("매고용못함");
            Debug.Log("남은 포인트 : " + actionPoint);
        }
    }
    public void FieldHospital(Card card)
    {
        for (int i = 0; i < deadSoldierNum[card.costType]; i++)
        {
            SpawnSoldier(baseNode.nodeName, baseNode.transform);
        }
        deadSoldierNum[card.costType] = 0;
    }


   public void GetScore()
   {
        int sawmillScore =0;
        int barrackScore = 0;
        int workShopScore= 0;
       if(remainSawmillNum == 4)
        {
            sawmillScore = 1;
        }
       if(remainSawmillNum ==3)
        {
            sawmillScore= 2;
        }
       if(remainSawmillNum==2)
        {
            sawmillScore = 3;
        }
       if(remainSawmillNum==1)
        {
            sawmillScore = 4;
        }
       if( remainSawmillNum==0)
        {
            sawmillScore = 5;
        }

       if(remainbarracksNum == 4)
        {
            barrackScore = 2;
        }
       if(remainbarracksNum == 3)
        {
            barrackScore = 2;
        }
       if(remainbarracksNum==2)
        {
            barrackScore = 3;
        }
       if(remainbarracksNum == 1)
        {
            barrackScore = 4;
        }
       if(remainbarracksNum == 0)
        {
            barrackScore = 5;
        }

       if(remainworkshopsNum == 4)
        {
            workShopScore = 1;
        }
       if (remainworkshopsNum == 3)
        {
            workShopScore= 2;
        }
       if(remainworkshopsNum==2)
        {
            workShopScore= 3;
        }
       if(remainworkshopsNum==1)
        {
            workShopScore = 3;
        }
       if(remainworkshopsNum ==0)
        {
            workShopScore= 4;
        }

        RoundManager.Instance.cat.Score += workShopScore + sawmillScore + barrackScore;
   }
}