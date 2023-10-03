using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 手动mono单例要拖动到对象上
/// </summary>
public class BaseMonoSingleton<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T instance;
    
    public static T Instance => instance;
    
    protected virtual void Awake()
    {
        instance = this as T;
    }
}
