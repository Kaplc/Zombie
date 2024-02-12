using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewInputManager : BaseSingleton<NewInputManager>
{
    private string jsonPath = "InputSetting/InputSetting";
    
    private InputActions inputActions;
    public InputActions InputActions
    {
        get
        {
            if (inputActions == null)
            {
                Init();
            }

            return inputActions;
        }
    }

    private void Init()
    {
        // 加载输入配置json文件
        string json = Resources.Load<TextAsset>(jsonPath).text;
        // 加载改键数据
        InputInfo info = JsonManager.Instance.Load<InputInfo>("InputInfo");
        // 根据数据修改按键配置
        // 获取默认 InputInfo 类型的所有字段，遍历所有字段，并输出字段名和字段值
        InputInfo originInfo = new InputInfo();
        foreach (FieldInfo field in originInfo.GetType().GetFields())
        {
            // 获取持久化文件中数据的字段
            FieldInfo fieldInfo = info.GetType().GetField(field.Name); 
            // 修改
            if ((string)field.GetValue(originInfo) != (string)fieldInfo.GetValue(info))
            {
               json = json.Replace((string)field.GetValue(originInfo), (string)fieldInfo.GetValue(info));
            }
        }
        
        inputActions = new InputActions(json);
    }
    
    /// <summary>
    /// 恢复按键默认
    /// </summary>
    public void ResetInputInfo()
    {
        JsonManager.Instance.Save("InputInfo", new InputInfo());
        // 刷新
        if (inputActions != null)Init();
    }
    
    /// <summary>
    /// 修改按键信息
    /// </summary>
    /// <param name="info"></param>
    public void ChangeInputInfo(InputInfo info)
    {
        // 持久化
        JsonManager.Instance.Save("InputInfo", info);
        // 刷新
        if (inputActions != null)Init();
    }

    public InputInfo GetInputInfo()
    {
        InputInfo info = JsonManager.Instance.Load<InputInfo>("InputInfo");
        return info;
    }
    
}