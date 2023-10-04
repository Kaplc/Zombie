using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
        MapInfo mapInfo = DataManager.Instance.mapInfos[DataManager.Instance.nowMapID];
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
        // 随机僵尸属性
        zombieInfo = DataManager.Instance.zombieInfos[Random.Range(0, DataManager.Instance.zombieInfos.Count)];
        
        GameObject zombie = PoolManager.Instance.GetObject(zombieInfo.path);
        if (zombie)
        {
            zombie.GetComponent<NavMeshAgent>().enabled = false;
            zombie.transform.SetParent(bornPos[Random.Range(0, bornPos.Count)]);
            zombie.transform.localPosition = Vector3.zero;
            zombie.GetComponent<NavMeshAgent>().enabled = true;
            zombie.GetComponent<Zombie>().Init(zombieInfo);
        }
        else
        {
            zombie = Instantiate(Resources.Load<GameObject>(zombieInfo.path),bornPos[Random.Range(0, bornPos.Count)]);
            zombie.AddComponent<Zombie>().Init(zombieInfo);
            zombie.name = zombieInfo.path;
        }
        
        surplusNum--;
    }

    public void CreateZombieTide()
    {
        waveCd *= 0.2f;
        eachCd *= 0.2f;
    }
}