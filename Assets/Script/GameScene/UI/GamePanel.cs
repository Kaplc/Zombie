using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    public RectTransform ImgHp;
    public Text txHp;
    public Text txCount;
    public Text txGameMoney;

    public Transform TowerPanel;
    public List<TowerBtn> towerBtns;

    protected override void Init()
    {
        // 初始化建塔面板
        
        // 开始隐藏建塔面板
        TowerPanel.gameObject.SetActive(false);
    }
    
    public void AddGameMoney(int num)
    {
        txGameMoney.text = num.ToString();
    }

    public void UpdateCount(int count)
    {
        txCount.text = count.ToString();
    }
    
    public void UpdateCoreHp(float hp, float maxHp)
    {
        ImgHp.sizeDelta = new Vector2(hp/maxHp * 600, 30);
        txHp.text = $"{hp}/{maxHp}";
    }
}