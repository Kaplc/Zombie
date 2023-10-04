using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public ParticleSystem ps;
    public ParticleSystem beamPs;
    public TrailRenderer tr;

    private void OnEnable()
    {
        if (ps)
        {
            ps.Play();
            beamPs.Play();
            tr.Clear();
        }
        
        // 缓存池保存
        Invoke("PushBullet", 3f);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(transform.forward * (Time.deltaTime * 50), Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Buildings"))
        {
            PushBullet();
        }
    }

    private void PushBullet()
    {
        PoolManager.Instance.PushObject(gameObject);
    }
    
}