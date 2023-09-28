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
    public int magazineCount;
}
