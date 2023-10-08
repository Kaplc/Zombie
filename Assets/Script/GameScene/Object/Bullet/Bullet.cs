using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public List<ParticleSystem> particleSystems;
    public TrailRenderer tr;

    private Coroutine coroutine;

    private void OnEnable()
    {
        if (particleSystems.Count > 0)
        {
            for (int i = 0; i < particleSystems.Count; i++)
            {
                particleSystems[i].Play();
            }
            tr.Clear();
        }
        
        // 缓存池保存
        coroutine = StartCoroutine(PushCoroutine());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(transform.forward * (Time.deltaTime * speed), Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Buildings"))
        {
            StopCoroutine(coroutine);
            PushBullet();
        }
    }

    private void PushBullet()
    {
        PoolManager.Instance.PushObject(gameObject);
    }

    private IEnumerator PushCoroutine()
    {
        yield return new WaitForSeconds(3f);
        PushBullet();
    }
}