using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    public Button btnClose;
    public Toggle tgMusic;
    public Toggle tgSound;
    public Slider sldMusic;
    public Slider sldSound;

    protected override void Init()
    {
        btnClose.onClick.AddListener(()=>
        {
            UIManager.Instance.Hide<SettingPanel>(true, null);
            DataManager.Instance.SaveMusicData();
        });
        tgMusic.onValueChanged.AddListener((isOn) =>
        {
            DataManager.Instance.musicData.musicOpen = isOn;
            BGMControl.Instance.UpdateMusicData();
        });
        tgSound.onValueChanged.AddListener((isOn) =>
        {
            DataManager.Instance.musicData.soundOpen = isOn;
        });
        sldMusic.onValueChanged.AddListener((value) =>
        {
            DataManager.Instance.musicData.musicVolume = value;
            BGMControl.Instance.UpdateMusicData();
        });
        sldSound.onValueChanged.AddListener((value) =>
        {
            DataManager.Instance.musicData.soundVolume = value;
        });
        
        // 初始化数据
        sldMusic.value = DataManager.Instance.musicData.musicVolume;
        sldSound.value = DataManager.Instance.musicData.soundVolume;
        tgMusic.isOn = DataManager.Instance.musicData.musicOpen;
        tgSound.isOn = DataManager.Instance.musicData.soundOpen;

    }
}
