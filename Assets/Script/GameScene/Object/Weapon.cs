using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Weapon
{
    Knife,
    HandGun
}


public class Weapon : MonoBehaviour
{
    public E_Weapon type;
    public Transform firePos;
    public int bulletCount;
    public int nowBulletCount;
    public int atk;

    private void Awake()
    {
        nowBulletCount = bulletCount;
    }
    
    public void SubBullet()
    {
        // 子弹-1
        nowBulletCount--;
    }
    
    public void ReLoading(int count)
    {
        nowBulletCount = count;
    }
}