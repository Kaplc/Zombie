using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    private static DataManager instance = new DataManager();
    public static DataManager Instance => instance;
    
    public MusicData musicData;
    public List<RoleInfo> roleInfos;
    public PlayerInfo playerInfo;

    public int nowRoleID = 0;

    private DataManager()
    {
        musicData = JsonManager.Instance.Load<MusicData>("musicData", E_JsonTool.LitJson);
        roleInfos = JsonManager.Instance.Load<List<RoleInfo>>("RoleInfos", E_JsonTool.LitJson);
        playerInfo = JsonManager.Instance.Load<PlayerInfo>("PlayerInfo", E_JsonTool.LitJson);
    }

    public void SaveMusicData()
    {
        JsonManager.Instance.Save("musicData", musicData, E_JsonTool.LitJson);
    }

    public void SavePlayerInfo()
    {
        JsonManager.Instance.Save("PlayerInfo", playerInfo, E_JsonTool.LitJson);
    }
}
