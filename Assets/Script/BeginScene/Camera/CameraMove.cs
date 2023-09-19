using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // 动画事件函数
    public void ShowChooseRolePanel()
    {
        // 完成旋转动画才显示面板
        UIManager.Instance.Show<ChooseRolePanel>();
    }

    public void ShowBeginPanel()
    {
        UIManager.Instance.Show<BeginPanel>();
    }
}
