using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitPanel : BasePanel
{
    public Button btnSure;
    public Button btnCancel;
    
    protected override void Init()
    {
        btnSure.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("BeginScene");
            UIManager.Instance.Hide<GamePanel>();
            UIManager.Instance.Hide<MenuPanel>();
            UIManager.Instance.Hide<ExitPanel>();
        });
        btnCancel.onClick.AddListener(() =>
        {
            UIManager.Instance.Hide<ExitPanel>();
        });
    }
}
