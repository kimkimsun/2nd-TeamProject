using CustomInterface;
using sihyeon;
using System.Collections.Generic;
using UnityEngine;

public enum LEADER_TYPE
{
    NONE,
    ARCHITECT,
    PROPHET,
    COMMANDER,
    TYRANT
}
public class Bird : Player
{
    [SerializeField]
    private LEADER_TYPE nowLeader;
    private Transform particlesParent;
    
    public Card birdCard;
    public BirdCardInventory inventory;
    public Building[] birdBuilding;

    public int getCards = 1;
    public int inputCard = 0;
    int spawn = 0;
    int move = 1;
    int battle = 2;
    int build = 3;

    public bool isFirstCheck;
    public bool isDelete;
    public LEADER_TYPE NowLeader
    {
        get => nowLeader;

        set
        {
            nowLeader = value;
            switch (nowLeader)
            {
                case LEADER_TYPE.NONE:
                    break;
                case LEADER_TYPE.ARCHITECT:
                    inventory.birdCardSlot[spawn].AddBirdCard(birdCard);
                    inventory.birdCardSlot[move].AddBirdCard(birdCard);
                    break;
                case LEADER_TYPE.PROPHET:
                    inventory.birdCardSlot[battle].AddBirdCard(birdCard);
                    inventory.birdCardSlot[spawn].AddBirdCard(birdCard);
                    break;
                case LEADER_TYPE.COMMANDER:
                    inventory.birdCardSlot[move].AddBirdCard(birdCard);
                    inventory.birdCardSlot[battle].AddBirdCard(birdCard);
                    break;
                case LEADER_TYPE.TYRANT:
                    inventory.birdCardSlot[move].AddBirdCard(birdCard);
                    inventory.birdCardSlot[build].AddBirdCard(birdCard);
                    break;
            }
        }
    }
    public int GetCards
    {
        get => getCards;
        set
        {
            getCards = value;
            if (hasBuildingDic.Count < 3)
                getCards = 1;
            else if (hasBuildingDic.Count > 2 && hasBuildingDic.Count < 6)
                getCards = 2;
            else if (hasBuildingDic.Count >= 6)
                getCards = 3;
        }
    }
    private new void Start()
    {
        Score = 0;
        DrawCardNum = 1;
        base.Start();
        hasNodeNames.Add("생쥐1");
        NowLeader = LEADER_TYPE.NONE;
        particlesParent = RoundManager.Instance.effectParent;
    }


    public override GameObject SpawnSoldier(string tileName, Transform targetTransform)
    {
        bool isCheckBuilding = hasBuildingDic.Count > 0;

        if (isFirstCheck)
            base.SpawnSoldier(tileName, targetTransform);
        else if (isCheckBuilding)
        {
            Vector3 tempVec = Vector3.zero;
            if (hasSoldierDic.ContainsKey(tileName))
            {
                tempVec = new Vector3(hasSoldierDic[tileName].Count, 0, 0);
            }
            if (this.nowLeader == LEADER_TYPE.PROPHET)
            {
                GameObject dubleSoldier = Instantiate(prefabSoldier, targetTransform.position + tempVec, Quaternion.identity);
                SetHasNode(tileName, dubleSoldier.GetComponent<Soldier>());//그타일에 방금 만든 병사를 저장해줌.
                return dubleSoldier;//생성한 병사를 return시킴
            }
            GameObject addedSoldier = Instantiate(prefabSoldier, targetTransform.position + tempVec, Quaternion.identity);
            SetHasNode(tileName, addedSoldier.GetComponent<Soldier>());//그타일에 방금 만든 병사를 저장해줌.
            //cardSlot.CurCard++;
            return addedSoldier;//생성한 병사를 return시킴

            // 2마리가 생성되게 해야함 - 선진
            // 그리고 지금 클릭 시, 상태가 변경되는거 고쳐야 됨.

            //더해줄 병사를 임의로 저장해주고
        }
        return null;
    }
    
    
    public override void SpawnBuilding(string tileName, Transform targetTransform, GameObject building)
    {
        if (hasBuildingDic.ContainsKey(tileName) == false)
            hasBuildingDic.Add(tileName, new List<GameObject>());
        if (!hasBuildingDic[tileName].Exists(temp => temp.GetComponent<Building>().type == building.GetComponent<Building>().type))
            SetHasBuildingNode(tileName, targetTransform, building); // 리스트에 넣고
        else
            Debug.Log("이미 건설됨");
        int temp = 0;
        foreach (KeyValuePair<string, List<GameObject>> kv in hasBuildingDic)
        {
            temp += kv.Value.Count;
        }
        if(temp >= 6)
        {
            DrawCardNum = 3;
        }
        else if(temp >= 3)
        {
            DrawCardNum = 2;
        }
        else
        {
            DrawCardNum = 1;
        }
    }
    
    public override void SetHasBuildingNode(string tileName, Transform targetTransform, GameObject building)
    {
        hasBuildingDic[tileName].Add(building);
        BuildingManager.Instance.InstantiateBuilding(building, targetTransform);
    }
    public void BreakingRule()
    {
        Debug.LogWarning("룰을 어김");
        RoundManager.Instance.bird.NowLeader = LEADER_TYPE.NONE;
        for (int i = 0; i < 4; i++) 
        {
            Uimanager.Instance.birdUI.birdSlot[i].CardReset();
        }
        Uimanager.Instance.birdUI.alarmText.text = "규율을 어겨 지도자가 교체 됩니다.";
        RoundManager.Instance.roundSM.SetState(MASTATE_TYPE.WOOD_WAIT);
    }
    public void SetBirdMoveTileEffect(NodeMember tempMem)
    {
        List<string> seedMem = new List<string>();
        NodeMember nowMem = roundManager.mapExtra.mapTiles.Find(tempNode => tempNode.nodeName == tempMem.nodeName);

        List<Edge> edges = roundManager.mapExtra.graph.nodeList.Find(node => node.name == nowMem.nodeName).edgesInNode;
        foreach (Edge edge in edges)
        {
            if (!seedMem.Exists(tempName => tempName == edge.eNode.name))
            {
                seedMem.Add(edge.eNode.name);
            }
            if (!seedMem.Exists(tempName => tempName == edge.sNode.name))
            {
                seedMem.Add(edge.sNode.name);
            }
        }
        seedMem.Remove(nowMem.nodeName);
        if (seedMem.Count > 0)
        {            
            for (int i = 0; i < seedMem.Count; i++)
            {
                Debug.Log(seedMem[i]);
                NodeMember member = roundManager.mapExtra.mapTiles.Find(node => node.nodeName == seedMem[i]);
                member.transform.GetChild(0).GetComponent<Effect>().gameObject.SetActive(true);
                Debug.Log(member.transform.GetChild(0).name);
            }
        }
        else
        {
            Uimanager.Instance.playerUI.AlarmWindow.SetActive(true);
            Uimanager.Instance.playerUI.turnAlarmText.text = "에러";
            roundManager.testType = RoundManager.SoldierTestType.Select;
        }
    }
    public void DeleteCard()
    {
        isDelete = true;
    }
}