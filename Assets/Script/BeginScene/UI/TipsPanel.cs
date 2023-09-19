using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsPanel : BasePanel
{
    public Button btnSure;
    public Text txTips;
    
    protected override void Init()
    {
        btnSure.onClick.AddListener(() =>
        {
            UIManager.Instance.Hide<TipsPanel>(true, null);
        });
    }

    public void UpdateTips(string info)
    {
        txTips.text = info;
    }
}
