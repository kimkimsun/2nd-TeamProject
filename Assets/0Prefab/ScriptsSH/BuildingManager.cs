using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomInterface;


namespace sihyeon
{
    public class BuildingManager : SingleTon<BuildingManager>
    {
        public Dictionary<Building_TYPE, GameObject> BuildingDics = new Dictionary<Building_TYPE, GameObject>();
        public GameObject selectedBuilding;//원하는건물
        public NodeMember catBaseNode;// 테스트용 고양이 기지 노드 12 -13 시현추가
        //빌딩의 딕셔너리, 건설할수있는 건물들
        [Header("테스트용 버튼들")]

        public Button buildCatABtn;
        public Button buildCatBBtn;
        public Button buildCatCBtn;
        public Button buildWoodBaseBtn;
        public Button buildBirdNestBtn;


        [Header("리스트")]

        public List<Building> buildingList;
        //건설된 건물의 리스트.

        [Header("테스트용 게임오브젝트aka 건물")]
        public GameObject catBasePrefab;
        public GameObject catSawMillPrefab;
        public GameObject catBarrackPrefab;
        public GameObject catWorkShopPrefab;
        public GameObject birdNestPrefab;
        public GameObject woodTokenPrefab;
        public GameObject woodFoxBasePrefab;
        public GameObject woodRabbitBasePrefab;
        public GameObject woodRatBasePrefab;
        private void Start()
        {
            buildingList = new List<Building>();
            setBuilding();        
        }
        private new void Awake()
        {
            base.Awake();
        }
        public void buildBuilding(Building building)
        {
            Debug.Log(building.type + "건설test");
        }

        public void setBuilding()
        {

            BuildingDics.Add(Building_TYPE.CAT_SAWMILL, catSawMillPrefab);
            BuildingDics.Add(Building_TYPE.CAT_BARRACKS, catBarrackPrefab);
            BuildingDics.Add(Building_TYPE.CAT_WORKSHOP, catWorkShopPrefab);
            BuildingDics.Add(Building_TYPE.WOOD_TOKEN, woodTokenPrefab);
            BuildingDics.Add(Building_TYPE.WOOD_FOX, woodFoxBasePrefab);
            BuildingDics.Add(Building_TYPE.WOOD_RAT, woodRatBasePrefab);
            BuildingDics.Add(Building_TYPE.WOOD_RABBIT, woodRabbitBasePrefab);
            BuildingDics.Add(Building_TYPE.BIRD_NEST, birdNestPrefab);
            BuildingDics.Add(Building_TYPE.CAT_BASE, catBasePrefab);
        }
        public void SetWoodBase(NodeMember node)
        {
            switch (node.nodeType)
            {
                case ANIMAL_COST_TYPE.FOX:
                    selectedBuilding = BuildingDics[Building_TYPE.WOOD_FOX];
                    break;
                case ANIMAL_COST_TYPE.RAT:
                    selectedBuilding = BuildingDics[Building_TYPE.WOOD_RAT];
                    break;
                case ANIMAL_COST_TYPE.RABBIT:
                    selectedBuilding = BuildingDics[Building_TYPE.WOOD_RABBIT];
                    break;
            }
        }
        //테스트용 버튼 // 
        /*public void SetBtnTest()
        {
            buildCatABtn.onClick.RemoveAllListeners();
            buildCatABtn.onClick.AddListener(() => {
                //buildBuilding(BuildingDics["catSawmill"]);
                //TestSpawnBuilding(BuildingDics["catSawmill"]);
                //buildingList.Add(BuildingDics["catSawmill"]);
            });
        }
        */
        public GameObject InstantiateBuilding(GameObject building, Transform transform)
        {
            NodeMember node = RoundManager.Instance.mapController.nowTile;

            GameObject buildingPrefab = Instantiate(building, transform.position +
                new Vector3(0, 0.4f, 0), Quaternion.identity);
            return buildingPrefab;
        }

    }
}
