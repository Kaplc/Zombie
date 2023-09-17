using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager
{
    private static UIManager instance = new UIManager();
    public static UIManager Instance => instance;

    private Dictionary<string, BasePanel> panelsDic = new Dictionary<string, BasePanel>();
    private Canvas panelCanvas;

    private UIManager()
    {
        panelCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        GameObject.DontDestroyOnLoad(panelCanvas);
    }

    public T Show<T>() where T : BasePanel
    {
        // 获取类名与预设体同名
        string panelName = typeof(T).Name;

        // 存在面板直接取出
        if (panelsDic.TryGetValue(panelName, out var value))
        {
            value.Show();
            return value as T;
        }

        // 不存在直接创建并保存
        T newPanel = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName), panelCanvas.transform).GetComponent<T>();
        panelsDic.Add(panelName, newPanel);
        newPanel.Show();
        return newPanel;
    }

    public void Hide<T>(bool isFade, UnityAction callBack) where T : BasePanel
    {
        string panelName = typeof(T).Name;

        if (panelsDic.TryGetValue(panelName, out BasePanel panel))
        {
            if (isFade)
            {
                // 淡出
                panel.Hide(callBack);
                // 删除面板
                panelsDic.Remove(panelName);
                GameObject.Destroy(panel.gameObject, 2);
            }
            else
            {
                // 删除面板
                panelsDic.Remove(panelName);
                GameObject.Destroy(panel.gameObject, 2);
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