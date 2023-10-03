using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger: MonoBehaviour
{
    public static GameManger Instance => instance;
    private static GameManger instance;
    
    public Transform roleBornPos;
    public Transform player;
    public Player playerCpm;
    
    public Core core;
    public int count;
    public int money;

    public bool isGameOver;

    public AudioSource bgm;

    public ZombieBorn zombieBorn;
    private bool zombieTide;

    private AudioClip acZombieTide;

    public bool showMenu;
    
    private void Awake()
    {
        instance = this;
        // 创建玩家
        player = Instantiate(Resources.Load<GameObject>(DataManager.Instance.roleInfo.path)).GetComponent<Transform>();
        player.position = roleBornPos.position;
        // 设置摄像机跟随
        Camera.main.GetComponent<CameraFollow>().playerTransform = player;
        // 创建BGM
        bgm = new GameObject("BGM").AddComponent<AudioSource>();
        bgm.gameObject.AddComponent<BGMControl>();
        bgm.clip = Resources.Load<AudioClip>("Music/BGM");
        bgm.Play();
        bgm.loop = true;
        // 提前加载音乐文件
        acZombieTide = Resources.Load<AudioClip>("Music/尸潮来袭");
    }

    private void Start()
    {
        // 初始化游戏数据
        money = DataManager.Instance.mapInfos[DataManager.Instance.nowMapID].startMoney; // 初始金钱
        count = DataManager.Instance.mapInfos[DataManager.Instance.nowMapID].count; // 数量
        // 初始化面板数据
        UIManager.Instance.GetPanel<GamePanel>().UpdateCount(count);
        UIManager.Instance.GetPanel<GamePanel>().UpdateGameMoney(money);
        // 
        playerCpm = player.GetComponent<Player>();
    }

    private void Update()
    {
        // esc打开菜单
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
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
            bgm.clip = Resources.Load<AudioClip>("Music/胜利");
        else 
            bgm.clip = Resources.Load<AudioClip>("Music/结束");
        bgm.Play();
        bgm.loop = false;
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
        
        bgm.clip = acZombieTide;
        bgm.Play();
        bgm.loop = true;
        zombieBorn.CreateZombieTide();
    }
}
