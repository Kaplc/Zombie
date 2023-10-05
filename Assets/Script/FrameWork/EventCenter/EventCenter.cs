using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 空接口装子类
public interface IEventInfo
{
    
}

public class EventInfo: IEventInfo
{
    public UnityAction action;
    
    // 构造函数方便外部初始化直接添加事件
    public EventInfo(UnityAction newAction)
    {
        action += newAction;
    }
}

public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> action;

    public EventInfo(UnityAction<T> newAction)
    {
        action += newAction;
    }
    
}


public class EventCenter : BaseSingleton<EventCenter>
{
    // 接口父类装子类, 方便带泛型事件
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();
    
    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="eventName">事件名字</param>
    /// <param name="newAction">新的事件</param>
    public void AddEventListener(string eventName, UnityAction newAction)
    {
        // 查找有无对应事件, 无则创建
        if (eventDic.ContainsKey(eventName))
        {
            ((EventInfo)eventDic[eventName]).action += newAction;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo(newAction));
        }
    }
    
    /// <summary>
    /// 带泛型添加事件监听
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="newAction"></param>
    /// <typeparam name="T"></typeparam>
    public void AddEventListener<T>(string eventName, UnityAction<T> newAction)
    {
        // 查找有无对应事件, 无则创建
        if (eventDic.ContainsKey(eventName))
        {
            ((EventInfo<T>)eventDic[eventName]).action += newAction;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T>(newAction));
        }
    }
    
    public void RemoveEventListener(string eventName, UnityAction oldAction)
    {
        if (eventDic.ContainsKey(eventName))
        {
            ((EventInfo)eventDic[eventName]).action -= oldAction;
        }
    }
    
    public void RemoveEventListener<T>(string eventName, UnityAction<T> oldAction)
    {
        if (eventDic.ContainsKey(eventName))
        {
            ((EventInfo<T>)eventDic[eventName]).action -= oldAction;
        }
    }
    
    // 触发事件执行
    public void TriggerEvent(string eventName)
    {
        if (eventDic.ContainsKey(eventName))
        {
            ((EventInfo)eventDic[eventName]).action?.Invoke();
        }
    }
    
    public void TriggerEvent<T>(string eventName, T parameter)
    {
        if (eventDic.ContainsKey(eventName))
        {
            // 带参数触发
            ((EventInfo<T>)eventDic[eventName]).action?.Invoke(parameter);
        }
    }
    
    // 清空该事件所有listener
    public void RemoveAllListener(string eventName)
    {
        if (eventDic.ContainsKey(eventName))
        {
            eventDic[eventName] = null;
        }
    }
    
    // 清空所有事件
    public void ClearAllEvent()
    {
        eventDic.Clear();
    }
}
