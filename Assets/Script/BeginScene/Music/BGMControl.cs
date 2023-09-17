using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMControl : MonoBehaviour
{
    private static BGMControl instance;
    public static BGMControl Instance => instance;
    
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.mute = !DataManager.Instance.musicData.musicOpen;
        audioSource.volume = DataManager.Instance.musicData.musicVolume;
    }

    public void UpdateMusicData()
    {
        audioSource.mute = !DataManager.Instance.musicData.musicOpen;
        audioSource.volume = DataManager.Instance.musicData.musicVolume;
    }
}
