using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInterface;
using UnityEngine.UI;
using System;
using TMPro;
using Unity.VisualScripting;
using static UnityEngine.ParticleSystem;
using System.Linq;

public class RoundManager : SingleTon<RoundManager>
{
    public List<GameObject> particles = new List<GameObject>();
    public Transform effectParent;
    [Header("[플레이어들]")]
    public Cat cat;//플레이어중에 고양이 
    public Bird bird;//플레이어중에 이어리
    public Wood wood;//플레이어중에 우드랜드
    public ManagerStateMahchine<RoundManager> roundSM;//라운드 상태머신
    public Player nowPlayer;// 현재 턴인 플레이어 (라운드 상태머신에서 자동으로 바뀜)
    public bool moveOver; // mapcontroller에서 최종 도착지까지 가기위해 이동이 끝나면 다음 타일로 이동하기 위해 선언해둔 것
    [Header("스폰 테스트")]
    public MapExtra mapExtra;// 최단거리 이동 계산하기위해 스크립트 넣어놓은것



    public enum SoldierTestType//버튼으로 모병이나 선택이나 이런거 클릭할때의 기능이 바뀌게됨
    {
        None,
        MoveSelect,
        Move,
        Select,
        Spawn,
        Build,
        Revoit,
        Battle,
        CatSet,
        CatSetSawMill,       
        CatSetWorkShop,
        CatSetBarrack,
        CatExtraWork,
        CatRecruit,
        BirdSpawn,
    }
    // 예외처리 거의 안되어있음.
    public SoldierTestType testType;

    // 맵 이동하는거나 선택하거나 클릭하는 모든것들 스크립트
    [Header("[맵]")]
    public MapController mapController;

    private new void Awake()
    {
        base.Awake();
        roundSM = new ManagerStateMahchine<RoundManager>(this);
        mapExtra = transform.GetComponent<MapExtra>();
        List<Effect> tempList = effectParent.GetComponentsInChildren<Effect>().ToList();
        foreach (Effect effect in tempList)
        {
            particles.Add(effect.gameObject);
        }
    }
    void Start()
    {
        //상태머신에 상태들 추가한것
        moveOver = true;
        testType = SoldierTestType.Select;             

        roundSM.AddStateDic(MASTATE_TYPE.CAT_WAIT, new CatWaitState());
        roundSM.AddStateDic(MASTATE_TYPE.CAT_MORNING, new CatMorningState());
        roundSM.AddStateDic(MASTATE_TYPE.CAT_AFTERNOON, new CatAfternoonState());
        roundSM.AddStateDic(MASTATE_TYPE.CAT_DINNER, new CatDinnerState());
        
        roundSM.AddStateDic(MASTATE_TYPE.BIRD_WAIT, new BirdWaitState());
        roundSM.AddStateDic(MASTATE_TYPE.BIRD_MORNING, new BirdMorningState());
        roundSM.AddStateDic(MASTATE_TYPE.BIRD_MORNING2, new BirdMorning2State());
        roundSM.AddStateDic(MASTATE_TYPE.BIRD_AFTERNOON, new BirdAfternoonState());
        roundSM.AddStateDic(MASTATE_TYPE.BIRD_DINNER, new BirdDinnerState());

        roundSM.AddStateDic(MASTATE_TYPE.WOOD_WAIT, new WoodWaitState());
        roundSM.AddStateDic(MASTATE_TYPE.WOOD_MORNING1, new WoodMorning1State());
        roundSM.AddStateDic(MASTATE_TYPE.WOOD_MORNING2, new WoodMorning2State());
        roundSM.AddStateDic(MASTATE_TYPE.WOOD_AFTERNOON, new WoodAfternoonState());
        roundSM.AddStateDic(MASTATE_TYPE.WOOD_DINNER, new WoodDinnerState());

        roundSM.SetState(MASTATE_TYPE.CAT_WAIT);

    }
    private void Update()
    {
        //상태가 변화되면 상태머신의 update가 실행된 상태.
        roundSM.Update();
    }
    public void SetEffect(NodeMember mem)
    {
        mem.transform.GetChild(0).GetComponent<Effect>().gameObject.SetActive(true);
    }
    public void SetOffAllEffect()
    {
        foreach (GameObject effect in particles)
        {
            effect.SetActive(false);
        }
    }
}

