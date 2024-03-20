# 두 번째 팀프로젝트
> ### **개발 환경**

![Windows](https://img.shields.io/badge/Windows-0078D6?style=for-the-badge&logo=windows&logoColor=white)
![Unity](https://img.shields.io/badge/unity-%23000000.svg?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
![Visual Studio](https://img.shields.io/badge/Visual%20Studio-5C2D91.svg?style=for-the-badge&logo=visual-studio&logoColor=white)
![Discord](https://img.shields.io/badge/Discord-%235865F2.svg?style=for-the-badge&logo=discord&logoColor=white)
![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)
# 프로젝트 특징
#### 조장이 아닌 역할로 참여하게 된 첫 보드게임 원작의 2D게임 입니다.
# 게임 특징
![루트 원작 게임](https://github.com/kimkimsun/2nd-TeamProject/assets/116052108/0cf393ee-9336-4019-931a-d33a6da45e28)
#### 최초의 원작은 보드게임이며, 스팀에도 출시 되어있는 Root라는 게임을 모작으로 만든 모작게임입니다.
#### 종족별로 특징이 명확히 다르며, 제가 맡은 종족의 특징을 설명 드리겠습니다.
#### 1. 시작 시, 고양이 후작이라는 종족이 하나의 땅을 고르면 나머지 땅에 병사들을 1마리씩 배치하고 본부는 가장 먼 거리에 지어집니다.
#### 2. 할 수 있는 행동으로는 이동, 모병, 배틀, 건설이 있습니다. 턴 한번에 2가지의 행동을 취할 수 있습니다.
#### 3. 매번 시작 시, 왕을 선택할 수 있습니다. 왕은 종류에 따라 행동양식을 유연하게 해줍니다.
#### 4. 하지만 왕이 가지고 있는 특징을 이루지 못했을 경우, 왕권은 물러나게되고 새로운 왕을 뽑아 전략적으로 플레이해야 합니다.
# **핵심 기능**
## 첫 번째 기능
### 기능설명
1. 카드를 드래그해서 사용할 수 있습니다.
2. 하지만 마우스의 위치가 카드덱을 넘지 않거나, 카드를 사용할 조건을 충족하지 못하였을 경우엔 사용이 불가능합니다.
3. 이는 그래픽 레이캐스터를 활용하여 만들었습니다.

### 코드
```C#
public void OnDrop(PointerEventData eventData)
{
    results = new List<RaycastResult>();
    ped.position = Input.mousePosition;
    gr.Raycast(ped, results);
    if (results.Count <= 0 || results[1].gameObject == null)
        return;
    if (results[1].gameObject.name == "Frame")
        isMove = true;
    else
        isMove = false;
    if (eventData.pointerDrag != null)
    {
        if (isMove)
        {
            if (RoundManager.Instance.bird.inputCard < 2)
            {
                results[1].gameObject.transform.GetChild(0).GetComponent<BirdCardAction>().
                AddCard(eventData.pointerDrag.GetComponentInParent<Slot>().card);
                eventData.pointerDrag.GetComponentInParent<Slot>().UseCard();
            }
            else
                return;
        }
        else if (Uimanager.Instance.woodUi.cardUseType != WoodUi.CardUseType.BATTLE)
        {
            eventData.pointerDrag.GetComponentInParent<Slot>().UseCard();
        }
        cardInfoWindow.gameObject.SetActive(false);
    }
}
```
##### bool 타입인 isMove가 True가 되는 조건은 화면상에 정확히 카드를 놓았나 안놓았나의 조건입니다. 이는 Graphic Raycaster의 결과 값으로 확인하였습니다.
##### 카드가 사용되는 조건은 Slot.UseCarD() 부분이며, 만약 조건이 충족되지 않았을 시에는 사용이 불가능하게 하였습니다.

## 두 번째 기능
### 기능 설명
1. 카드를 사용할 시에는 많은 제약사항이 존재합니다.
2. 예를 들면, 토끼동물이 그려진 카드를 사용하고 싶을 땐, 토끼 타일에 내 병사가 위치해야됩니다.
3. 또한 카드는 동물타입 이외에도 전투시에 사용하는 것과 점수를 획득할 때 사용하는 제작카드라는 것들이 존재합니다.
4. 이들은 행위를 클래스로 캡슐화하여 동적으로 행위를 자유롭게 해주는 패턴인 전략 패턴을 사용해 좀 더 코드를 용이하게 구현하였습니다.

### 사진 설명
![두 번째 팀프 사진](https://github.com/kimkimsun/2nd-TeamProject/assets/116052108/144a27f5-48df-4d33-ae87-50a7becf774e)

### 다이어그램
![두 번째 팀프 다이어그램](https://github.com/kimkimsun/2nd-TeamProject/assets/116052108/b0107f14-89af-4612-9e77-7f65c6c0fe4b)
##### 위와 같은 구조로 이루어져있습니다.
##### 이 외에도 전투 시에 사용되는 시네머신과 이어리왕조라는 종족 중 하나를 구현하는 등의 기능들을 구현하였습니다.
