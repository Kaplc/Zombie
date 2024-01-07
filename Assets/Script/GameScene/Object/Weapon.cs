using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Weapon
{
    Knife,
    HandGun,
    MachineGun,
    RocketLauncher
}

public class WeaponBag
{
    public int resSpareBullets;
    public int spareBullets;
    public Weapon weapon;

    public WeaponBag(int bulletCount, Weapon weapon)
    {
        spareBullets = resSpareBullets = bulletCount;
        this.weapon = weapon;
    }
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
        if (nowBulletCount<0)
        {
            nowBulletCount = 0;
        }
    }
    
    public void ReLoading(int count)
    {
        nowBulletCount = count;
    }
}