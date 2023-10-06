using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Script.FrameWork.MusicManager
{
    public class MusicData
    {
        public bool musicMute;
        public bool soundMute;
        public float musicVolume;
        public float soundVolume;
    }


    public class MusicManger : BaseSingleton<MusicManger>
    {
        // 音乐
        private GameObject musicObject; // 音乐依附的对象
        private AudioSource musicAudioSource;

        // 音效
        private GameObject soundObject; // 音效依附的对象
        private List<AudioSource> soundAudioSources;

        public MusicManger()
        {
            MonoManager.Instance.AddUpdateEvent(UpdateSound);
            soundAudioSources = new List<AudioSource>();
        }

        private void UpdateSound()
        {
            if (!soundObject)
            {
                soundObject = null;
                soundAudioSources.Clear();
            }
            
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

        /// <summary>
        /// 音乐播放
        /// </summary>
        /// <param name="path">音乐资源路径</param>
        /// <param name="volume">音量大小</param>
        /// <param name="isLoop">是否循环</param>
        public void PlayMusic(string path, float volume, bool isLoop)
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
                musicAudioSource.volume = volume;
                musicAudioSource.loop = isLoop;
                musicAudioSource.Play();
            });
        }

        // 修改音量
        public void ChangeMusicVolume(float volume)
        {
            if (musicAudioSource)
            {
                musicAudioSource.volume = volume;
                return;
            }
            Debug.Log("无音乐对象");
        }
        // 静音
        public void MuteMusic(bool isMute)
        {
            if (musicAudioSource)
            {
                musicAudioSource.mute = isMute;
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
        public void PlaySound(string path,float volume, bool isLoop, UnityAction callBack = null)
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
                newAudioSource.volume = volume;
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
            for (int i = 0; i < soundAudioSources.Count; i++)
            {
                soundAudioSources[i].volume = volume;
            }
        }
        // 静音
        public void MuteSound(bool isMute)
        {
            for (int i = 0; i < soundAudioSources.Count; i++)
            {
                soundAudioSources[i].mute = isMute;
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
}