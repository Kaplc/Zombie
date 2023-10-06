using System.Collections;
using System.Collections.Generic;
using Script.FrameWork.MusicManager;
using UnityEngine;

public class DataManager
{
    private static DataManager instance = new DataManager();
    public static DataManager Instance => instance;
    
    public MusicData musicData;
    public List<RoleInfo> roleInfos;
    public List<MapInfo> mapInfos;
    public List<ZombieInfo> zombieInfos;
    
    public PlayerInfo playerInfo; // 玩家信息
    
    public RoleInfo roleInfo; // 当前选择的人物信息
    public int nowRoleID = 0; // 当前选择的人物id

    public MapInfo mapInfo;
    public int nowMapID = 0;

    private DataManager()
    {
        musicData = JsonManager.Instance.Load<MusicData>("musicData", E_JsonTool.LitJson);
        roleInfos = JsonManager.Instance.Load<List<RoleInfo>>("RoleInfos", E_JsonTool.LitJson);
        playerInfo = JsonManager.Instance.Load<PlayerInfo>("PlayerInfo", E_JsonTool.LitJson);
        mapInfos = JsonManager.Instance.Load<List<MapInfo>>("MapInfos", E_JsonTool.LitJson);
        zombieInfos = JsonManager.Instance.Load<List<ZombieInfo>>("ZombieInfos", E_JsonTool.LitJson);
    }

    public void SaveMusicData()
    {
        JsonManager.Instance.Save("musicData", musicData, E_JsonTool.LitJson);
    }

    public void SavePlayerInfo()
    {
        JsonManager.Instance.Save("PlayerInfo", playerInfo, E_JsonTool.LitJson);
    }

    public void RecordGameInfo()
    {
        roleInfo = roleInfos[nowRoleID];
        mapInfo = mapInfos[nowMapID];
    }
}
