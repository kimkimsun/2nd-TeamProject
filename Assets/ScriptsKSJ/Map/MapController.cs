using sihyeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using CustomInterface;

public class MapController : MonoBehaviour, IPointerDownHandler
{
    [Header("[움직일 애]")]
    public List<Soldier> soldiers = new List<Soldier>();//현재 선택된 움직일 병사들 turntext가 select일때 타일을 누르면
    public int soldierNum;
    //그곳이 자신의 땅에 포함되어 있으면 병사들을 모두 가져옴 (명수 버튼 생기면 함수 수정예정)
    public NodeMember nowTile;//현재 선택한 타일
    List<string> nodeStrings = new List<string>(); //움직일 노드의 순서를 저장한 리스트 (계산한 것)
    MapExtra mapExtra;//계산을 위해 스크립트 저장해놓은것
    Coroutine moveCo;//이동할 때 코루틴 시작하는 거 저장
    Soldier checkSoldier;//코루틴 돌릴때 이동 끝난거 체크하려고 병사하나 넣어 놓은 것

    [Header("[맵 카메라]")]
    public Camera miniMapCam; // 클릭을 위해 있는 렌더러 카메라
    public GameObject prefaba; // 클릭했을때 클릭한 곳에 생성되는 박스(클릭포인트)
    [SerializeField]
    LayerMask layerMask;//타일만 선택할 수 있게 레이어마스크 설정

    public Action catOnAction;
    public event Action catEmploy;
    public Dictionary<RoundManager.SoldierTestType, Control> controlDic = new Dictionary<RoundManager.SoldierTestType, Control>();

    private void Start()
    {
        mapExtra = RoundManager.Instance.mapExtra;
        soldierNum = 0;
        controlDic.Add(RoundManager.SoldierTestType.Move, new ControlMove());
    }
    public void CursorCal(PointerEventData eventData)
    {
        Vector2 cursor = new Vector2(0, 0);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform,
            eventData.pressPosition, eventData.pressEventCamera, out cursor))
        {
            Texture texture = GetComponent<RawImage>().texture;
            Rect rect = GetComponent<RawImage>().rectTransform.rect;

            float coordX = Mathf.Clamp(0.0f, (((cursor.x - rect.x) * texture.width) / rect.width), texture.width);
            float coordY = Mathf.Clamp(0.0f, (((cursor.y - rect.y) * texture.height) / rect.height), texture.height);
            //Debug.Log(coordX + ", " + coordY);
            //Debug.Log(curosr.x + ", " + curosr.y);
            //Debug.Log(rect.x + ", " + rect.y);
            //Debug.Log(rect.width + ", " + rect.height);//클릭하는 캔버스의 크기
            //Debug.Log(texture.width + ", " + texture.height);//맵 rawimage에 들어가는  rawimage텍스쳐의 크기
            float calX = coordX / texture.width;
            float calY = coordY / texture.height;

            cursor = new Vector2(calX, calY);
            //여기까지 클릭되는 곳 계산한 것
            CastRayToWorld(cursor);
            /*
            if (RoundManager.Instance.testType == RoundManager.SoldierTestType.Select)//선택될떄마다
            {
                tileTextObj.gameObject.SetActive(true);
                tileTextObj.position = cursor;
            }*/

        }
    }
    private void Update()
    {
        /*
        if(tileTextObj !=null && nowTile == null)
        {
            tileTextObj.gameObject.SetActive(false);
        }*/
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        CursorCal(eventData);
    }

    private void CastRayToWorld(Vector2 vec)//맵만 쏠수있게 바ㅜ꺼야함
    {

        Ray MapRay = miniMapCam.ScreenPointToRay(new Vector2(vec.x * miniMapCam.pixelWidth,
            vec.y * miniMapCam.pixelHeight));
        //캔버스에서 계산한 값을 레이캐스터를 렌더러카메라에서 바닥으로 쏴서 
        //클릭한 곳을 찾는 것
        RaycastHit miniMapHit;
        //minimapHit.point를 하면 클릭한 위치 찾을 수 있음.
        if (Physics.Raycast(MapRay, out miniMapHit, Mathf.Infinity, layerMask))
        {
            Instantiate(prefaba, miniMapHit.point, Quaternion.identity);

            if (RoundManager.Instance.nowPlayer != null)
            {
                SetSoldier(miniMapHit);
            }
        }
    }
    //클릭할 떄 enum변수에 따라서 이벤트가 달라짐.
    void SetSoldier(RaycastHit miniMapHit)
    {
        //controlDic[RoundManager.Instance.testType].Active(miniMapHit);


        switch (RoundManager.Instance.testType)
        {
            case RoundManager.SoldierTestType.None:
                break;
            case RoundManager.SoldierTestType.Select:
                if (miniMapHit.transform.TryGetComponent(out NodeMember tempMem))//nodemember를 찾음.
                {
                    Debug.Log(tempMem.nodeName);
                    nowTile = tempMem;
                    //선택된 애들을 리스트에 넣어줌.
                }
                else
                    soldiers.Clear();
                break;
            case RoundManager.SoldierTestType.BirdSpawn:
                if (miniMapHit.transform.TryGetComponent(out NodeMember temp))
                {
                    nowTile = temp;

                    if(temp.isTileCheck == true && RoundManager.Instance.bird.NowLeader == LEADER_TYPE.PROPHET)
                    {
                        for(int m = 0; m < 2; m++)
                        {
                            RoundManager.Instance.bird.SpawnSoldier(nowTile.nodeName, nowTile.transform);
                            RoundManager.Instance.testType = RoundManager.SoldierTestType.Select;
                            for (int k = 0; k < RoundManager.Instance.mapExtra.mapTiles.Count; k++)
                            {
                                RoundManager.Instance.mapExtra.mapTiles[k].isTileCheck = false;
                                Uimanager.Instance.birdUI.alarmText.text = "모병완료!";
                            }
                        }
                        BirdCardAction tempBC = Uimanager.Instance.birdUI.birdSlot[0];
                        if (tempBC.curNum < tempBC.isOver.Count)
                        {
                            tempBC.isOver[tempBC.curNum] = true;
                        }
                        else
                        {
                            BirdCardAction tempBC1 = Uimanager.Instance.birdUI.birdSlot[1];
                            BirdCardAction tempBC2 = Uimanager.Instance.birdUI.birdSlot[2];
                            BirdCardAction tempBC3 = Uimanager.Instance.birdUI.birdSlot[3];
                            if (tempBC1.birdCards.Count > 0)
                            {
                                tempBC1.isOver[0] = true;
                            }
                            else if (tempBC2.birdCards.Count > 0)
                            {
                                tempBC2.isOver[0] = true;
                            }
                            else if(tempBC3.birdCards.Count > 0)
                            {
                                tempBC3.isOver[0] = true;
                            }
                            else
                            {
                                Debug.Log("에러");
                            }
                        }
                    }                    
                    else if (temp.isTileCheck == true)
                    {
                        Debug.Log("1");
                        RoundManager.Instance.bird.SpawnSoldier(nowTile.nodeName, nowTile.transform);
                        RoundManager.Instance.testType = RoundManager.SoldierTestType.Select;
                        Uimanager.Instance.birdUI.alarmText.text = "모병완료!";
                        for (int k = 0; k < RoundManager.Instance.mapExtra.mapTiles.Count; k++)
                        {
                            RoundManager.Instance.mapExtra.mapTiles[k].isTileCheck = false;
                        }
                        BirdCardAction tempBC = Uimanager.Instance.birdUI.birdSlot[0];
                        if (tempBC.curNum < tempBC.isOver.Count)
                        {
                            tempBC.isOver[tempBC.curNum] = true;
                        }
                        else
                        {
                            Debug.Log("왜안되는데");
                            BirdCardAction tempBC1 = Uimanager.Instance.birdUI.birdSlot[1];
                            BirdCardAction tempBC2 = Uimanager.Instance.birdUI.birdSlot[2];
                            BirdCardAction tempBC3 = Uimanager.Instance.birdUI.birdSlot[3];
                            if (tempBC1.birdCards.Count > 0)
                            {
                                tempBC1.isOver[0] = true;
                            }
                            else if (tempBC2.birdCards.Count > 0)
                            {
                                tempBC2.isOver[0] = true;
                            }
                            else
                            {
                                tempBC3.isOver[0] = true;
                            }

                            tempBC.curNum = 0;
                        }
                        Debug.Log(tempBC.curNum);

                    }
                    else
                        RoundManager.Instance.testType = RoundManager.SoldierTestType.BirdSpawn;
                    Uimanager.Instance.birdUI.birdAlarm.gameObject.SetActive(false);
                }
                RoundManager.Instance.SetOffAllEffect();
                break;
            case RoundManager.SoldierTestType.Spawn:
                if (miniMapHit.transform.TryGetComponent(out NodeMember spawnMem))
                {
                    nowTile = spawnMem;
                    //string tempName = RoundManager.Instance.nowPlayer.hasNodeNames[0];//테스트용 리스트
                    string tempName = nowTile.nodeName;
                    RoundManager.Instance.nowPlayer.SpawnSoldier(tempName,
                    RoundManager.Instance.mapExtra.mapTiles.Find(node => node.nodeName == tempName).transform);
                    RoundManager.Instance.SetOffAllEffect();
                }
                RoundManager.Instance.testType = RoundManager.SoldierTestType.Select;
                break;
            case RoundManager.SoldierTestType.MoveSelect:
                if (miniMapHit.transform.TryGetComponent(out NodeMember tempTile))//nodemember를 찾음.
                {
                    nowTile = tempTile;
                    RoundManager.Instance.SetOffAllEffect();
                    if (RoundManager.Instance.nowPlayer is Bird bird)
                    {
                        if (nowTile.isTileCheck == true)
                        {
                            RoundManager.Instance.bird.SetBirdMoveTileEffect(nowTile);
                            soldiers = RoundManager.Instance.nowPlayer.hasSoldierDic[tempTile.nodeName];
                            Uimanager.Instance.playerUI.MoveSoldier();
                            for (int k = 0; k < RoundManager.Instance.mapExtra.mapTiles.Count; k++)
                            {
                                RoundManager.Instance.mapExtra.mapTiles[k].isTileCheck = false;
                            }
                        }
                    }
                    else if (RoundManager.Instance.nowPlayer is Cat cat)
                    {
                        RoundManager.Instance.bird.SetBirdMoveTileEffect(nowTile);
                        soldiers = RoundManager.Instance.nowPlayer.hasSoldierDic[tempTile.nodeName];
                        Uimanager.Instance.playerUI.MoveSoldier();
                    }
                    else if (RoundManager.Instance.nowPlayer.hasSoldierDic.ContainsKey(tempTile.nodeName) &&
                        RoundManager.Instance.nowPlayer.hasSoldierDic[tempTile.nodeName].Count > 0)
                    {
                        soldiers = RoundManager.Instance.nowPlayer.hasSoldierDic[tempTile.nodeName];
                        Uimanager.Instance.playerUI.MoveSoldier();
                        //선택된 애들을 리스트에 넣어줌.
                    }
                }
                else
                    soldiers.Clear();                
                break;
            case RoundManager.SoldierTestType.Move:
                NodeMember finNode = null;               
                {  
                    if (RoundManager.Instance.nowPlayer is Cat cat)
                    {
                        if(RoundManager.Instance.cat.actionPoint >0)
                        {
                            if (RoundManager.Instance.cat.firstMove == false)
                            {
                                RoundManager.Instance.cat.firstMove = true;
                                if (moveCo != null)
                                {
                                    StopCoroutine(moveCo);
                                }
                                RoundManager.Instance.moveOver = true;
                                if (miniMapHit.transform.TryGetComponent(out NodeMember mem))
                                {
                                    finNode = mem;
                                    //Debug.Log("a" + nowTile.nodeName + "/" + finNode.nodeName);
                                    nodeStrings = mapExtra.SetAl(nowTile.nodeName, finNode.nodeName);
                                    //최단거리 계산하는 부분.
                                }
                                moveCo = StartCoroutine("MoveCoroutine");

                                Uimanager.Instance.playerUI.soldierMove.SetActive(false);
                                Uimanager.Instance.playerUI.isOn = true;
                                RoundManager.Instance.testType = RoundManager.SoldierTestType.Select;
                                catOnAction();
                                break;
                            }
                            else if (RoundManager.Instance.cat.secondMove == false)
                            {
                                RoundManager.Instance.cat.secondMove = true;
                                if (moveCo != null)
                                {
                                    StopCoroutine(moveCo);
                                }
                                RoundManager.Instance.moveOver = true;
                                if (miniMapHit.transform.TryGetComponent(out NodeMember mem))
                                {
                                    finNode = mem;
                                    //Debug.Log("a" + nowTile.nodeName + "/" + finNode.nodeName);
                                    nodeStrings = mapExtra.SetAl(nowTile.nodeName, finNode.nodeName);
                                    //최단거리 계산하는 부분.
                                }
                                moveCo = StartCoroutine("MoveCoroutine");

                                Uimanager.Instance.playerUI.soldierMove.SetActive(false);
                                Uimanager.Instance.playerUI.isOn = true;
                                RoundManager.Instance.testType = RoundManager.SoldierTestType.Select;
                                catOnAction();
                                break;
                            }
                            if(RoundManager.Instance.cat.actionPoint ==0)
                            {
                                Uimanager.Instance.playerUI.buildBtn.enabled = false;
                            }
                            else
                            {
                                Uimanager.Instance.playerUI.soldierMove.SetActive(false);
                                Debug.Log("포인트없거나 두번다 이동함");
                                break;
                            }
                        }
                        break;
                    }
                    else
                    {
                        if(RoundManager.Instance.nowPlayer is Bird bird)
                        {
                            BirdCardAction tempBC = Uimanager.Instance.birdUI.birdSlot[1];
                            Debug.Log(tempBC.isOver.Count);
                            Debug.Log(tempBC.curNum);
                            if (tempBC.curNum < tempBC.isOver.Count)
                            {
                                tempBC.isOver[tempBC.curNum] = true;
                            }
                            else
                            {
                                BirdCardAction tempBC2 = Uimanager.Instance.birdUI.birdSlot[2];
                                BirdCardAction tempBC3 = Uimanager.Instance.birdUI.birdSlot[3];
                                if (tempBC2.birdCards.Count > 0)
                                {
                                    Debug.Log("테스트1");
                                    tempBC2.isOver[0] = true;
                                }
                                else if(tempBC3.birdCards.Count > 0)
                                {
                                    Debug.Log("테스트2");
                                    tempBC3.isOver[0] = true;
                                }
                                else
                                {
                                    Uimanager.Instance.playerUI.nextBtn.gameObject.SetActive(true);
                                    Debug.Log("에러");
                                }
                            }
                        }
                        if (moveCo != null)
                        {
                            StopCoroutine(moveCo);
                        }
                        RoundManager.Instance.moveOver = true;
                        if (miniMapHit.transform.TryGetComponent(out NodeMember mem))
                        {
                            finNode = mem;
                            //Debug.Log("a" + nowTile.nodeName + "/" + finNode.nodeName);
                            nodeStrings = mapExtra.SetAl(nowTile.nodeName, finNode.nodeName);
                            //최단거리 계산하는 부분.
                        }
                        moveCo = StartCoroutine("MoveCoroutine");

                        Uimanager.Instance.playerUI.soldierMove.SetActive(false);
                        Uimanager.Instance.playerUI.isOn = true;
                        Uimanager.Instance.birdUI.alarmText.text = "이동할 공터를 선택하세요 선택하세요.";
                        RoundManager.Instance.testType = RoundManager.SoldierTestType.Select;
                        Uimanager.Instance.birdUI.birdAlarm.gameObject.SetActive(false);
                        break;
                    }
                }
            case RoundManager.SoldierTestType.Build:
                if (miniMapHit.transform.TryGetComponent(out NodeMember buildTile))//nodemember를 찾음.
                {
                    nowTile = buildTile;
                    if (RoundManager.Instance.nowPlayer is Bird bird)
                    {
                        BuildingManager.Instance.selectedBuilding = BuildingManager.Instance.BuildingDics[Building_TYPE.BIRD_NEST];
                        if (nowTile.isTileCheck == true && RoundManager.Instance.bird.hasBuildingDic.ContainsKey(nowTile.nodeName) == false)
                        {
                            RoundManager.Instance.nowPlayer.SpawnBuilding(nowTile.nodeName, nowTile.transform,
                            BuildingManager.Instance.selectedBuilding);
                            RoundManager.Instance.testType = RoundManager.SoldierTestType.Select;
                            for (int k = 0; k < RoundManager.Instance.mapExtra.mapTiles.Count; k++)
                            {
                                RoundManager.Instance.mapExtra.mapTiles[k].isTileCheck = false;
                            }
                            BirdCardAction tempBC = Uimanager.Instance.birdUI.birdSlot[3];
                            if (tempBC.curNum < tempBC.isOver.Count)
                            {
                                tempBC.isOver[tempBC.curNum] = true;
                            }
                            else
                            {
                                Uimanager.Instance.playerUI.nextBtn.gameObject.SetActive(true);
                                Debug.Log("에러");
                            }
                        }
                    }
                    if (RoundManager.Instance.nowPlayer is Wood wood)
                    {
                        wood.buildCost = 1;
                        RoundManager.Instance.nowPlayer.SpawnBuilding(nowTile.nodeName, nowTile.transform,
                        BuildingManager.Instance.selectedBuilding);
                        RoundManager.Instance.testType = RoundManager.SoldierTestType.Select;
                    }
                    if(RoundManager.Instance.nowPlayer is Cat cat)
                    {
                        if (RoundManager.Instance.cat.actionPoint > 0)
                        {
                            
                            RoundManager.Instance.nowPlayer.SpawnBuilding(nowTile.nodeName, nowTile.transform,
                            BuildingManager.Instance.selectedBuilding);
                            RoundManager.Instance.testType = RoundManager.SoldierTestType.Select;
                            if (BuildingManager.Instance.selectedBuilding == BuildingManager.Instance.catSawMillPrefab&& RoundManager.Instance.cat.woodProductNum > RoundManager.Instance.cat.catSawMillCost)
                            {
                                Debug.Log("1111111111");
                                Debug.Log(RoundManager.Instance.cat.RemainSawmillNum);
                                RoundManager.Instance.cat.RemainSawmillNum -=1;
                                Debug.Log(RoundManager.Instance.cat.RemainSawmillNum);
                                RoundManager.Instance.cat.woodProductNum -= RoundManager.Instance.cat.catSawMillCost;
                                RoundManager.Instance.cat.costBuilding();
                                Uimanager.Instance.catUI.sawmillCostText.text = RoundManager.Instance.cat.catSawMillCost.ToString();
                                Uimanager.Instance.catUI.woodProductText.text = RoundManager.Instance.cat.woodProductNum.ToString();
                                Uimanager.Instance.catUI.bulidSectionSawmillCostText.text = RoundManager.Instance.cat.catSawMillCost.ToString();
                            }
                            if (BuildingManager.Instance.selectedBuilding == BuildingManager.Instance.catBarrackPrefab &&RoundManager.Instance.cat.woodProductNum > RoundManager.Instance.cat.catBarrackCost)
                            {
                                RoundManager.Instance.cat.RemainBarracksNum -=1;
                                RoundManager.Instance.cat.woodProductNum -= RoundManager.Instance.cat.catBarrackCost;
                                RoundManager.Instance.cat.costBuilding();
                                Uimanager.Instance.catUI.barracksCostText.text = RoundManager.Instance.cat.catBarrackCost.ToString();
                                Uimanager.Instance.catUI.woodProductText.text = RoundManager.Instance.cat.woodProductNum.ToString();
                                Uimanager.Instance.catUI.bulidSectionBarracksCostText.text = RoundManager.Instance.cat.catBarrackCost.ToString();
                            }
                            if (BuildingManager.Instance.selectedBuilding == BuildingManager.Instance.catWorkShopPrefab && RoundManager.Instance.cat.woodProductNum > RoundManager.Instance.cat.catWorkShopCost)
                            {
                                RoundManager.Instance.cat.RemainWorkshopsNum -=1;
                                RoundManager.Instance.cat.woodProductNum -= RoundManager.Instance.cat.catWorkShopCost;
                                RoundManager.Instance.cat.costBuilding();
                                Uimanager.Instance.catUI.workshopCostText.text = RoundManager.Instance.cat.catWorkShopCost.ToString();
                                Uimanager.Instance.catUI.woodProductText.text = RoundManager.Instance.cat.woodProductNum.ToString();
                                Uimanager.Instance.catUI.bulidSectionWorkshopCostText.text = RoundManager.Instance.cat.catWorkShopCost.ToString();
                            }
                            catOnAction();
                            if(RoundManager.Instance.cat.actionPoint == 0)
                            {
                                Uimanager.Instance.playerUI.buildBtn.enabled = false;
                            }
                        }
                        else
                        {
                            Debug.Log("액션포인트없음");
                        }
                    }
                }
                RoundManager.Instance.SetOffAllEffect();
                break;
            case RoundManager.SoldierTestType.Revoit:
                if (miniMapHit.transform.TryGetComponent(out NodeMember revoitTile))//nodemember를 찾음.
                {
                    nowTile = revoitTile;
                    if (RoundManager.Instance.wood.hasBuildingDic[nowTile.nodeName].
                        Exists(gameObject => gameObject.GetComponent<Building>().type == Building_TYPE.WOOD_TOKEN))
                    {
                        RoundManager.Instance.wood.buildCost = 2;
                        BuildingManager.Instance.SetWoodBase(nowTile);
                        RoundManager.Instance.nowPlayer.SpawnBuilding(nowTile.nodeName, nowTile.transform,
                        BuildingManager.Instance.selectedBuilding);
                    }
                    RoundManager.Instance.SetOffAllEffect();
                    RoundManager.Instance.testType = RoundManager.SoldierTestType.Select;
                }
                break;
            case RoundManager.SoldierTestType.Battle:
                if (miniMapHit.transform.TryGetComponent(out NodeMember battleMem))//nodemember를 찾음.
                {
                    nowTile = battleMem;
                    if(RoundManager.Instance.nowPlayer.craftedCards.Exists(//배틀카드 체크하려고
                        tempCard => tempCard.skillType == CustomInterface.CARD_SKILL_TYPE.BATTLE))//방어도 추가
                    {

                    }
                    if (RoundManager.Instance.nowPlayer is Bird bird && (nowTile.isTileCheck == true))
                    {
                        Uimanager.Instance.playerUI.battleWindow.gameObject.SetActive(true);
                        for (int k = 0; k < RoundManager.Instance.mapExtra.mapTiles.Count; k++)
                        {
                            RoundManager.Instance.mapExtra.mapTiles[k].isTileCheck = false;
                        }
                        BirdCardAction tempBC = Uimanager.Instance.birdUI.birdSlot[2];
                        if (tempBC.curNum < tempBC.isOver.Count)
                        {
                            tempBC.isOver[tempBC.curNum] = true;
                        }
                        else
                        {
                            BirdCardAction tempBC3 = Uimanager.Instance.birdUI.birdSlot[3];
                            if (tempBC3.birdCards.Count > 0)
                            {
                                tempBC3.isOver[0] = true;
                            }
                            else
                            {
                                Uimanager.Instance.playerUI.nextBtn.gameObject.SetActive(true);
                                Debug.Log("에러");
                            }
                        }
                        RoundManager.Instance.testType = RoundManager.SoldierTestType.Select;
                    }
                    else
                    {
                        RoundManager.Instance.testType = RoundManager.SoldierTestType.Select;
                        Uimanager.Instance.playerUI.battleWindow.gameObject.SetActive(true);
                    }
                    /*
                    //if (RoundManager.Instance.nowPlayer is Cat cat)
                    //{
                    //    if (RoundManager.Instance.cat.actionPoint > 0)
                    //    {
                    //        Debug.Log(battleMem.nodeName);
                    //        nowTile = battleMem;
                    //        catOnAction();
                    //        Uimanager.Instance.playerUI.battleWindow.gameObject.SetActive(true);
                    //        if (RoundManager.Instance.cat.actionPoint == 0)
                    //        {
                    //            Uimanager.Instance.playerUI.battleBtn.enabled = false;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    Debug.Log(battleMem.nodeName);
                    //    nowTile = battleMem;
                    //    Uimanager.Instance.playerUI.battleWindow.gameObject.SetActive(true);
                    //}*/
                }
                else
                {
                    Uimanager.Instance.playerUI.battleWindow.gameObject.SetActive(false);
                    Debug.Log(RoundManager.Instance.cat.hasSoldierDic[nowTile.nodeName].Count);
                }
                break;
            #region 고양이 후작
            case RoundManager.SoldierTestType.CatSet: // 첫 세팅..
                RoundManager.Instance.bird.isFirstCheck = true;
                if (miniMapHit.transform.TryGetComponent(out NodeMember settingTile))
                {
                    nowTile = settingTile;
                    if (nowTile == RoundManager.Instance.mapExtra.mapTiles[11])
                    {
                        Debug.Log("쥐 4");  //
                        for (int i = 0; i < mapExtra.mapTiles.Count; i++)
                        {
                            if (i != 0)
                                RoundManager.Instance.cat.SpawnSoldier(mapExtra.mapTiles[i].nodeName, mapExtra.mapTiles[i].transform);
                            else
                            {
                                for (int j = 0; j < 7; j++)
                                {
                                    RoundManager.Instance.bird.SpawnSoldier(mapExtra.mapTiles[0].nodeName, mapExtra.mapTiles[0].transform);
                                }
                            }
                        }
                        RoundManager.Instance.bird.isFirstCheck = false;
                        RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform,
                        BuildingManager.Instance.catBasePrefab);
                        RoundManager.Instance.bird.SpawnBuilding(RoundManager.Instance.mapExtra.mapTiles[0].nodeName, RoundManager.Instance.mapExtra.mapTiles[0].transform,
                        BuildingManager.Instance.birdNestPrefab);
                        Debug.Log(RoundManager.Instance.mapExtra.mapTiles[0].nodeName);
                        Debug.Log(RoundManager.Instance.bird.hasBuildingDic[RoundManager.Instance.mapExtra.mapTiles[0].nodeName][0].name);
                    }
                    else if (nowTile == RoundManager.Instance.mapExtra.mapTiles[8])
                    {
                        Debug.Log("여우4"); //에 새 둥지 생성 
                        for (int i = 0; i < mapExtra.mapTiles.Count; i++)
                        {
                            if (i != 2)
                                RoundManager.Instance.cat.SpawnSoldier(mapExtra.mapTiles[i].nodeName, mapExtra.mapTiles[i].transform);
                            else
                            {
                                for (int j = 0; j < 7; j++)
                                {
                                    RoundManager.Instance.bird.SpawnSoldier(mapExtra.mapTiles[2].nodeName, mapExtra.mapTiles[2].transform);
                                }
                            }
                        }
                        RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform,
                        BuildingManager.Instance.catBasePrefab);
                        //Debug.Log(RoundManager.Instance.bird.hasBuildingDic[RoundManager.Instance.mapExtra.mapTiles[8].nodeName].Count);
                        RoundManager.Instance.bird.SpawnBuilding(RoundManager.Instance.mapExtra.mapTiles[2].nodeName, RoundManager.Instance.mapExtra.mapTiles[2].transform,
                        BuildingManager.Instance.birdNestPrefab);
                        Debug.Log(RoundManager.Instance.bird.hasBuildingDic[RoundManager.Instance.mapExtra.mapTiles[2].nodeName].Count);

                    }
                    else if (nowTile == RoundManager.Instance.mapExtra.mapTiles[2])
                    {
                        Debug.Log("생쥐 1");
                        for (int i = 0; i < mapExtra.mapTiles.Count; i++)
                        {
                            if (i != 8)
                                RoundManager.Instance.cat.SpawnSoldier(mapExtra.mapTiles[i].nodeName, mapExtra.mapTiles[i].transform);
                            else
                            {
                                for (int j = 0; j < 7; j++)
                                {
                                    RoundManager.Instance.bird.SpawnSoldier(mapExtra.mapTiles[8].nodeName, mapExtra.mapTiles[8].transform);
                                }
                            }
                        }
                        RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform,
                        BuildingManager.Instance.catBasePrefab);
                        RoundManager.Instance.bird.SpawnBuilding(RoundManager.Instance.mapExtra.mapTiles[8].nodeName, RoundManager.Instance.mapExtra.mapTiles[8].transform,
                        BuildingManager.Instance.birdNestPrefab);
                        Debug.Log(RoundManager.Instance.mapExtra.mapTiles[8].nodeName);
                    }
                    else if (nowTile == RoundManager.Instance.mapExtra.mapTiles[0])
                    {
                        Debug.Log("여우1");
                        for (int i = 0; i < mapExtra.mapTiles.Count; i++)
                        {
                            if (i != 11)
                                RoundManager.Instance.cat.SpawnSoldier(mapExtra.mapTiles[i].nodeName, mapExtra.mapTiles[i].transform);
                            else
                            {
                                for (int j = 0; j < 7; j++)
                                {
                                    RoundManager.Instance.bird.SpawnSoldier(mapExtra.mapTiles[11].nodeName, mapExtra.mapTiles[11].transform);
                                }
                            }
                        }
                        RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform,
                        BuildingManager.Instance.catBasePrefab);
                        RoundManager.Instance.bird.SpawnBuilding(RoundManager.Instance.mapExtra.mapTiles[11].nodeName, RoundManager.Instance.mapExtra.mapTiles[11].transform,
                        BuildingManager.Instance.birdNestPrefab);
                        //Debug.Log(RoundManager.Instance.mapExtra.mapTiles[11].nodeName);
                        Debug.Log(RoundManager.Instance.bird.hasBuildingDic[RoundManager.Instance.mapExtra.mapTiles[11].nodeName][0]);
                    }
                    else
                        return;

                    RoundManager.Instance.cat.baseNode = nowTile;
                    Debug.Log(RoundManager.Instance.cat.baseNode);
                    RoundManager.Instance.cat.isDisposable = false;
                    RoundManager.Instance.testType = RoundManager.SoldierTestType.CatSetSawMill;

                }
                break;
            case RoundManager.SoldierTestType.CatSetSawMill:
                if (miniMapHit.transform.TryGetComponent(out NodeMember settingTileA))
                {  
                 
                    //이유 발견 딕셔너리 생성이 안되있음 조건바꿔야함.
                    //- 딕셔너리가 1개만 생성되있음
                    //조건 바꿔야함.// 해결 - 12.17 시현
                    nowTile = settingTileA;                    
                    if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("여우1")) //여우 1 이면 토끼1,여우2,생쥐2에 건설할수있어야함   여우 1이 0;
                    {
                        if (nowTile.nodeName == "토끼1")
                        {
                            RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catSawMillPrefab);
                            RoundManager.Instance.cat.RemainSawmillNum--;
                            RoundManager.Instance.cat.costBuilding();
                        }
                        else if (nowTile.nodeName == "여우2")
                        {
                            RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catSawMillPrefab);
                            RoundManager.Instance.cat.RemainSawmillNum--;
                            RoundManager.Instance.cat.costBuilding();
                        }
                        else if (nowTile.nodeName == "생쥐2")
                        {
                            RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catSawMillPrefab);
                            RoundManager.Instance.cat.RemainSawmillNum--;
                            RoundManager.Instance.cat.costBuilding();
                        }                       
                    }
                    else if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("여우4")) //여우4는 생쥐2,3 토끼 2                       
                    {
                        if (nowTile.nodeName == "생쥐2")
                        {
                            RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catSawMillPrefab);
                            RoundManager.Instance.cat.RemainSawmillNum--;
                            RoundManager.Instance.cat.costBuilding();
                        }
                        else if (nowTile.nodeName == "생쥐4")
                        {
                            RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catSawMillPrefab);
                            RoundManager.Instance.cat.RemainSawmillNum--;
                            RoundManager.Instance.cat.costBuilding();
                        }
                        else if (nowTile.nodeName == "토끼2")
                        {
                            RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catSawMillPrefab);
                            RoundManager.Instance.cat.RemainSawmillNum--;
                            RoundManager.Instance.cat.costBuilding();
                        }
                    }
                    else if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("생쥐1"))//토 1,3 여2                        
                    {
                        if (nowTile.nodeName == "토끼1")
                        {
                            RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catSawMillPrefab);
                            RoundManager.Instance.cat.RemainSawmillNum--;
                            RoundManager.Instance.cat.costBuilding();
                        }
                        else if (nowTile.nodeName == "토끼3")
                        {
                            RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catSawMillPrefab);
                            RoundManager.Instance.cat.RemainSawmillNum--;
                            RoundManager.Instance.cat.costBuilding();
                        }
                        else if (nowTile.nodeName == "여우2")
                        {
                            RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catSawMillPrefab);
                            RoundManager.Instance.cat.RemainSawmillNum--;
                            RoundManager.Instance.cat.costBuilding();
                        }
                    }
                    else if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("생쥐4"))// 토 34 여 3                        
                    {
                        if (nowTile.nodeName == "토끼3")
                        {
                            RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catSawMillPrefab);
                            RoundManager.Instance.cat.RemainSawmillNum--;
                            RoundManager.Instance.cat.costBuilding();
                        }
                        else if (nowTile.nodeName == "토끼4")
                        {
                            RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catSawMillPrefab);
                            RoundManager.Instance.cat.RemainSawmillNum--;
                            RoundManager.Instance.cat.costBuilding();
                        }
                        else if (nowTile.nodeName == "여우3")
                        {
                            RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catSawMillPrefab);
                            RoundManager.Instance.cat.RemainSawmillNum--;
                            RoundManager.Instance.cat.costBuilding();
                        }
                    }
                    else
                        break;
                    RoundManager.Instance.testType = RoundManager.SoldierTestType.CatSetWorkShop;                    
                }
                    break;             
            case RoundManager.SoldierTestType.CatSetWorkShop:
                if (miniMapHit.transform.TryGetComponent(out NodeMember settingTileB))
                {
                    nowTile = settingTileB;                    
                    NodeMember extraNode;

                    if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("여우1"))
                    {
                        if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("토끼1"))
                        {
                            if (nowTile.nodeName == "여우2")
                            {
                                extraNode = mapExtra.mapTiles[4];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();

                            }
                            else if (nowTile.nodeName == "생쥐2")
                            {
                                extraNode = mapExtra.mapTiles[3];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                        }
                        else if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("여우2"))
                        {
                            if (nowTile.nodeName == "토끼1")
                            {
                                extraNode = mapExtra.mapTiles[4];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                            else if (nowTile.nodeName == "생쥐2")
                            {
                                extraNode = mapExtra.mapTiles[1];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                        }
                        else if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("생쥐2"))
                        {
                            if (nowTile.nodeName == "토끼1")
                            {
                                extraNode = mapExtra.mapTiles[3];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                            else if (nowTile.nodeName == "여우2")
                            {
                                extraNode = mapExtra.mapTiles[1];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                        }
                    }
                    else if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("여우4")) //생쥐 23, 토끼 2
                    {
                        if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("생쥐2"))
                        {
                            if (nowTile.nodeName == "생쥐3")
                            {
                                extraNode = mapExtra.mapTiles[5];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                            else if (nowTile.nodeName == "토끼2")
                            {
                                extraNode = mapExtra.mapTiles[7];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                        }
                        else if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("생쥐3"))
                        {
                            if (nowTile.nodeName == "생쥐2")
                            {
                                extraNode = mapExtra.mapTiles[5];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                            else if (nowTile.nodeName == "토끼2")
                            {
                                extraNode = mapExtra.mapTiles[4];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                        }
                        else if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("토끼2"))
                        {
                            if (nowTile.nodeName == "생쥐3")
                            {
                                extraNode = mapExtra.mapTiles[4];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                            else if (nowTile.nodeName == "생쥐2")
                            {
                                extraNode = mapExtra.mapTiles[9];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                        }
                    }
                    else if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("생쥐1"))//토 1,3 여2
                    {
                        if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("토끼1"))
                        {
                            if (nowTile.nodeName == "토끼3")
                            {
                                extraNode = mapExtra.mapTiles[3];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                            else if (nowTile.nodeName == "여우2")
                            {
                                extraNode = mapExtra.mapTiles[7];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                        }
                        else if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("토끼3"))
                        {
                            if (nowTile.nodeName == "토끼1")
                            {
                                extraNode = mapExtra.mapTiles[3];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                            else if (nowTile.nodeName == "여우2")
                            {
                                extraNode = mapExtra.mapTiles[1];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                        }
                        else if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("여우2"))
                        {
                            if (nowTile.nodeName == "토끼3")
                            {
                                extraNode = mapExtra.mapTiles[1];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                            else if (nowTile.nodeName == "토끼1")
                            {
                                extraNode = mapExtra.mapTiles[7];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                        }
                    }
                    else if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("생쥐4"))// 토 34 여 3  
                    {
                        if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("토끼3"))
                        {
                            if (nowTile.nodeName == "토끼4")
                            {
                                extraNode = mapExtra.mapTiles[6];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                            else if (nowTile.nodeName == "여우3")
                            {
                                extraNode = mapExtra.mapTiles[10];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                        }
                        else if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("토끼4"))
                        {
                            if (nowTile.nodeName == "토끼3")
                            {
                                extraNode = mapExtra.mapTiles[6];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                            else if (nowTile.nodeName == "여우3")
                            {
                                extraNode = mapExtra.mapTiles[7];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                        }
                        else if (RoundManager.Instance.cat.hasBuildingDic.ContainsKey("여우3"))
                        {
                            if (nowTile.nodeName == "토끼3")
                            {
                                extraNode = mapExtra.mapTiles[10];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                            else if (nowTile.nodeName == "토끼4")
                            {
                                extraNode = mapExtra.mapTiles[7];
                                RoundManager.Instance.cat.SpawnBuilding(nowTile.nodeName, nowTile.transform, BuildingManager.Instance.catWorkShopPrefab);
                                RoundManager.Instance.cat.SpawnBuilding(extraNode.nodeName, extraNode.transform, BuildingManager.Instance.catBarrackPrefab);
                                RoundManager.Instance.cat.RemainBarracksNum--;
                                RoundManager.Instance.cat.RemainWorkshopsNum--;
                                RoundManager.Instance.cat.costBuilding();
                            }
                        }                       
                    }
                    else
                        break;
                    RoundManager.Instance.testType = RoundManager.SoldierTestType.Select;
                }
                break;
            case RoundManager.SoldierTestType.CatExtraWork: // 이부분은 성환이형 스크립트와 이어야함 나중에 .
                //if (RoundManager.Instance.cat.isWorked == false)
                //{
                //    RoundManager.Instance.cat.woodProductNum += RoundManager.Instance.cat.turnAddWoodToken;
                //    Uimanager.Instance.catUI.woodProductText.text = RoundManager.Instance.cat.woodProductNum.ToString();
                //    RoundManager.Instance.cat.isWorked = true;
                //}
                break;
            case RoundManager.SoldierTestType.CatRecruit:
                catEmploy();
                break;

            default: break;
            #endregion
        }

    }
    IEnumerator MoveCoroutine()//병사 이동하는 코루틴 
    {
        RoundManager.Instance.SetOffAllEffect();
        int count = nodeStrings.Count;
        int num = 1;
        while (count > 0)
        {
            NodeMember foundNode = mapExtra.mapTiles.Find(node => node.nodeName == nodeStrings[num]);
            Vector3 tempPostion = foundNode.transform.position;
            if (RoundManager.Instance.nowPlayer is Wood wood)
            {
                if (wood.BattleActionNum == wood.OfficerNum)
                {
                    Debug.Log("행동력 끝남");
                    break;//행동이끝나면 이동더못함.
                }

            }
            if (RoundManager.Instance.moveOver)
            {
                checkSoldier = soldiers[soldierNum - 1];
                for (int i = 1; i <= soldierNum; i++)
                {
                    //Debug.Log(soldiers.Count - 1);
                    Soldier tempSoldier = soldiers[soldiers.Count - 1];
                    //Debug.Log(nodeStrings[num]);
                    tempSoldier.MoveAuto(tempPostion);//움직이게하고  
                    RoundManager.Instance.nowPlayer.SetHasNode(foundNode.nodeName, tempSoldier);//옮겨갈 땅으로 병사정보
                    RoundManager.Instance.nowPlayer.hasSoldierDic[nowTile.nodeName].RemoveAt(soldiers.Count - 1);
                    //원래있던 곳에서 없애줌
                }
                    if(RoundManager.Instance.nowPlayer.hasSoldierDic[nowTile.nodeName].Count == 0)
                    {
                        RoundManager.Instance.nowPlayer.hasSoldierDic.Remove(nowTile.nodeName);
                    }
                //foreach (Soldier debuging in RoundManager.Instance.nowPlayer.hasSoldierDic[nowTile.nodeName])
                //{
                //    Debug.Log(debuging.name + "현재병사");
                //}
                //foreach (Soldier debuging in RoundManager.Instance.nowPlayer.hasSoldierDic[foundNode.nodeName])
                //{
                //    Debug.Log(debuging.name + "바뀐병사");
                //}

                nowTile = foundNode;
                soldiers = RoundManager.Instance.nowPlayer.hasSoldierDic[foundNode.nodeName];
                //RoundManager.Instance.nowPlayer.hasSoldierDic[nowTile.nodeName] = 
                count--;//다움직일때까지
                num++;//시작점을 제외하고 움직이기
                RoundManager.Instance.wood.BattleActionNum++;
                RoundManager.Instance.moveOver = false;
            }
            yield return new WaitForSeconds(Time.deltaTime * 20f);
            if (RoundManager.Instance.moveOver == false)
            {
                if(RoundManager.Instance.nowPlayer is Bird)
                {
                    break;
                }
                if (checkSoldier.agent.remainingDistance < 1f)
                {
                    checkSoldier.agent.ResetPath();
                    Debug.Log("들어옴");
                    RoundManager.Instance.moveOver = true;
                }
            }
            if (num >= nodeStrings.Count)
                break;
            yield return new WaitForSeconds(Time.deltaTime * 10f);
        }
        yield return null;
    }
}
