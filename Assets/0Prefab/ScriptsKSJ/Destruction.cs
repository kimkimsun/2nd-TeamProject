using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyThis", 1f);
    }
    private void DestroyThis()
    {
        Destroy(gameObject);
    }
}
