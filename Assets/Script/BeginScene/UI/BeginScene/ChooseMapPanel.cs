using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChooseMapPanel : BasePanel
{
    public Button btnSure;
    public Button btnBack;
    public Button btnLeft;
    public Button btnRight;
    public Text txMapName;
    public Text txMapIntroduce;
    public Image imgMap;

    protected override void Init()
    {
        btnSure.onClick.AddListener(() =>
        {
            UIManager.Instance.Hide<ChooseMapPanel>(true, () =>
            {
                SceneManager.LoadScene("GameSceneCountrySide");
                UIManager.Instance.Show<GamePanel>();
            });
        });
        btnBack.onClick.AddListener(() =>
        {
            UIManager.Instance.Hide<ChooseMapPanel>(true, () =>
            {
                UIManager.Instance.Show<ChooseRolePanel>();
            });
        });
        btnLeft.onClick.AddListener(() =>
        {
            DataManager.Instance.nowMapID =
                --DataManager.Instance.nowMapID < 0 ? DataManager.Instance.mapInfos.Count - 1 : DataManager.Instance.nowMapID;
            CreateMapInfo();
        });
        btnRight.onClick.AddListener(() =>
        {
            DataManager.Instance.nowMapID =
                ++DataManager.Instance.nowMapID > DataManager.Instance.mapInfos.Count - 1
                    ? 0
                    : DataManager.Instance.nowMapID;
            CreateMapInfo();
        });
        // 初始化地图信息
        CreateMapInfo();
    }

    private void CreateMapInfo()
    {
        imgMap.sprite = Resources.Load<Sprite>(DataManager.Instance.mapInfos[DataManager.Instance.nowMapID].path);
        txMapName.text = DataManager.Instance.mapInfos[DataManager.Instance.nowMapID].name;
        txMapIntroduce.text = DataManager.Instance.mapInfos[DataManager.Instance.nowMapID].introduce;
    }
}
