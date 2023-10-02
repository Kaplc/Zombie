using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AboutPanel : BasePanel
{
    public Button btnClose;
    
    protected override void Init()
    {
        btnClose.onClick.AddListener(() =>
        {
            UIManager.Instance.Hide<AboutPanel>();
        });
    }
}
