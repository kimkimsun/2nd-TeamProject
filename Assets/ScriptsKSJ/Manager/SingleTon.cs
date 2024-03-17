using UnityEngine;

public class SingleTon<T> : MonoBehaviour where T : SingleTon<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected void Awake()
    {
        if(instance == null)
        {
            instance = (T)this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

    }
}
