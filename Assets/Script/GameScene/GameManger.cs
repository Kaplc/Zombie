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
    
    public Core core;
    public int count;
    public int money;

    public bool isGameOver;

    public AudioSource bgm;

    public ZombieBorn zombieBorn;
    private bool zombieTide;

    private AudioClip acZombieTide;
    
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
        bgm.clip = Resources.Load<AudioClip>("Music/BGM");
        bgm.Play();
        bgm.loop = true;
        acZombieTide = Resources.Load<AudioClip>("Music/尸潮来袭");
    }

    private void Start()
    {
        // 初始化游戏数据
        money = DataManager.Instance.mapInfo.startMoney; // 初始金钱
        count = DataManager.Instance.mapInfo.count; // 数量
        // 初始化面板数据
        UIManager.Instance.GetPanel<GamePanel>().UpdateCount(count);
        UIManager.Instance.GetPanel<GamePanel>().AddGameMoney(money);
    }

    public void AddMoney(int num)
    {
        money += num;
        UIManager.Instance.GetPanel<GamePanel>().AddGameMoney(money);
    }
    
    /// <summary>
    /// 减少数量
    /// </summary>
    public void SubCount()
    {
        count--;
        // 少于1/3尸潮
        if (count <= zombieBorn.totalNum/2 && zombieTide == false)
        {
            zombieTide = true;
            AccelerateCreateZombie();
        }
        
        if (count == 0)
        {
            // 胜利BGM
            bgm.clip = Resources.Load<AudioClip>("Music/胜利");
            GameOver(true);
        }
        UIManager.Instance.GetPanel<GamePanel>().UpdateCount(count);
    }
    
    public void GameOver(bool isWin)
    {
        isGameOver = true;
        // 人物停止移动并失活
        player.GetComponent<Animator>().SetFloat("Xspeed", 0f);
        player.GetComponent<Animator>().SetFloat("Yspeed", 0f);
        player.gameObject.GetComponent<Player>().enabled = false;
        // 摄像机脚本跟随失活
        Camera.main.GetComponent<CameraFollow>().enabled = false;
        // 鼠标解除锁定
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // 更新结束面板
        UIManager.Instance.Show<EndPanel>().UpdateInfo(isWin?"胜利": "失败", (money - 200)/10);
        // 更新金币并保存
        DataManager.Instance.playerInfo.money += (money - 200) / 10;
        DataManager.Instance.SavePlayerInfo();
    }

    private void AccelerateCreateZombie()
    {

        bgm.clip = acZombieTide;
        bgm.Play();
        zombieBorn.CreateZombieTide();
    }
}
