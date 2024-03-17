using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST2 : MonoBehaviour
{
    public List<Transform> list = new List<Transform>();
    // Start is called before the first frame update
    public void SetPathes(Queue<Vector3> temp)
    {
        foreach(Transform t in list)
        {
            temp.Enqueue(t.position);
        }
    }
}
