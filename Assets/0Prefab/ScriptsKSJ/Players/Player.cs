using CustomInterface;
using sihyeon;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int drawCardNum;
    public int DrawCardNum
    {
        get => drawCardNum;
        set
        {
            drawCardNum = value;
            if (this is Wood)
            {                
                Uimanager.Instance.woodUi.drawCardText.text = drawCardNum.ToString();
            }
            
        }
    }
    public PlayerInventory inven;
    public Dictionary<ANIMAL_COST_TYPE, List<Card>> cardDecks = new Dictionary<ANIMAL_COST_TYPE, List<Card>>();
    public List<Card> craftedCards = new List<Card>();

    public bool isOver;//현재 자기턴이 아니면 false상태
    [SerializeField]
    private int score;
    public List<string> hasNodeNames = new List<string>();//어떤 타일을 지배하고 있는지 체크하는 용도
    public GameObject prefabSoldier;
    public int battleSoldierNum;
    public int battleBuildingNum;
    public Dictionary<string, List<Soldier>> hasSoldierDic = new Dictionary<string, List<Soldier>>();
    //병사를 가지고있는 타일에 병사 리스트를 저장
    //병사명수를 정해서 리스트의 끝부터 하나씩 꺼내서 쓰게 하면됨.

    public Dictionary<string, List<GameObject>> hasBuildingDic = new Dictionary<string, List<GameObject>>();
    //건물 저장해둘 공간 -시현 추가
    protected RoundManager roundManager;//많이 쓸거같아서 넣어놨음.

    public int Score
    { 
        get { return score; } 
        set
        { 
            score = value; 
            if(score >= 0)
            {
                if(this is Cat)
                {
                    Uimanager.Instance.scoreBoard.SetScore(score, 0);
                }
                else if(this is Bird)
                {
                    Uimanager.Instance.scoreBoard.SetScore(score, 1);
                }
                else if(this is Wood)
                {
                    Uimanager.Instance.scoreBoard.SetScore(score, 2);
                }
            }
        }
    }
    public void AddCard(Card card, ANIMAL_COST_TYPE cardType)
    {
        //Debug.Log(card.gameObject.name);
        if (!cardDecks.ContainsKey(cardType))
        {
            cardDecks[cardType] = new List<Card>();
        }
        cardDecks[cardType].Add(card);
        inven.AddCard(card);
        Debug.Log(card.cardName + "뽑은카드");
    }

    protected void Start()
    {
        roundManager = RoundManager.Instance;
        isOver = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Debug.Log("무슨 타입인지 궁금해" + RoundManager.Instance.testType);
        }
    }
    public void SetPlayer()
    {
        RoundManager.Instance.nowPlayer = this;
    }
    public virtual GameObject SpawnSoldier(string tileName, Transform targetTransform)
    {

        Vector3 tempVec = Vector3.zero;
        if (hasSoldierDic.ContainsKey(tileName))//병사가 존재하는지 체크
        {
            tempVec = new Vector3(hasSoldierDic[tileName].Count, 0, 0);//명수에 따라 소환하는 위치를 바꿔야해서
        }
        GameObject addedSoldier = Instantiate(prefabSoldier, targetTransform.position + tempVec, Quaternion.identity);
        //더해줄 병사를 임의로 저장해주고
        SetHasNode(tileName, addedSoldier.GetComponent<Soldier>());//그타일에 방금 만든 병사를 저장해줌.
        return addedSoldier;//생성한 병사를 return시킴
    }
    public void SetHasNode(string tempName, Soldier tempSoldier)//리스트를 생성하고 더해주기위함.
    {
        if (hasSoldierDic.ContainsKey(tempName) == false)
        {//병사딕셔너리에 이게 있는지 체크하고 
            //없으면 list를 새로 생성해준다.
            hasSoldierDic.Add(tempName, new List<Soldier>());
        }
        hasSoldierDic[tempName].Add(tempSoldier);//딕셔너리에 추가해줌.
    }

    // 시현 추가//
    public virtual void SpawnBuilding(string tileName, Transform targetTransform, GameObject building)
    {

        if (hasBuildingDic.ContainsKey(tileName) == false)
        {
            hasBuildingDic.Add(tileName, new List<GameObject>());
        }
        if (!hasBuildingDic[tileName].Exists(temp => temp.GetComponent<Building>().type == building.GetComponent<Building>().type))//예외처리 실수
        {
            SetHasBuildingNode(tileName, targetTransform, building); // 리스트에 넣고
        }
        else
        {
            Debug.Log("이미 건설됨");
        }

    }
    public virtual void SetHasBuildingNode(string tileName, Transform targetTransform, GameObject building)
    {
        //Debug.Log("setHasBuilding 작동1");
        hasBuildingDic[tileName].Add(BuildingManager.Instance.InstantiateBuilding(building,targetTransform));
        //Debug.Log("setHasBuilding 작동2");
    }
    public void testSetBtn()
    {
        Uimanager.Instance.testBtn.onClick.RemoveAllListeners();
        Uimanager.Instance.testBtn.onClick.AddListener(() => {
            BuildingManager.Instance.selectedBuilding 
            = BuildingManager.Instance.BuildingDics[Building_TYPE.CAT_SAWMILL];
        });
    }


    //private void Update() //- 임시
    //{
    //    testSetBtn();
    //}
        public void DrawCard()
    {
        int tempNum = 0;
        foreach (KeyValuePair<ANIMAL_COST_TYPE, List<Card>> kv in cardDecks)
        {
            tempNum += kv.Value.Count;
            Debug.Log(kv.Key + " " + tempNum);
        }
        Debug.Log("현재 카드 수" + tempNum);
        int overCardNum = tempNum + drawCardNum - 4;
        Debug.Log("뽑아야하는 수" + overCardNum);
        if (overCardNum > 0)
        {
            for (int i = 0; i < overCardNum; i++)
            {
                CardManager.Instance.cardDeck.Add(cardDecks[inven.slot[0].card.costType][0]);
                cardDecks[inven.slot[0].card.costType].RemoveAt(0);
                inven.slot[0].EmptySlot();
            }
            CardManager.Instance.DrawCard(drawCardNum, this);
        }
        else
        {
            Debug.Log(drawCardNum);
            CardManager.Instance.DrawCard(drawCardNum, this);
        }
    }
}
