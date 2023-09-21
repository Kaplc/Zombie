using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    public RectTransform ImgHp;
    public Text txHp;
    public Text txTimes;
    public Text txGameMoney;

    public Transform TowerPanel;
    public List<TowerBtn> towerBtns;

    private int hp;
    private int nowTimes;
    private int totalTimes;
    private int gameMoney;
    
    protected override void Init()
    {
        txGameMoney.text = 100.ToString(); // 默认游戏内金钱为100

        hp = 60;
        nowTimes = 1;
        totalTimes = 10;
        gameMoney = 200;
        
        UpdateHp();
        UpdateTimes();
        UpdateGameMoney(999);
        
        // 初始化建塔面板
        
        // 开始隐藏建塔面板
        TowerPanel.gameObject.SetActive(false);
    }

    public void UpdateGameMoney(int num)
    {
        txGameMoney.text = (gameMoney + num).ToString();
    }

    public void UpdateTimes()
    {
        txTimes.text = nowTimes + "/" + totalTimes;
    }
    
    public void UpdateHp()
    {
        ImgHp.sizeDelta = new Vector2(hp/100f * 600, 30);
        txHp.text = $"{hp}/100";
    }
}