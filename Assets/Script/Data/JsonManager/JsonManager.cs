using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

/// <summary>
/// json解析工具类型
/// </summary>
public enum E_JsonTool
{
    JsonUtility,
    LitJson
}

public class JsonManager
{
    private static JsonManager instance = new JsonManager();
    public static JsonManager Instance => instance;

    private JsonManager()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="data">数据对象</param>
    /// <param name="toolType">默认使用LitJson</param>
    public void Save(string fileName, object data, E_JsonTool toolType)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".json";
        string json = "";

        switch (toolType)
        {
            case E_JsonTool.JsonUtility:
                json = JsonUtility.ToJson(data);
                break;
            case E_JsonTool.LitJson:
                json = JsonMapper.ToJson(data);
                break;
        }

        File.WriteAllText(path, json);
    }

    public T Load<T>(string fileName, E_JsonTool toolType) where T : new()
    {
        string path = Application.streamingAssetsPath + "/" + fileName + ".json";

        if (!File.Exists(path))
        {
            path = Application.persistentDataPath + "/" + fileName + ".json";
        }

        if (!File.Exists(path))
        {
            // 不存在文件返回默认
            return new T();
        }

        T newObj = new T();
        string json = "";
        json = File.ReadAllText(path);
        switch (toolType)
        {
            case E_JsonTool.JsonUtility:
                newObj = JsonUtility.FromJson<T>(json);
                break;
            case E_JsonTool.LitJson:
                newObj = JsonMapper.ToObject<T>(json);
                break;
        }

        return newObj;
    }
}