using System.Collections;
using System.Collections.Generic;
using Script.FrameWork.MusicManager;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    public Button btnClose;
    public Button btnInputSetting;
    public Toggle tgMusic;
    public Toggle tgSound;
    public Slider sldMusic;
    public Slider sldSound;

    protected override void Init()
    {
        btnClose.onClick.AddListener(() =>
        {
            UIManager.Instance.Hide<SettingPanel>(true, null);
            DataManager.Instance.SaveMusicData();
        });
        btnClose.onClick.AddListener(() =>
        {
            UIManager.Instance.Hide<InputSettingPanel>();
        });
        tgMusic.onValueChanged.AddListener((isOn) =>
        {
            MusicManger.Instance.MuteMusic(!isOn);
            DataManager.Instance.musicData.musicMute = !isOn;
        });
        tgSound.onValueChanged.AddListener((isOn) =>
        {
            MusicManger.Instance.MuteSound(!isOn);
            DataManager.Instance.musicData.soundMute = !isOn;
        });
        sldMusic.onValueChanged.AddListener((value) =>
        {
            MusicManger.Instance.ChangeMusicVolume(value);
            DataManager.Instance.musicData.musicVolume = value;
        });
        sldSound.onValueChanged.AddListener((value) =>
        {
            MusicManger.Instance.ChangeSoundVolume(value);
            DataManager.Instance.musicData.soundVolume = value;
        });

        // 初始化数据
        sldMusic.value = DataManager.Instance.musicData.musicVolume;
        sldSound.value = DataManager.Instance.musicData.soundVolume;
        tgMusic.isOn = !DataManager.Instance.musicData.musicMute;
        tgSound.isOn = !DataManager.Instance.musicData.soundMute;
    }
}