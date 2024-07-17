using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PriorityQueue<TElement, TPriority>
{
    private struct Node
    {
        public TElement Element;
        public TPriority Priority;
    }

    private List<Node> nodes;
    private IComparer<TPriority> comparer;

    public PriorityQueue()
    {
        this.nodes = new List<Node>();
        this.comparer = Comparer<TPriority>.Default;
    }

    public PriorityQueue(IComparer<TPriority> comparer)
    {
        this.nodes = new List<Node>();
        this.comparer = comparer;
    }

    public int Count { get { return nodes.Count; } }
    public IComparer<TPriority> Comparer { get { return comparer; } }

    public void Enqueue(TElement element, TPriority priority)
    {
        Node newNode = new Node() { Element = element, Priority = priority };

        PushHeap(newNode);
    }

    public TElement Peek()
    {
        if (nodes.Count == 0)
            throw new InvalidOperationException();

        return nodes[0].Element;
    }

    public bool TryPeek(out TElement element, out TPriority priority)
    {
        if (nodes.Count == 0)
        {
            element = default(TElement);
            priority = default(TPriority);
            return false;
        }

        element = nodes[0].Element;
        priority = nodes[0].Priority;
        return true;
    }

    public TElement Dequeue()
    {
        if (nodes.Count == 0)
            throw new InvalidOperationException();

        Node rootNode = nodes[0];
        PopHeap();
        return rootNode.Element;
    }

    public bool TryDequeue(out TElement element, out TPriority priority)
    {
        if (nodes.Count == 0)
        {
            element = default(TElement);
            priority = default(TPriority);
            return false;
        }

        Node rootNode = nodes[0];
        element = rootNode.Element;
        priority = rootNode.Priority;
        PopHeap();
        return true;
    }

    private void PushHeap(Node newNode)
    {
        nodes.Add(newNode);
        int newNodeIndex = nodes.Count - 1;
        while (newNodeIndex > 0)
        {
            int parentIndex = GetParentIndex(newNodeIndex);
            Node parentNode = nodes[parentIndex];

            if (comparer.Compare(newNode.Priority, parentNode.Priority) < 0)
            {
                nodes[newNodeIndex] = parentNode;
                newNodeIndex = parentIndex;
            }
            else
            {
                break;
            }
        }
        nodes[newNodeIndex] = newNode;
    }

    private void PopHeap()
    {
        Node lastNode = nodes[nodes.Count - 1];
        nodes.RemoveAt(nodes.Count - 1);

        int index = 0;
        while (index < nodes.Count)
        {
            int leftChildIndex = GetLeftChildIndex(index);
            int rightChildIndex = GetRightChildIndex(index);

            if (rightChildIndex < nodes.Count)
            {
                int compareIndex = comparer.Compare(nodes[leftChildIndex].Priority, nodes[rightChildIndex].Priority) < 0 ?
                leftChildIndex : rightChildIndex;

                if (comparer.Compare(nodes[compareIndex].Priority, lastNode.Priority) < 0)
                {
                    nodes[index] = nodes[compareIndex];
                    index = compareIndex;
                }
                else
                {
                    nodes[index] = lastNode;
                    break;
                }
            }
            else if (leftChildIndex < nodes.Count)
            {
                if (comparer.Compare(nodes[leftChildIndex].Priority, lastNode.Priority) < 0)
                {
                    nodes[index] = nodes[leftChildIndex];
                    index = leftChildIndex;
                }
                else
                {
                    nodes[index] = lastNode;
                    break;
                }
            }
            else
            {
                nodes[index] = lastNode;
                break;
            }
        }
    }

    private int GetParentIndex(int childIndex)
    {
        return (childIndex - 1) / 2;
    }

    private int GetLeftChildIndex(int parentIndex)
    {
        return parentIndex * 2 + 1;
    }

    private int GetRightChildIndex(int parentIndex)
    {
        return parentIndex * 2 + 2;
    }
}

public class Node
{
    public string name;
    public Node parent;

    public List<Edge> edgesInNode;
    public Node(string name)
    {
        this.name = name;
        edgesInNode = new List<Edge>();
        parent = null;
    }
}
public class Edge : IEquatable<Edge>, IComparable<Edge>
{
    public Node sNode;
    public Node eNode;
    public int cost;
    public Edge(Node sNode, Node eNode, int cost)
    {
        this.sNode = sNode;
        this.eNode = eNode;
        this.cost = cost;
    }

    public int CompareTo(Edge other)
    {
        return cost.CompareTo(other.cost);
    }

    public bool Equals(Edge other)
    {
        return cost.Equals(other.cost);
    }
}
public class Graph
{
    public List<Node> nodeList;
    public List<Edge> edges;
    public Dictionary<string, bool> hasNodeCheckDic;

    public Graph()
    {
        nodeList = new List<Node>();
        edges = new List<Edge>();
        hasNodeCheckDic = new Dictionary<string, bool>();
    }
    public void AddEdge(Node sNode, Node eNode, int cost)
    {
        //Debug.Log(sNode.name + " " + eNode.name);
        TryAddNode(sNode);
        TryAddNode(eNode);
        Edge temp = new Edge(sNode, eNode, cost);
        edges.Add(temp);
        sNode.edgesInNode.Add(temp);
        eNode.edgesInNode.Add(temp);
    }

    bool TryAddNode(Node node)
    {
        //Debug.Log(node.name+"ds");
        if (hasNodeCheckDic.ContainsKey(node.name) == false)
        {
            hasNodeCheckDic[node.name] = true;
            nodeList.Add(node);
            return true;
        }
        return false;
    }

}
public class AsAlgo
{
    Dictionary<string, int> roadCostDic;//코스트 저장해서 계산하는곳
    Dictionary<string, bool> visited;//방문했는지 체크하는거

    public AsAlgo()
    {
        roadCostDic = new Dictionary<string, int>();
        visited = new Dictionary<string, bool>();
    }
    public List<string> FindAs(Graph graph, string startNodeName, string endNodeName)
    {
        roadCostDic.Clear();
        visited.Clear();

        List<string> returnList = new List<string>();

        PriorityQueue<Node, int> visitPQ = new PriorityQueue<Node, int>();

        Node startNode = graph.nodeList.Find(node => node.name == startNodeName);
        Node endNode = graph.nodeList.Find(node => node.name == endNodeName);
        foreach (Node node in graph.nodeList)
        {
            roadCostDic.Add(node.name, 999);
            visited.Add(node.name, false);
        }
        roadCostDic[startNodeName] = 0;
        visited[startNodeName] = true;

        visitPQ.Enqueue(startNode, roadCostDic[startNodeName]);
        while (visitPQ.Count > 0)
        {
            //방문할 정점(노드)를 꺼냄, 우선순위 큐이므로 비용이 낮은거부터 꺼냄
            Node curVisitNode = visitPQ.Dequeue();

            visited[curVisitNode.name] = true;
            if (curVisitNode.name == endNodeName)
            {
                string tempName = endNodeName;
                while (tempName != startNodeName)
                {
                    returnList.Add(tempName);
                    tempName = graph.nodeList.Find(node => node.name == tempName).parent.name;
                }
                returnList.Add(startNodeName);
                returnList.Reverse();
                return returnList;
            }
            for (int i = 0; i < curVisitNode.edgesInNode.Count; i++)
            {
                Edge nowEdge = curVisitNode.edgesInNode[i];
                Node nowNode;
                if (curVisitNode.edgesInNode[i].sNode != curVisitNode)
                {
                    Node tempNode = curVisitNode.edgesInNode[i].sNode;
                    curVisitNode.edgesInNode[i].sNode = curVisitNode.edgesInNode[i].eNode;
                    curVisitNode.edgesInNode[i].eNode = tempNode;

                }
                nowNode = curVisitNode.edgesInNode[i].eNode;

                if (visited[nowNode.name])
                {
                    continue;
                }
                int cost = roadCostDic[curVisitNode.name] + nowEdge.cost;
                if (roadCostDic[nowNode.name] > cost)
                {
                    nowNode.parent = curVisitNode;
                    roadCostDic[nowNode.name] = cost;
                    visitPQ.Enqueue(nowNode, roadCostDic[nowNode.name]);
                }
            }
        }

        return returnList;
    }

}
public class MapExtra : MonoBehaviour
{
    [SerializeField]
    GameObject tileParent;//타일들 부모

    public List<NodeMember> mapTiles = new List<NodeMember>();
    public Dictionary<string, Node> nodeDic;
    public List<string> nodeNameList = new List<string>();

    public Graph graph;
    public AsAlgo asAlgo;
    void Awake()
    {
        graph = new Graph();
        asAlgo = new AsAlgo();
        mapTiles = tileParent.GetComponentsInChildren<NodeMember>().ToList();

        nodeDic = new Dictionary<string, Node>();

        int temp = 0;

        foreach (NodeMember nodeMem in mapTiles)
        {
            temp++;
            nodeDic.Add(nodeMem.nodeName, nodeMem.node);
            nodeNameList.Add(nodeMem.nodeName);
        }
        graph.AddEdge(nodeDic[nodeNameList[0]], nodeDic[nodeNameList[1]], 1);
        graph.AddEdge(nodeDic[nodeNameList[0]], nodeDic[nodeNameList[3]], 1);//여우2
        graph.AddEdge(nodeDic[nodeNameList[3]], nodeDic[nodeNameList[6]], 1);
        graph.AddEdge(nodeDic[nodeNameList[0]], nodeDic[nodeNameList[4]], 1);//생쥐2
        graph.AddEdge(nodeDic[nodeNameList[1]], nodeDic[nodeNameList[2]], 1);//생쥐1
        graph.AddEdge(nodeDic[nodeNameList[1]], nodeDic[nodeNameList[3]], 1);//여우2
        graph.AddEdge(nodeDic[nodeNameList[2]], nodeDic[nodeNameList[3]], 1);//여우2
        graph.AddEdge(nodeDic[nodeNameList[2]], nodeDic[nodeNameList[7]], 1);//토끼3
        graph.AddEdge(nodeDic[nodeNameList[4]], nodeDic[nodeNameList[5]], 1);//토끼2
        graph.AddEdge(nodeDic[nodeNameList[4]], nodeDic[nodeNameList[8]], 1);
        graph.AddEdge(nodeDic[nodeNameList[5]], nodeDic[nodeNameList[6]], 1);
        graph.AddEdge(nodeDic[nodeNameList[5]], nodeDic[nodeNameList[8]], 1);
        graph.AddEdge(nodeDic[nodeNameList[6]], nodeDic[nodeNameList[7]], 1);
        graph.AddEdge(nodeDic[nodeNameList[6]], nodeDic[nodeNameList[9]], 1);
        graph.AddEdge(nodeDic[nodeNameList[8]], nodeDic[nodeNameList[9]], 1);
        graph.AddEdge(nodeDic[nodeNameList[9]], nodeDic[nodeNameList[10]], 1);
        graph.AddEdge(nodeDic[nodeNameList[11]], nodeDic[nodeNameList[7]], 1);
        graph.AddEdge(nodeDic[nodeNameList[11]], nodeDic[nodeNameList[6]], 1);
        graph.AddEdge(nodeDic[nodeNameList[11]], nodeDic[nodeNameList[10]], 1);//19
        //길들 저장한거

    }
    public List<string> SetAl(string a, string b)//a->b의 최단거리 계산하고 넣는 함수(호출용)
    {
        List<string> tempStrings = new List<string>();
        tempStrings = asAlgo.FindAs(graph, a, b);
        Debug.Log("이동 횟수" + (int)(tempStrings.Count - 1));
        foreach (string tempString in tempStrings)
        {
            Debug.Log(tempString);
        }
        return tempStrings;
    }
    public Dictionary<string, int> SetCostMove(string a, int cost)
    {
        Dictionary<string,int> costDic = new Dictionary<string,int>();
        List<NodeMember> tempNodes = new List<NodeMember>();
        foreach(NodeMember node in mapTiles)
        {
            if(node.nodeName != a)
            {
                tempNodes.Add(node);
            }
        }
        int tempCost = 0;
        foreach (NodeMember tempNode in tempNodes)
        {
            tempCost = SetAl(a, tempNode.nodeName).Count - 1;
            Debug.Log(tempCost);
            if(tempCost <= cost)
            {
                costDic.Add(tempNode.nodeName, tempCost);
            }            
        }
        Debug.Log(costDic.Count);
        return costDic;
    }
}
