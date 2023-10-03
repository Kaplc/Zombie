using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMonoAutoSingleton<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T instance;
    
    // 仅第一次使用才会new
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // 第一次调用创建空物体并命名
                GameObject gameObject = new GameObject(typeof(T).Name);
                // 防止切场景销毁
                DontDestroyOnLoad(gameObject);
                // 将该单例脚本添加到物体上面, 并返回该脚本
                instance = gameObject.AddComponent<T>();
            }
            return instance;
        }
    }
}
