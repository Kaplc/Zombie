using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    public Button btnStart;
    public Button btnSetting;
    public Button btnAbout;

    protected override void Init()
    {
        btnStart.onClick.AddListener(() =>
        {
            
        });
        btnSetting.onClick.AddListener(() =>
        {
            UIManager.Instance.Show<SettingPanel>();
        });
        btnAbout.onClick.AddListener(() =>
        {
            
        });
    }
}
