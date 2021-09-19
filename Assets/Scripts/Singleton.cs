using UnityEngine;

public class Singleton<T>:MonoBehaviour
where T:Singleton<T>
{
    public static T instance{get;private set;}

    protected virtual void Awake()
    {
        CreateInstance();
    }

    void CreateInstance()
    {
        if(instance == null)
        {
            instance = this as T;
        }
        else
        {
            Debug.LogError(typeof(T).Name+"is singleton!! breaking");
            Destroy(this);
        }
    }
    
}