using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    // 动作相关
    private Animator animator;
    public float moveSpeed;

    // 轴向
    private float xDir;
    private float yDir;

    private bool isCrouch; // 蹲下
    private Coroutine crouchCoroutine;

    private bool isProne;
    private Coroutine proneCoroutine;

    // 武器相关
    private Weapon weapon;
    public Transform handPos;


    private void Start()
    {
        animator = GetComponent<Animator>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Move();

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
    }

    private void Move()
    {
        yDir = Input.GetAxis("Vertical");
        xDir = Input.GetAxis("Horizontal");

        // 按下shift跑动
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // 在后退时不能跑动
            if (yDir > 0)
            {
                yDir += 1;
            }

            if (xDir > 0)
            {
                xDir += 1;
            }

            if (xDir < 0)
            {
                xDir -= 1;
            }
        }

        // 禁止s后退时左右移动
        if (yDir < 0)
        {
            xDir = 0;
        }

        Crouch();
        Prone();

        // 人物移动
        animator.SetFloat("XSpeed", Mathf.Lerp(animator.GetFloat("XSpeed"), xDir, Time.deltaTime * moveSpeed));
        animator.SetFloat("YSpeed", Mathf.Lerp(animator.GetFloat("YSpeed"), yDir, Time.deltaTime * moveSpeed));
        // 镜头随人物旋转
        transform.rotation *= Quaternion.Euler(transform.up * Input.GetAxis("Mouse X"));
    }

    #region 动作

    private void Crouch()
    {
        // 按下c蹲下
        if (Input.GetKeyDown(KeyCode.C))
        {
            // 趴下时按蹲下强制起身
            if (isProne)
            {
                isProne = !isProne;
                animator.SetBool("Prone", isProne);
            }
            
            if (crouchCoroutine != null)
            {
                StopCoroutine(crouchCoroutine);
            }

            isCrouch = !isCrouch;
            crouchCoroutine = StartCoroutine(CrouchAnimation(isCrouch));
        }
    }

    private void Prone()
    {
        // 按下z趴下
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // 当前状态非蹲下不用重置
            if (animator.GetLayerWeight(animator.GetLayerIndex("Crouch")) > 0.1f)
            {
                // 重置蹲下分层权重
                if (crouchCoroutine != null)
                {
                    StopCoroutine(crouchCoroutine);
                }

                isCrouch = !isCrouch;
                crouchCoroutine = StartCoroutine(CrouchAnimation(isCrouch));
            }

            // 触发prone参数
            isProne = !isProne;
            animator.SetBool("Prone", isProne);
        }

        // 趴下禁止左右
        if (isProne)
        {
            xDir = 0;
        }
    }

    #endregion

    #region 动作过度协程

    /// <summary>
    /// 蹲下优化动画协程
    /// </summary>
    /// <param name="isCrouch"></param>
    /// <returns></returns>
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

    #endregion

    #region 动作事件

    public void KnifeEvent()
    {
        // 范围检测
        Collider[] enemies = Physics.OverlapSphere(transform.forward, 0.5f, 1 << LayerMask.NameToLayer("Enemy"));
        foreach (Collider enemy in enemies)
        {
            
        }
    }

    #endregion
}