using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.forward * (Time.deltaTime * 50), Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Builtings"))
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, 0.5f);
        }
    }
}
