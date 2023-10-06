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
            UIManager.Instance.Hide<GamePanel>();
            UIManager.Instance.Hide<MenuPanel>();
            UIManager.Instance.Hide<ExitPanel>();
            SceneManager.LoadScene("BeginScene");
        });
        btnCancel.onClick.AddListener(() =>
        {
            UIManager.Instance.Hide<ExitPanel>();
        });
    }
}
