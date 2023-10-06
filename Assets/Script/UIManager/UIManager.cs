using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EUILayerType
{
    Bottom,
    Middle,
    Top,
    System
}

public class UIManager
{
    private static UIManager instance = new UIManager();
    public static UIManager Instance => instance;

    private Dictionary<string, BasePanel> panelsDic = new Dictionary<string, BasePanel>();
    private Canvas panelCanvas;
    // 各层面板
    private Transform bottom;
    private Transform middle;
    private Transform top;
    private Transform system;

    private UIManager()
    {
        // 获取canvas
        panelCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        if (!panelCanvas)
        {
            panelCanvas = GameObject.Instantiate(Resources.Load<GameObject>("UI/Canvas")).GetComponent<Canvas>();
        }
        // 获取各层
        bottom = panelCanvas.transform.Find("Bottom");
        if (!bottom)
        {
            bottom = new GameObject("Bottom").transform;
            bottom.SetParent(panelCanvas.transform);
        }
        
        middle =  panelCanvas.transform.Find("Middle");
        if (!middle)
        {
            middle = new GameObject("Middle").transform;
            middle.SetParent(panelCanvas.transform);
        }
        
        top = panelCanvas.transform.Find("Top");
        if (!top)
        {
            top = new GameObject("Top").transform;
            top.SetParent(panelCanvas.transform);
        }
        
        system = panelCanvas.transform.Find("System");
        if (!system)
        {
            system = new GameObject("System").transform;
            system.SetParent(panelCanvas.transform);
        }
        
        GameObject.DontDestroyOnLoad(panelCanvas);
    }

    public T Show<T>(EUILayerType layerType= EUILayerType.Bottom, bool isFade = true, UnityAction callBack = null) where T : BasePanel
    {
        // 获取类名与预设体同名
        string panelName = typeof(T).Name;

        // 存在面板直接取出
        if (panelsDic.TryGetValue(panelName, out var value))
        {
            value.Show(isFade, callBack);
            return value as T;
        }

        // 不存在直接创建并保存
        T newPanel = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName), panelCanvas.transform).GetComponent<T>();
        panelsDic.Add(panelName, newPanel);
        newPanel.Show(isFade, callBack);
        
        // 设置层级
        switch (layerType)
        {
            case EUILayerType.Bottom:
                newPanel.transform.SetParent(bottom);
                break;
            case EUILayerType.Middle:
                newPanel.transform.SetParent(middle);
                break;
            case EUILayerType.Top:
                newPanel.transform.SetParent(top);
                break;
            case EUILayerType.System:
                newPanel.transform.SetParent(system);
                break;
        }
        
        return newPanel;
    }

    public void Hide<T>(bool isFade =true, UnityAction callBack = null) where T : BasePanel
    {
        string panelName = typeof(T).Name;

        if (panelsDic.TryGetValue(panelName, out BasePanel panel))
        {
            if (isFade)
            {
                callBack += () => { GameObject.Destroy(panel.gameObject); };
                // 淡出
                panel.Hide(callBack);
                // 删除面板
                panelsDic.Remove(panelName);
            }
            else
            {
                // 直接删除面板
                panelsDic.Remove(panelName);
                GameObject.Destroy(panel.gameObject);
                // 直接执行回调
                callBack?.Invoke();
            }
        }
    }

    public T GetPanel<T>() where T : BasePanel
    {
        string panelName = typeof(T).Name;

        if (panelsDic.TryGetValue(panelName, out BasePanel panel))
        {
            return panel as T;
        }

        return null;
    }
}