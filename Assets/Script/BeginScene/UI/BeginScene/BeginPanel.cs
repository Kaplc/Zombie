using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    public Button btnStart;
    public Button btnSetting;
    public Button btnAbout;
    public Button btnQuit;
    
    private Animator cameraAnimator;

    protected override void Init()
    {
        cameraAnimator = Camera.main.GetComponent<Animator>();
        
        btnStart.onClick.AddListener(() =>
        {
            // 摄像机左转动画
            cameraAnimator.SetTrigger("TurnLeft");
            UIManager.Instance.Hide<BeginPanel>(true, null);
        });
        btnSetting.onClick.AddListener(() =>
        {
            UIManager.Instance.Show<SettingPanel>();
        });
        btnAbout.onClick.AddListener(() =>
        {
            UIManager.Instance.Show<AboutPanel>();
        });
        btnQuit.onClick.AddListener(() =>
        {
            Application.Quit(0);
        });
    }
}
