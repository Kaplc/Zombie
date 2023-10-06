using System;
using System.Collections;
using System.Collections.Generic;
using Script.FrameWork.MusicManager;
using UnityEngine;

public class GameManger: MonoBehaviour
{
    public static GameManger Instance => instance;
    private static GameManger instance;
    
    public Transform roleBornPos;
    public GameObject player;

    public Core core;
    public int count;
    public int money;

    public bool isGameOver;
    
    public ZombieBorn zombieBorn;
    private bool zombieTide;

    private AudioClip acZombieTide;

    public bool showMenu;

    public CameraFollow cameraFollow;
    
    private void Awake()
    {
        instance = this;
        // 创建BGM
        MusicManger.Instance.PlayMusic("Music/BGM", DataManager.Instance.musicData.musicVolume, true);

        // 提前加载音乐文件
        acZombieTide = Resources.Load<AudioClip>("Music/尸潮来袭");
        // 动态生成玩家出生点
        roleBornPos = new GameObject("RoleBornPos").GetComponent<Transform>();
        roleBornPos.transform.position = new Vector3(245f,29f,223f);
        // 创建玩家
        player = Instantiate(Resources.Load<GameObject>(DataManager.Instance.roleInfo.path), roleBornPos);
        // 设置摄像机跟随
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        cameraFollow.playerTransform = player.transform;
        
        // 启动按键检测
        InputManger.Instance.Start();
        // T建塔
        InputManger.Instance.AddCheckKeyCode(KeyCode.T);
        // ESC打开菜单面板
        InputManger.Instance.AddCheckKeyCode(KeyCode.Escape);
        // 补给相关
        InputManger.Instance.AddCheckKeyCode(KeyCode.E); // E键打开补给面板
        InputManger.Instance.AddCheckKeyCode(KeyCode.F1); // F1加子弹
        InputManger.Instance.AddCheckKeyCode(KeyCode.F2); // F2加血
        InputManger.Instance.AddCheckKeyCode(KeyCode.F3); // F3加攻击力
        
    }

    private void Start()
    {
        // 初始化游戏数据
        money = DataManager.Instance.mapInfos[DataManager.Instance.nowMapID].startMoney; // 初始金钱
        count = DataManager.Instance.mapInfos[DataManager.Instance.nowMapID].count; // 数量
        // 初始化面板数据
        UIManager.Instance.GetPanel<GamePanel>().UpdateCount(count);
        UIManager.Instance.GetPanel<GamePanel>().UpdateGameMoney(money);
        
        Invoke("SetPlayerPos", 0.5f);
        
        
        EventCenter.Instance.AddEventListener(KeyCode.Escape+"按下", () =>
        {
            if (!isGameOver)
            {
                showMenu = !showMenu;
            
                // 按一下打开
                if (showMenu)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    UIManager.Instance.Show<MenuPanel>();
                }
                else // 第二下关闭
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    UIManager.Instance.Hide<MenuPanel>();
                    UIManager.Instance.Hide<ExitPanel>();
                    UIManager.Instance.Hide<SettingPanel>();
                }
            }
        });
    }
     
    private void OnDestroy()
    {
        // 清空按键检测事件
        InputManger.Instance.Clear();
        EventCenter.Instance.ClearAllEvent();
        PoolManager.Instance.Clear();
    }

    private void SetPlayerPos()
    {
        player.transform.localPosition = Vector3.zero;
    }

    public void AddOrSubMoney(int num)
    {
        money += num;
        UIManager.Instance.GetPanel<GamePanel>().UpdateGameMoney(money);
    }

    /// <summary>
    /// 减少数量
    /// </summary>
    public void SubCount()
    {
        count--;
        // 少于1/2尸潮
        if (zombieTide == false && count <= zombieBorn.totalNum/1.5f)
        {
            zombieTide = true;
            AccelerateCreateZombie();
        }
        
        if (count == 0)
        {
            GameOver(true);
        }
        UIManager.Instance.GetPanel<GamePanel>().UpdateCount(count);
    }
    
    public void GameOver(bool isWin)
    {
        isGameOver = true;
        // 胜利或失败BGM
        if (isWin)
            MusicManger.Instance.PlayMusic("Music/胜利", DataManager.Instance.musicData.musicVolume, false);
            // bgm.clip = Resources.Load<AudioClip>("Music/胜利");
        else 
            MusicManger.Instance.PlayMusic("Music/结束", DataManager.Instance.musicData.musicVolume, false);
            // bgm.clip = Resources.Load<AudioClip>("Music/结束");
        // bgm.Play();
        // bgm.loop = false;
        // 准星隐藏
        UIManager.Instance.GetPanel<GamePanel>().imgStar.gameObject.SetActive(false);
        // 摄像机脚本跟随失活
        Camera.main.GetComponent<CameraFollow>().enabled = false;
        // 鼠标解除锁定
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // 更新结束面板
        UIManager.Instance.Show<EndPanel>().UpdateInfo(isWin?"胜利": "失败", DataManager.Instance.mapInfos[DataManager.Instance.nowMapID].count - count);
        UIManager.Instance.Hide<MenuPanel>();
        // 更新金币并保存
        DataManager.Instance.playerInfo.money += DataManager.Instance.mapInfos[DataManager.Instance.nowMapID].count - count;
        DataManager.Instance.SavePlayerInfo();
    }

    private void AccelerateCreateZombie()
    {
        UIManager.Instance.GetPanel<GamePanel>().ShowTileTips();
        
        MusicManger.Instance.PlayMusic("Music/尸潮来袭", DataManager.Instance.musicData.musicVolume, true);
        zombieBorn.CreateZombieTide();
    }
}
