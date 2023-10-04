using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muzzle : MonoBehaviour
{
    public List<ParticleSystem> childPs;

    private bool isAwake;

    private void OnEnable()
    {
        for (int i = 0; i < childPs.Count; i++)
        {
            childPs[i].Play();
        }
        
        Invoke("PushMuzzle", childPs[0].main.duration);
    }
    
    public void PushMuzzle()
    {
        PoolManager.Instance.PushObject(gameObject);
    }
}