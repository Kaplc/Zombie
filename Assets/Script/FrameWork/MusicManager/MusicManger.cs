using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicManger : BaseSingleton<MusicManger>
{
    // 音乐
    private GameObject musicObject; // 音乐依附的对象
    public AudioSource musicAudioSource;
    private float musicVolume;

    // 音效
    private GameObject soundObject; // 音效依附的对象
    public List<AudioSource> soundAudioSources;
    private float soundVolume;

    public MusicManger()
    {
        MonoManager.Instance.AddUpdateEvent(UpdateSound);
    }

    private void UpdateSound()
    {
        for (int i = 0; i < soundAudioSources.Count; i++)
        {
            // 音效播放完毕就自动移除
            if (!soundAudioSources[i].isPlaying)
            {
                GameObject.Destroy(soundAudioSources[i]);
                soundAudioSources.RemoveAt(i);
            }
        }
    }

    #region 音乐相关

    // 音乐播放
    public void PlayMusic(string path)
    {
        if (!musicObject)
        {
            musicObject = new GameObject("Music");
            musicAudioSource = musicObject.AddComponent<AudioSource>();
        }
        // 异步加载音乐文件并播放
        ResourcesFrameWork.Instance.LoadAsync<AudioClip>(path, ac =>
        {
            musicAudioSource.clip = ac;
            musicAudioSource.Play();
            musicAudioSource.loop = true;
        });
    }

    // 修改音量
    public void ChangeMusicVolume(float volume)
    {
        if (musicAudioSource)
        {
            musicVolume = volume;
            musicAudioSource.volume = musicVolume;
            return;
        }
        Debug.Log("无音乐对象");
    }

    // 暂停
    public void PauseMusic()
    {
        if (musicAudioSource)
        {
            musicAudioSource.Pause();
            return;
        }
        Debug.Log("无音乐对象");
    }

    // 结束
    public void StopMusic()
    {
        if (musicAudioSource)
        {
            musicAudioSource.Stop();
            return;
        }
        Debug.Log("无音乐对象");
    }

    #endregion

    #region 音效相关

    // 音效播放
    public void PlaySound(string path, bool isLoop, UnityAction callBack = null)
    {
        if (!soundObject)
        {
            soundObject = new GameObject("Sound");
        }
        // 异步加载音乐文件并播放
        ResourcesFrameWork.Instance.LoadAsync<AudioClip>(path, ac =>
        {
            // 每次播放都添加一个音效组件在身上
            AudioSource newAudioSource = soundObject.AddComponent<AudioSource>();
            soundAudioSources.Add(newAudioSource); 
            newAudioSource.clip = ac;
            newAudioSource.Play();
            newAudioSource.loop = isLoop;
            // 播放音效完成的回调
            callBack?.Invoke();
        });
    }

    // 修改音效
    public void ChangeSoundVolume(float volume)
    {
        soundVolume = volume;
        for (int i = 0; i < soundAudioSources.Count; i++)
        {
            soundAudioSources[i].volume = soundVolume;
        }
    }

    // 停止音效
    public void StopSound(AudioSource audioSource)
    {
        if (soundAudioSources.Contains(audioSource))
        {
            soundAudioSources.Remove(audioSource);
            GameObject.Destroy(audioSource);
        }
    }

    #endregion
}