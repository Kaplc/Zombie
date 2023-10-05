using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

public class MonoManager : BaseMonoAutoSingleton<MonoManager>
{
    public UnityAction action;
    
    void Update()
    {
        // 一直执行事件
        action?.Invoke();
    }
    
    // 添加事件
    public void AddUpdateEvent(UnityAction newEvent)
    {
        action += newEvent;
    }
    // 移除事件
    public void RemoveUpdateEvent(UnityAction oldEvent)
    {
        action -= oldEvent;
    }
    
    // 协程开启
    public Coroutine StartCoroutineFrameWork(string methodName)
    {
        return StartCoroutine(methodName);
    }

    public Coroutine StartCoroutineFrameWork(string methodName, [DefaultValue("null")] object value)
    {
        return StartCoroutine(methodName,value);
    }

    public Coroutine StartCoroutineFrameWork(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }
    // 协程关闭
    public void StopCoroutineFrameWork(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }

    public void StopCoroutineFrameWork(IEnumerator routine)
    {
        StopCoroutine(routine);
    }
}
