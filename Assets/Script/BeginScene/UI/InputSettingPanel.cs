using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class InputSettingPanel : BasePanel
{
    public Button btnReturn;
    public Button btnReset;
    public Button btnApply;

    public InputSettingControl wControl;
    public InputSettingControl aControl;
    public InputSettingControl sControl;
    public InputSettingControl dControl;
    public InputSettingControl xControl;
    public InputSettingControl cControl;
    public InputSettingControl rControl;
    public InputSettingControl gControl;

    public InputInfo info;
    public InputInfo newInfo;

    protected override void Init()
    {
        btnApply.onClick.AddListener(() =>
        {
            // 应用改键信息
            NewInputManager.Instance.ChangeInputInfo(newInfo);
        });
        btnReset.onClick.AddListener(() =>
        {
            NewInputManager.Instance.ResetInputInfo();
            newInfo = new InputInfo();
            UpdateControlInfo(new InputInfo());
            
        });
        btnReturn.onClick.AddListener(() =>
        {
            UIManager.Instance.Hide<InputSettingPanel>();  
        });

        info = NewInputManager.Instance.GetInputInfo();
        
        newInfo = new InputInfo(); // 改键缓存信息
        wControl.btnChange.onClick.AddListener(() =>
        {
            InputSystem.onAnyButtonPress.CallOnce(control =>
            {
                newInfo.forward = newInfo.forward.Replace("w", control.name);
                wControl.UpdateInfo(newInfo.forward);
            });
        });
        gControl.btnChange.onClick.AddListener(() =>
        {
            InputSystem.onAnyButtonPress.CallOnce(control =>
            {
                newInfo.weapon = newInfo.weapon.Replace("g", control.name);
                gControl.UpdateInfo(newInfo.weapon);
            });
        });
        
        
        UpdateControlInfo(info);
    }

    public void UpdateControlInfo(InputInfo info)
    {
        wControl.UpdateInfo(info.forward);
        aControl.UpdateInfo(info.left);
        sControl.UpdateInfo(info.back);
        dControl.UpdateInfo(info.right);
        xControl.UpdateInfo(info.roll);
        cControl.UpdateInfo(info.crouch);
        rControl.UpdateInfo(info.reloading);
        gControl.UpdateInfo(info.weapon);
    }
}
