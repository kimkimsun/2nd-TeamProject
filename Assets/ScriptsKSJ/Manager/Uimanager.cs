using CustomInterface;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Uimanager : SingleTon<Uimanager>
{

    public Button testBtn; // 임시 테스트버튼

    // 현재 플레이어 턴 텍스트
    [Header("[플레이어 UI]")]
    public PlayerUI playerUI;

    public CatUI catUI;
    public BirdUI birdUI;
    public WoodUi woodUi;
    public DropableUI dropableUI;
    public ScoreBoard scoreBoard;

    [Header("[카드 정보창]")]

    public GameObject cardWindow;

    public TextMeshProUGUI cardName;

    public TextMeshProUGUI cardInfo;
    [Header("[인벤토리 UI]")]
    public GameObject catInven;
    public GameObject birdInven;
    public GameObject woodInven;

    public BattleP1 battlep1;
    public BattleP2 battlep2;
    private new void Awake()
    {
        base.Awake();
    }


}
