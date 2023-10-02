using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleRound : MonoBehaviour
{
    public float speed;
    private bool allowRota;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), 200, 1<< LayerMask.NameToLayer("Player")))
            {
                allowRota = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            allowRota = false;
        }

        if (allowRota)
        {
            transform.Rotate(transform.up, -Input.GetAxis("Mouse X") * Time.deltaTime * speed);
        }
    }
}
