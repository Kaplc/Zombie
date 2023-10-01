using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    // 核心
    public RectTransform ImgHp;
    public Text txHp;
    // 金钱
    public Text txGameMoney;

    // 交互提示
    public Text txGameTips;

    // zombie提示
    public Text txCount;

    public Animator animTideTips;

    // 准星
    public Image imgStar;

    // 人物血量
    public RectTransform imgPlayerHp;

    public Text txPlayerHp;

    // 子弹信息
    public Image imgBulletCd;
    public Text txBulletCount;

    private bool isOnRefreshBulletImg; // 刷新开火图标cd

    // 补给面板
    public Transform shopPanel;
    public List<ItemBtn> itemBtns;
    
    // atk
    public Text txAtk;

    protected override void Init()
    {
        // 初始化建塔面板

        // 开始隐藏商店面板
        HideShopPanel();
        HideGameTips();
    }

    protected override void Update()
    {
        base.Update();

        if (isOnRefreshBulletImg)
        {
            imgBulletCd.fillAmount = Mathf.Clamp(imgBulletCd.fillAmount + Time.deltaTime * 2, 0.001f, 1);
            // 刷新完成
            if (imgBulletCd.fillAmount >= 1)
            {
                isOnRefreshBulletImg = false;
            }
        }
    }

    #region 面板数据相关

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
        ImgHp.sizeDelta = new Vector2(hp / maxHp * 600, 30);
        txHp.text = $"{hp}/{maxHp}";
    }

    public void UpdatePlayerHp(float hp, float maxHp)
    {
        imgPlayerHp.sizeDelta = new Vector2(hp / maxHp * 400, 30);
        txPlayerHp.text = $"{hp}/{maxHp}";
    }

    public void RefreshBulletImg()
    {
        isOnRefreshBulletImg = true;
        imgBulletCd.fillAmount = 0;
    }

    public void UpdateBullet(int bulletCount, int totalBulletCount)
    {
        txBulletCount.text = bulletCount + "/" + totalBulletCount;
    }

    public void HideBulletInfo()
    {
        imgBulletCd.gameObject.SetActive(false);
        txBulletCount.gameObject.SetActive(false);
    }

    public void ShowBulletInfo()
    {
        imgBulletCd.gameObject.SetActive(true);
        txBulletCount.gameObject.SetActive(true);
    }

    public void UpdateAtk(int atk)
    {
        txAtk.text = $"基础攻击力: {atk}";
    }

    #endregion

    #region 提示信息相关

    public void ShowGameTips(string info)
    {
        txGameTips.gameObject.SetActive(true);
        txGameTips.text = info;
        Invoke("HideGameTips", 2);
    }

    public void HideGameTips()
    {
        txGameTips.gameObject.SetActive(false);
    }

    public void ShowTileTips()
    {
        animTideTips.SetTrigger("ShowTips");
    }

    public void ShowShopPanel()
    {
        shopPanel.gameObject.SetActive(true);
    }

    public void HideShopPanel()
    {
        shopPanel.gameObject.SetActive(false);
    }

    #endregion
}