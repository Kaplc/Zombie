using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseRolePanel : BasePanel
{
    public Button btnRight;
    public Button btnLeft;
    public Button btnBack;
    public Button btnStart;
    public Button btnUnlock;
    public Text txMoney;

    public Animator cameraAnimator;

    public GameObject role;

    protected override void Init()
    {
        cameraAnimator = Camera.main.GetComponent<Animator>();
        
        // 初始化控件
        btnStart.onClick.AddListener(() =>
        {
            UIManager.Instance.Hide<ChooseRolePanel>(true, () =>
            {
                Destroy(role.gameObject);
                UIManager.Instance.Show<ChooseMapPanel>();
            });
        });
        btnBack.onClick.AddListener(() =>
        {
            // 摄像机右转动画
            cameraAnimator.SetTrigger("TurnRight");
            UIManager.Instance.Hide<ChooseRolePanel>(true, () =>
            {
                // 返回开始面板删除模型
                Destroy(role.gameObject);
            });
        });
        btnLeft.onClick.AddListener(() =>
        {
            // 防止越界
            DataManager.Instance.nowRoleID =
                --DataManager.Instance.nowRoleID < 0 ? DataManager.Instance.roleInfos.Count - 1 : DataManager.Instance.nowRoleID;
            CreateRole();
        });
        btnRight.onClick.AddListener(() =>
        {
            DataManager.Instance.nowRoleID =
                ++DataManager.Instance.nowRoleID > DataManager.Instance.roleInfos.Count - 1
                    ? 0
                    : DataManager.Instance.nowRoleID;
            CreateRole();
        });
        btnUnlock.onClick.AddListener(() =>
        {
            if (DataManager.Instance.playerInfo.money >= DataManager.Instance.roleInfos[DataManager.Instance.nowRoleID].unlockMoney)
            {
                // 扣钱解锁
                DataManager.Instance.playerInfo.money -= DataManager.Instance.roleInfos[DataManager.Instance.nowRoleID].unlockMoney;
                DataManager.Instance.playerInfo.unlockRoles.Add(DataManager.Instance.nowRoleID);
                DataManager.Instance.SavePlayerInfo();
                txMoney.text = DataManager.Instance.playerInfo.money.ToString();
                btnUnlock.gameObject.SetActive(false);
                btnStart.gameObject.SetActive(true);
            }
            else
            {
                // 提示
                UIManager.Instance.Show<TipsPanel>().UpdateTips("金钱不足");
            }
        });
        txMoney.text = DataManager.Instance.playerInfo.money.ToString();
        
        btnUnlock.gameObject.SetActive(false);
        // 初始化角色
        CreateRole();
    }

    private void CreateRole()
    {
        // 启用开始按钮
        btnStart.gameObject.SetActive(true);
        // 每次创建新对象删除旧对象
        if (role)
        {
            Destroy(role.gameObject);
            btnUnlock.gameObject.SetActive(false);
        }

        role = Instantiate(Resources.Load<GameObject>(DataManager.Instance.roleInfos[DataManager.Instance.nowRoleID].path),
            Main.Instance.showRolePos);

        if (!DataManager.Instance.playerInfo.unlockRoles.Contains(DataManager.Instance.nowRoleID))
        {
            // 未解锁显示解锁按钮
            btnUnlock.gameObject.SetActive(true);
            btnUnlock.transform.Find("Text").GetComponent<Text>().text = "$" +
                                                                         DataManager.Instance.roleInfos[DataManager.Instance.nowRoleID].unlockMoney;
            // 禁用开始按钮
            btnStart.gameObject.SetActive(false);
        }
    }
    
}