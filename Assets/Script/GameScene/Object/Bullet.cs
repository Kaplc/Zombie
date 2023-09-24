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
}
