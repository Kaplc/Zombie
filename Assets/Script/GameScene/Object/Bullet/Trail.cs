using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    private TrailRenderer tr;
    
    private void OnEnable()
    {
        if (tr)
        {
            tr.Clear();
            tr.enabled = true; 
        }
        Invoke("Push", 1f);
    }

    // Start is called before the first frame update
    void Awake()
    {
        tr = GetComponent<TrailRenderer>();
    }

    private void Push()
    {
        PoolManager.Instance.PushObject(gameObject);
    }
}
