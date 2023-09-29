using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    // 动作相关
    public Animator animator;
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
    public int remainingBullets; // 剩余子弹
    private Dictionary<int, GameObject> weaponBag;

    // 开火点
    public Transform hungGunFirePos;
    public Transform hungGunFirePosCrouch;

    // 属性
    private int baseAtk;
    private float maxHp;
    private float hp;

    public bool reloading;

    private void Start()
    {
        animator = GetComponent<Animator>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // 初始化武器背包
        weaponBag = new Dictionary<int, GameObject>();
        weaponIndex = 3;
        ChangeWeapon();
        // 属性
        maxHp = hp = DataManager.Instance.roleInfos[DataManager.Instance.nowRoleID].hp;
        baseAtk = DataManager.Instance.roleInfos[DataManager.Instance.nowRoleID].baseAtk;

        UIManager.Instance.GetPanel<GamePanel>().UpdatePlayerHp(hp, maxHp);
    }

    private void Update()
    {
        Move();

        if (GameManger.Instance.isGameOver || GameManger.Instance.showMenu)
        {
            return;
        }
        
        // 读取数字键换武器
        GetKeyNumToChangeWeapon();
        
        // 攻击
        if (Input.GetMouseButtonDown(0))
        {
            // 有后备子弹重新装填
            if (remainingBullets > 0 && weapon.nowBulletCount == 0)
            {
                animator.SetBool("Reloading", true);
                return;
            }

            // 枪里有子弹才攻击
            if (weapon.nowBulletCount > 0 && remainingBullets > 0 || weapon.nowBulletCount > 0 && remainingBullets == 0)
            {
                Attack();
            }
            else // 无子弹提示
            {
                UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("弹药不足");
            }
        }
        
        // 手动装弹
        if (Input.GetKeyDown(KeyCode.R) && weapon.nowBulletCount != weapon.bulletCount)
        {
            // 有后备子弹才能重新装填
            if (remainingBullets > 0)
            {
                animator.SetBool("Reloading", true);
            }
        }
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
    }

    public void Wound(float num)
    {
        hp = Mathf.Clamp(hp - num, 0, maxHp);
        if (!GameManger.Instance.isGameOver && hp <= 0)
        {
            animator.SetBool("Dead", true);
            GameManger.Instance.GameOver(false);
            UIManager.Instance.GetPanel<GamePanel>().UpdatePlayerHp(hp, maxHp);
            return;
        }

        UIManager.Instance.GetPanel<GamePanel>().UpdatePlayerHp(hp, maxHp);
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
            if (yDir > 0) yDir += 1;
            if (xDir > 0) xDir += 1;
            if (xDir < 0) xDir -= 1;
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

        // 游戏结束或者打开面板停止移动
        if (GameManger.Instance.isGameOver || GameManger.Instance.showMenu)
        {
            xDir = 0;
            yDir = 0;
        }
        else
        {
            // 镜头随人物旋转
            transform.rotation *= Quaternion.Euler(transform.up * Input.GetAxis("Mouse X"));
        }

        // 人物移动
        animator.SetFloat("XSpeed", Mathf.Lerp(animator.GetFloat("XSpeed"), xDir, Time.deltaTime * moveSpeed));
        animator.SetFloat("YSpeed", Mathf.Lerp(animator.GetFloat("YSpeed"), yDir, Time.deltaTime * moveSpeed));
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
            GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet/Bullet"));
            bullet.transform.position = hungGunFirePosCrouch.position;
            bullet.transform.rotation = hungGunFirePosCrouch.rotation;
            Destroy(bullet, 0.5f);
            RaycastHit info;

            // 开枪射线检测
            if (Physics.Raycast(hungGunFirePosCrouch.position, hungGunFirePosCrouch.forward, out info, 1000, 1 << LayerMask.NameToLayer("Enemy")))
            {
                info.transform.gameObject.GetComponent<Zombie>().Wound(5);
            }
        }
        else
        {
            // 起身开火
            Debug.DrawRay(hungGunFirePos.position, hungGunFirePos.forward * 1000, Color.red);
            GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet/Bullet"));
            bullet.transform.position = hungGunFirePos.position;
            bullet.transform.rotation = hungGunFirePos.rotation;
            Destroy(bullet, 0.5f);
            RaycastHit info;

            // 开枪射线检测
            if (Physics.Raycast(hungGunFirePos.position, hungGunFirePos.forward, out info, 1000, 1 << LayerMask.NameToLayer("Enemy")))
            {
                info.transform.gameObject.GetComponent<Zombie>().Wound(5);
            }
        }

        // 减少子弹
        weapon.SubBullet();
        // 刷新开火图标
        UIManager.Instance.GetPanel<GamePanel>().RefreshBulletImg();
        // 更新面板子弹数
        UpdateBulletCountToPanel();
    }

    public void EndReloadEvent()
    {
        // 先加回枪里剩下的子弹再重新装填
        remainingBullets += weapon.nowBulletCount;
        // 所有子弹加起来少于一个弹夹则填装剩下的全部子弹
        if (remainingBullets < weapon.bulletCount)
        {
            weapon.ReLoading(remainingBullets);
            remainingBullets = 0;
        }
        else
        {
            weapon.ReLoading(weapon.bulletCount);
            remainingBullets -= weapon.bulletCount;
        }
        UpdateBulletCountToPanel();
        animator.SetBool("Reloading", false);
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
        if (weapon)
        {
            weapon.gameObject.SetActive(false);
        }
        
        switch (weaponIndex)
        {
            case 1:
                break;
            case 2:
                // 没有副武器就创建有就取出
                if (!weaponBag.ContainsKey(2))
                {
                    weapon = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/weapon_handgun"), handPos).GetComponent<Weapon>();
                    weaponBag.Add(2, weapon.gameObject);
                }
                else
                {
                    weapon = weaponBag[2].GetComponent<Weapon>();
                    weapon.gameObject.SetActive(true);
                }

                animator.runtimeAnimatorController = Instantiate(Resources.Load<RuntimeAnimatorController>("Animator/Role/HandGun"));
                if (isCrouch)
                    Crouch();
                // 其他武器现实子弹数
                UIManager.Instance.GetPanel<GamePanel>().ShowBulletInfo();

                UpdateBulletCountToPanel();
                break;
            case 3:

                if (!weaponBag.ContainsKey(3))
                {
                    weapon = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/weapon_knife"), handPos).GetComponent<Weapon>();
                    weaponBag.Add(3,weapon.gameObject);
                }
                else
                {
                    weapon = weaponBag[3].GetComponent<Weapon>();
                    weapon.gameObject.SetActive(true);
                }
                
                animator.runtimeAnimatorController = Instantiate(Resources.Load<RuntimeAnimatorController>("Animator/Role/Knife"));
                if (isCrouch)
                    Crouch();

                UpdateBulletCountToPanel();
                // 近战武器隐藏子弹信息
                UIManager.Instance.GetPanel<GamePanel>().HideBulletInfo();
                break;
        }
    }

    #endregion

    private void UpdateBulletCountToPanel()
    {
        UIManager.Instance.GetPanel<GamePanel>().UpdateBullet(weapon.nowBulletCount, remainingBullets);
    }
}