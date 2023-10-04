using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEff : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;
    private bool isAwake;

    private void OnEnable()
    {
        if (isAwake)
        {
            for (int i = 0; i < particleSystems.Count; i++)
            {
                particleSystems[i].Play();
            }
            Invoke("Push", particleSystems[0].main.duration);
        }
        
    }

    private void Awake()
    {
        isAwake = true;
    }

    private void Push()
    {
        PoolManager.Instance.PushObject(gameObject);
    }
}
