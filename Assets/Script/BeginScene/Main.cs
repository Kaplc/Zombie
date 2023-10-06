using System;
using System.Collections;
using System.Collections.Generic;
using Script.FrameWork.MusicManager;
using UnityEngine;

public class Main : MonoBehaviour
{
    private static Main instance;
    public static Main Instance => instance;

    public Transform showRolePos; // 生成角色模型位置

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.Show<BeginPanel>();
        
        MusicManger.Instance.PlayMusic("Music/开始界面", DataManager.Instance.musicData.musicVolume, true);
    }
}