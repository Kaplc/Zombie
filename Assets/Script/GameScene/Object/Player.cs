using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    private Animator animator;
    public float animaSpeed;
    
    private float xDir;
    private float yDir;

    private void Start()
    {
        animator = GetComponent<Animator>();
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    private void Update()
    {
        yDir = Input.GetAxis("Vertical");
        xDir = Input.GetAxis("Horizontal");
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // 按下shift跑步
            yDir += 1;
        }

        if (yDir>0)
        {
            animator.SetFloat("XSpeed", Mathf.Lerp(animator.GetFloat("XSpeed"), xDir, Time.deltaTime * animaSpeed));
            animator.SetFloat("YSpeed", Mathf.Lerp(animator.GetFloat("YSpeed"), yDir, Time.deltaTime * animaSpeed));
        }

        transform.rotation *= Quaternion.Euler(transform.up * Input.GetAxis("Mouse X"));

    }
}
