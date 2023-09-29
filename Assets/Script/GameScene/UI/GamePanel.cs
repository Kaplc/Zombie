using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    public RectTransform ImgHp;
    public Text txHp;
    public Text txCount;
    public Text txGameMoney;

    public Text txGameTips;

    public Animator animTileTips;

    // 准星
    public Image imgStar;

    // 人物血量
    public RectTransform imgPlayerHp;
    public Text txPlayerHp;

    public Image imgBulletCd;
    public Text txBulletCount;

    public Transform TowerPanel;
    public List<ItemBtn> towerBtns;
    
    // 刷新开火图标cd
    private bool isOnRefreshBulletImg;

    protected override void Init()
    {
        // 初始化建塔面板

        // 开始隐藏建塔面板
        TowerPanel.gameObject.SetActive(false);
        HideGameTips();
    }

    protected override void Update()
    {
        base.Update();

        if (isOnRefreshBulletImg)
        {
            imgBulletCd.fillAmount = Mathf.Clamp(imgBulletCd.fillAmount + Time.deltaTime, 0.001f, 1);
            // 刷新完成
            if (imgBulletCd.fillAmount >= 1)
            {
                isOnRefreshBulletImg = false;
            }
        }
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
        ImgHp.sizeDelta = new Vector2(hp / maxHp * 600, 30);
        txHp.text = $"{hp}/{maxHp}";
    }

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

    public void ShowTileTips()
    {
        animTileTips.SetTrigger("ShowTips");
    }
}