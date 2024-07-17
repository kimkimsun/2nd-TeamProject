using CustomInterface;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<Slot> slot = new List<Slot>();
    public Transform parentTransfom;
    private void Awake()
    {
        slot = parentTransfom.GetComponentsInChildren<Slot>().ToList();
    }
    public void AddCard(Card card)
    {
        for (int i = 0; i < slot.Count; i++)
        {
            if (slot[i].card == null)
            {
                slot[i].SetItem(card);
                slot[i].gameObject.SetActive(true);
                card.onActive = null;
                card.onActive += () => {
                    if (card.isUse == true)
                    {
                        Debug.Log(i+"번쨰 비움");
                        slot[i].EmptySlot();
                    }
                };
                return;
            }
        }
    }
    public void SetSort()
    {
        int tempCardNum = 0;
        foreach (KeyValuePair<ANIMAL_COST_TYPE, List<Card>> kv in RoundManager.Instance.nowPlayer.cardDecks)
        {
            tempCardNum += kv.Value.Count;
        }
        Debug.Log("남은카드" + tempCardNum);
        for (int j = 0; j < tempCardNum; j++) //카드 갯수만큼
        {
            int a = j; // 0
            int next = j + 1;
            Debug.Log("도는거 체크" + j);
            if (slot[j].card == null) 
            {

                slot[j].card = slot[next].card;
                slot[j].image.sprite = slot[next].image.sprite;
                slot[next].card = null;
                slot[next].image.sprite = null;

                Debug.Log(a);
                slot[a].card.onActive = null;
                Debug.Log(a + "왜");
                slot[a].card.onActive += () =>
                {
                    Debug.Log(a + "번쨰 비움");
                    if (slot[a].card.isUse == true)
                    {
                        slot[a].EmptySlot();
                    }
                };

                Debug.Log(j);


            }
        }

        if (slot[tempCardNum].card == null)
        {
            slot[tempCardNum].gameObject.SetActive(false);
        }
        
    }
}