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
    public Text txGameTips;

    public Transform TowerPanel;
    public List<TowerBtn> towerBtns;

    protected override void Init()
    {
        // 初始化建塔面板
        
        // 开始隐藏建塔面板
        TowerPanel.gameObject.SetActive(false);
        HideGameTips();
    }
    
    public void UpdateGameMoney(int num)
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

    public void ShowGameTips(string info)
    {
        txGameTips.gameObject.SetActive(true);
        txGameTips.text = info;
    }

    public void HideGameTips()
    {
        txGameTips.gameObject.SetActive(false);
    }
}