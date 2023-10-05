using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManger : BaseSingleton<InputManger>
{
    private bool isStart;
    
    public InputManger()
    {
        // 每帧执行检测
        MonoManager.Instance.AddUpdateEvent(AddKeyCode);
    }
    
    /// <summary>
    /// 执行触发
    /// </summary>
    /// <param name="keyCode">要检测的按键</param>
    public void TriggerKeyCode(KeyCode keyCode)
    {
        // 长按
        if (Input.GetKey(keyCode))
        {
            EventCenter.Instance.TriggerEvent(keyCode+"长按");
        }
        // 按下
        if (Input.GetKeyDown(keyCode))
        {
            EventCenter.Instance.TriggerEvent(keyCode+"按下");
        }
        // 抬起
        if (Input.GetKeyUp(keyCode))
        {
            EventCenter.Instance.TriggerEvent(keyCode+"抬起");
        }
    }
    
    /// <summary>
    /// 添加的检测按键
    /// </summary>
    /// <param name="checkKeyCodeFunc"></param>
    public void AddKeyCode()
    {
        if (!isStart)return;

        TriggerKeyCode(KeyCode.A);
    }

    public void Start()
    {
        isStart = true;
    }

    public void Stop()
    {
        isStart = false;
    }
    
}
