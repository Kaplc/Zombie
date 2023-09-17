using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    private static DataManager instance = new DataManager();
    public static DataManager Instance => instance;
    
    public MusicData musicData;

    private DataManager()
    {
        musicData = JsonManager.Instance.Load<MusicData>("musicData", E_JsonTool.LitJson);
    }

    public void SaveMusicData()
    {
        JsonManager.Instance.Save("musicData", musicData, E_JsonTool.LitJson);
    }
}
