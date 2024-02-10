using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputSettingPanel : BasePanel
{
    public Button btnReturn;
    public Button btnReset;
    public Button btnApply;

    protected override void Init()
    {
        btnApply.onClick.AddListener(() =>
        {
            
        });
        btnReset.onClick.AddListener(() =>
        {
            
        });
        btnReturn.onClick.AddListener(() =>
        {
            UIManager.Instance.Hide<InputSettingPanel>();  
        });
    }
}
