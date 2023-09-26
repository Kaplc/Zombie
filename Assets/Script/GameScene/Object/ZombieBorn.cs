using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieBorn : MonoBehaviour
{
    public List<Transform> bornPos;
    public int totalNum; // 总数量
    public int surplusNum; // 剩余数量

    public float eachCd; // 每只cd
    private float lastEachTime;

    public int waveNum; // 每波数量
    public float waveCd; // 每波cd
    private float lastWaveTime;

    private ZombieInfo zombieInfo;

    private void Start()
    {
        MapInfo mapInfo = DataManager.Instance.mapInfo;
        totalNum = mapInfo.count;
        eachCd = mapInfo.eachCd;
        waveNum = mapInfo.waveNum;
        waveCd = mapInfo.waveCd;
        
        surplusNum = totalNum;
    }

    private void Update()
    {
        // 还有剩余数量且cd满足就创建新的一波
        if (surplusNum > 0 && Time.time - lastWaveTime >= waveCd)
        {
            lastWaveTime = Time.time; // 记录上一波的时间
            Invoke("CreateWaveZombies", waveCd);
        }
    }

    private void CreateWaveZombies()
    {
        // 创建所有延迟函数
        for (int i = 0; i < waveNum; i++)
        {
            Invoke("CreateEachZombies", i * eachCd);
        }
    }

    private void CreateEachZombies()
    {
        if (surplusNum <= 0)
        {
            return;
        }
        
        zombieInfo = DataManager.Instance.zombieInfos[Random.Range(0, DataManager.Instance.zombieInfos.Count - 1)];
        Instantiate(Resources.Load<GameObject>(zombieInfo.path), bornPos[Random.Range(0, bornPos.Count - 1)]).AddComponent<Zombie>().Init(zombieInfo);
        surplusNum--;
        
    }

    public void CreateZombieTide()
    {
        // 数量小于1/3加速生成
        if (surplusNum == totalNum / 3)
        {
            waveCd *= 0.5f;
            eachCd *= 0.5f;
        }
    }
}