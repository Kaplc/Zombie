﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    private Animator animator;
    public float moveSpeed;
    public float crouchSpeed;

    // 轴向
    private float xDir;
    private float yDir;

    private bool isCrouch; // 蹲下
    private Coroutine crouchCoroutine;

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

        if (yDir <= 0)
        {
            // 禁止s后退
            yDir = 0;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (crouchCoroutine != null)
            {
                StopCoroutine(crouchCoroutine);
            }
            isCrouch = !isCrouch;
            crouchCoroutine = StartCoroutine(CrouchAnimation(isCrouch));
        }
        

        animator.SetFloat("XSpeed", Mathf.Lerp(animator.GetFloat("XSpeed"), xDir, Time.deltaTime * moveSpeed));
        animator.SetFloat("YSpeed", Mathf.Lerp(animator.GetFloat("YSpeed"), yDir, Time.deltaTime * moveSpeed));

        transform.rotation *= Quaternion.Euler(transform.up * Input.GetAxis("Mouse X"));
    }

    private IEnumerator CrouchAnimation(bool isCrouch)
    {
        if (isCrouch)
        {
            while (animator.GetLayerWeight(1) < 1f)
            {
                // 蹲下
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1, Time.deltaTime * moveSpeed));
                yield return null;
            }
        }
        else
        {
            while (animator.GetLayerWeight(1) > 0f)
            {
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0, Time.deltaTime * moveSpeed));
                yield return null;
            }
        }
    }
}