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
    private int weaponIndex;


    private void Start()
    {
        animator = GetComponent<Animator>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        ChangeWeapon();
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

    #region 拿刀动作

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

        // 按下c蹲下
        if (Input.GetKeyDown(KeyCode.C))
        {
            // 趴下时按c, 
            if (isProne)
            {
                ResetWeight(); // 重置权重
                isProne = false; // 取消标记为趴下
                animator.SetBool("Prone", false);
            }
            isCrouch = !isCrouch;
            Crouch();
        }

        // 按下z趴下
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isProne = !isProne;
            Prone();
        }

        // 按x翻滚
        if (Input.GetKeyDown(KeyCode.X))
        {
            ResetWeight();
        }

        // 趴下禁止左右
        if (isProne)
        {
            xDir = 0;
        }

        // 人物移动
        animator.SetFloat("XSpeed", Mathf.Lerp(animator.GetFloat("XSpeed"), xDir, Time.deltaTime * moveSpeed));
        animator.SetFloat("YSpeed", Mathf.Lerp(animator.GetFloat("YSpeed"), yDir, Time.deltaTime * moveSpeed));
        // 镜头随人物旋转
        transform.rotation *= Quaternion.Euler(transform.up * Input.GetAxis("Mouse X"));
    }

    private void Crouch()
    {
        // 趴下时按蹲下强制起身
        if (crouchCoroutine != null)
        {
            StopCoroutine(crouchCoroutine);
        }

        if (isCrouch)
            // true为蹲下
            crouchCoroutine = StartCoroutine(CrouchDownAnimation());
        else
            crouchCoroutine = StartCoroutine(CrouchUpAnimation());
    }

    private void Prone()
    {
        ResetWeight();
        // 触发prone参数
        animator.SetBool("Prone", isProne);
    }

    // 重置权重
    private void ResetWeight()
    {
        if (crouchCoroutine != null)
        {
            StopCoroutine(crouchCoroutine);
        }
        isCrouch = false;
        crouchCoroutine = StartCoroutine(CrouchUpAnimation());

    }

    #endregion

    #region 动作过度协程

    /// <summary>
    /// 蹲下优化动画协程
    /// </summary>
    /// <param name="isCrouch"></param>
    /// <returns></returns>
    private IEnumerator CrouchDownAnimation()
    {
        while (animator.GetLayerWeight(1) < 1f)
        {
            // 蹲下
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1, Time.deltaTime * moveSpeed));
            yield return null;
        }
    }

    /// <summary>
    /// 蹲下后起身动画
    /// </summary>
    /// <param name="isCrouch"></param>
    /// <returns></returns>
    private IEnumerator CrouchUpAnimation()
    {
        while (animator.GetLayerWeight(1) > 0f)
        {
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0, Time.deltaTime * moveSpeed));
            yield return null;
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

    public void ShootEvent()
    {
        
    }

    #endregion

    #region 换武器

    private void GetKeyNum()
    {
        // 数字键
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // 主武器
            ChangeWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // 副武器
            weaponIndex = 2;
            ChangeWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // 近战武器
            weaponIndex = 3;
            ChangeWeapon();
        }
    }

    private void ChangeWeapon()
    {
        switch (weaponIndex)
        {
            case 1:
                break;
            case 2:
                weapon = Instantiate(Resources.Load<Weapon>("Prefabs/Weapon/weapon_handgun"), handPos);
                break;
            case 3:
                weapon = Instantiate(Resources.Load<Weapon>("Prefabs/Weapon/weapon_knife"), handPos);
                animator = Instantiate(Resources.Load<Animator>("Animator/Role/Knife"));
                break;
        }
    }

    #endregion
}