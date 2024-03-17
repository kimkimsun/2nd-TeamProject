using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ChoicePlayer : MonoBehaviour
{
    [Header("플레이어")]
    public GameObject cat;
    public GameObject bird;
    public GameObject wood;

    //public TextMeshPro playerName;
    public TextMeshProUGUI characterNameText;

    [Header("플레이어 선택버튼")]
    public Button left;
    public Button right;

    [SerializeField]
    int playernumber;
    public int Playernumber
    {
        get { return playernumber; } 
        set
        {
            playernumber = value;
            if(playernumber < 0)
                playernumber = 2;
            else if (playernumber > 2)
                playernumber = 0;        
        }
    }
    

    List<GameObject> characterOBJ;
    List<string> characterName;

    private void Start()
    {
        playernumber = 0;
        characterOBJ = new List<GameObject>();
        characterName = new List<string>();

        characterOBJ.Add(cat);
        characterOBJ.Add(bird);
        characterOBJ.Add(wood);

        characterName.Add("cat");
        characterName.Add("bird");
        characterName.Add("wood");

        characterOBJ[Playernumber].SetActive(true);
        characterNameText.text = characterName[Playernumber].ToString();

        right.onClick.AddListener(NextPlayer);
        left.onClick.AddListener(PrevPlayer);
    }

    void NextPlayer()
    {
        characterOBJ[Playernumber].SetActive(false);
        Playernumber++;
        characterOBJ[Playernumber].SetActive(true);
        characterNameText.text = characterName[Playernumber].ToString();
    }

    void PrevPlayer()
    {
        characterOBJ[Playernumber].SetActive(false);
        Playernumber--;
        characterOBJ[Playernumber].SetActive(true);
        characterNameText.text = characterName[Playernumber].ToString();
    }


}
