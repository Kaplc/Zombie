using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
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
    private int weaponIndex;
    private Weapon weapon;
    public Transform handPos;
    private Dictionary<int, GameObject> weaponDic;

    public Transform hungGunFirePos;
    public Transform hungGunFirePosCrouch;

    private void Start()
    {
        animator = GetComponent<Animator>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        weaponIndex = 3;
        ChangeWeapon();
    }

    private void Update()
    {
        Move();

        GetKeyNumToChangeWeapon();

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
    }

    #region 手枪和刀动作

    private void Move()
    {
        yDir = Input.GetAxis("Vertical");
        xDir = Input.GetAxis("Horizontal");

        // 按下shift跑动
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // 在后退时不能跑动
            if (yDir > 0)
                yDir += 1;


            if (xDir > 0)
                xDir += 1;
            
            if (xDir < 0)
                xDir -= 1;
        }

        // s后退时左右移动
        if (yDir < 0)
        {
            // 限制状态树的参数范围
            if (xDir > 0)
                xDir = Mathf.Clamp(xDir, 0f, 0.5f);

            if (xDir < 0)
                xDir = Mathf.Clamp(xDir, -0.5f, 0f);
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
            Roll();
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

    private void Roll()
    {
        animator.SetTrigger("Roll");
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
        Collider[] enemies =
            Physics.OverlapSphere(transform.position + transform.forward * 2 + transform.up, 1f, 1 << LayerMask.NameToLayer("Enemy"));
        foreach (Collider enemy in enemies)
        {
            enemy.GetComponent<Zombie>().Wound(3);
        }
    }

    public void ShootEvent()
    {
        if (isCrouch)
        {
            // 蹲下开火
            Debug.DrawRay(hungGunFirePosCrouch.position, hungGunFirePosCrouch.forward * 1000, Color.red);
            GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bullet.AddComponent<Bullet>();
            bullet.GetComponent<SphereCollider>().isTrigger = true;
            bullet.transform.position = hungGunFirePosCrouch.position;
            bullet.transform.rotation = hungGunFirePosCrouch.rotation;
            bullet.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            Destroy(bullet, 3);
            RaycastHit info;
            // 开枪射线检测
            if (Physics.Raycast(hungGunFirePosCrouch.position, hungGunFirePosCrouch.forward, out info, 1000, 1 << LayerMask.NameToLayer("Enemy")))
            {
                info.transform.gameObject.GetComponent<Zombie>().Wound(5);
            }
        }
        else
        {
            // 蹲下开火
            Debug.DrawRay(hungGunFirePos.position, hungGunFirePos.forward * 1000, Color.red);
            GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bullet.AddComponent<Bullet>();
            bullet.GetComponent<SphereCollider>().isTrigger = true;
            bullet.transform.position = hungGunFirePos.position;
            bullet.transform.rotation = hungGunFirePos.rotation;
            bullet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Destroy(bullet, 3);
            RaycastHit info;
            // 开枪射线检测
            if (Physics.Raycast(hungGunFirePos.position, hungGunFirePos.forward, out info, 1000, 1 << LayerMask.NameToLayer("Enemy")))
            {
                info.transform.gameObject.GetComponent<Zombie>().Wound(5);
            }
        }
    }

    #endregion

    #region 换武器

    private void GetKeyNumToChangeWeapon()
    {
        // 数字键
        if (Input.GetKeyDown(KeyCode.Alpha1) && weaponIndex != 1)
        {
            // 主武器
            ChangeWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && weaponIndex != 2)
        {
            // 副武器
            weaponIndex = 2;
            ChangeWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && weaponIndex != 3)
        {
            // 近战武器
            weaponIndex = 3;
            ChangeWeapon();
        }
    }

    private void ChangeWeapon()
    {
        if (weapon != null)
        {
            Destroy(weapon.gameObject);
        }

        switch (weaponIndex)
        {
            case 1:
                break;
            case 2:
                weapon = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/weapon_handgun"), handPos).GetComponent<Weapon>();
                animator.runtimeAnimatorController = Instantiate(Resources.Load<RuntimeAnimatorController>("Animator/Role/HandGun"));
                if (isCrouch)
                {
                    Crouch();
                }
                break;
            case 3:
                weapon = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/weapon_knife"), handPos).GetComponent<Weapon>();
                animator.runtimeAnimatorController = Instantiate(Resources.Load<RuntimeAnimatorController>("Animator/Role/Knife"));
                if (isCrouch)
                {
                    Crouch();
                }
                break;
        }
    }

    #endregion
}