using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneMain : MonoBehaviour
{
    public static GameSceneMain Instance => instance;
    private static GameSceneMain instance;

    public Transform player;
    public Transform core;
    
    private void Awake()
    {
        instance = this;
    }
}
