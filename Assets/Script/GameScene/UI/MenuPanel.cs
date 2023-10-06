using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : BasePanel
{
    public Button btnExit;
    public Button btnSetting;
    public Button btnBack;
    
    protected override void Init()
    {
        btnExit.onClick.AddListener(() =>
        {
            UIManager.Instance.Show<ExitPanel>();
        });
        btnSetting.onClick.AddListener(() =>
        {
            UIManager.Instance.Show<SettingPanel>();
        });
        btnBack.onClick.AddListener(() =>
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            GameManger.Instance.showMenu = false;
            UIManager.Instance.Hide<MenuPanel>();
        });
    }
}
