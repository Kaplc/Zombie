using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSingleton<T> where T : class, new()
{
    private static T instance;
    
    // 仅第一次使用才会new
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }
    /// <summary>
    /// 销毁单例
    /// </summary>
    private void DestroySingle()
    {
        instance = null;
    }
}
