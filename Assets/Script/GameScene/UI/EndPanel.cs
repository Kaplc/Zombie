using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndPanel : BasePanel
{
    public Button btnSure;
    public Text txTitle;
    public Text txGem;

    protected override void Init()
    {
        btnSure.onClick.AddListener(() =>
        {
            UIManager.Instance.Hide<GamePanel>();
            // 异步返回主菜单
            AsyncOperation ao = SceneManager.LoadSceneAsync("BeginScene");
            ao.completed += operation =>
            {
                Time.timeScale = 1; // 恢复时间
            };
            UIManager.Instance.Hide<EndPanel>();
        });
    }

    public void UpdateInfo(string title, int gemNum)
    {
        txTitle.text = title;
        txGem.text = "获得金币" + gemNum;
    }
}