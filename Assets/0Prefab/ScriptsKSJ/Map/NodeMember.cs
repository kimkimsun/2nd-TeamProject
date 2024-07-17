using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInterface;

public class NodeMember : MonoBehaviour
{
    public Node node;
    public string nodeName;
    public ANIMAL_COST_TYPE nodeType;
    public bool isTileCheck;

    void Awake()
    {
        node = new Node(nodeName);
        //Debug.Log(nodeName + "생성");
    }
    void tempdd()
    {
        

    }
}
