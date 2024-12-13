using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBase<T> : MonoBehaviour where T:MonoBehaviour
{
    private static  T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;
            }
            
            return _instance;
        }   
    }

    protected virtual void Awake()
    {
        _instance = this as T;
    }
}
