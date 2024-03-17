using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



namespace sihyeon
{
    interface IDestroyAble
    {
        public void Destroy(Building targetBuilding);
    }

    public enum Building_STATE
    {
        DESTROY,
        NONE
    }
    public enum Building_TYPE
    {
        CAT_SAWMILL,
        CAT_BARRACKS,
        CAT_WORKSHOP,
        WOOD_TOKEN,
        WOOD_FOX,
        WOOD_RAT,
        WOOD_RABBIT,
        BIRD_NEST,
        CAT_BASE
    }


    //문제 1
    //1. 일단 건물이 건설된다 - ㅇ
    //2. 그런데 이게 누구의 건물이고 어디에 건설된지 판별을 하고있는가 x
    //3. 이걸 해결하고 건설을 다시 수정한다 

    //문제 2
    //1. 클릭한곳의 노드정보를 가져온다 (아마 ray를 이용해서 해결하면될것)
    //2. 정보를 가져왔으면 그곳의 tile의 position을 가져와 건설하고 
    //3. 임의의 리시트 or 딕셔너리등 아무튼 저장해 위와 연계해 그곳의 건물의 소유주를
    //4. 설정한다


    //문제 3
    //건물 파괴와 관련된것.
    //임시로 일단은 건물의 상태를 Destroy로 바꾸고 임시로 로그를 찍음.. 12.11 -시현


    //해결책?
    // 일단은 건물의 스크립트를 가진 게임오브젝트 자체를 전달하는쪽으로. 12-11 시현

    public class Building : MonoBehaviour
    {
        //public playerState state;
        public event Action onDestroy;
        public Building_TYPE type;
        public Building_STATE buildingState = Building_STATE.NONE;
        public int cost;


        public void Build()
        {
            Debug.Log("testBuild");
        }

        public void Destroy()
        {
            onDestroy();
            Debug.Log("파괴됨");
        }
    }
}
