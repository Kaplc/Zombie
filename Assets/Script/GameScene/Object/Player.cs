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

    // 武器相关
    private int weaponIndex;
    private Weapon weapon;
    public Transform handPos;
    private int hanGunBullets; // 手枪剩余子弹
    private int mainBullets; // 主武器剩余子弹
    private Dictionary<int, GameObject> weaponBag;

    // 开火点
    public Transform firePos;
    public Transform crouchFirePos;

    // 属性
    private float baseAtk;
    private float maxHp;
    private float hp;
    
    // 动画过度协程
    public float transitionSpeed;
    private Coroutine addWeightCoroutine;
    private Coroutine subWeightCoroutine;
    
    
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
        RestoreBullet();

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
        switch (weapon.type)
        {
            case E_Weapon.Knife:
            case E_Weapon.HandGun:
                // 点射
                if (Input.GetMouseButtonDown(0))
                {
                    // 有后备子弹重新装填
                    if (hanGunBullets > 0 && weapon.nowBulletCount == 0)
                    {
                        animator.SetBool("Reloading", true);
                        return;
                    }

                    // 枪里有子弹才攻击
                    if (weapon.nowBulletCount > 0 && hanGunBullets > 0 || weapon.nowBulletCount > 0 && hanGunBullets == 0)
                    {
                        Attack();
                    }
                    else // 无子弹提示
                    {
                        UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("弹药不足");
                    }
                }
                break;
            case E_Weapon.MachineGun:
                // 连射
                if (Input.GetMouseButton(0))
                {
                    // 有后备子弹重新装填
                    if (mainBullets > 0 && weapon.nowBulletCount == 0)
                    {
                        animator.SetBool("Attack", false);
                        animator.SetBool("Reloading", true);
                        return;
                    }

                    // 枪里有子弹才攻击
                    if (weapon.nowBulletCount > 0 && mainBullets > 0 || weapon.nowBulletCount > 0 && mainBullets == 0)
                    {
                        Attack();
                    }
                    // 无子弹提示
                    else if (weapon.nowBulletCount == 0 && mainBullets == 0 )
                    {
                        UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("弹药不足");
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    animator.SetBool("Attack", false);
                }
                break;
            case E_Weapon.RocketLauncher:
                break;
        }
        
        // 手动装弹
        if (Input.GetKeyDown(KeyCode.R) && weapon.nowBulletCount != weapon.bulletCount)
        {
            // 当前武器有后备子弹才能重新装填
            if (hanGunBullets > 0 && weapon.type == E_Weapon.HandGun ||mainBullets > 0 && weapon.type == E_Weapon.MachineGun)
            {
                animator.SetBool("Reloading", true);
            }
        }
    }

    private void Attack()
    {
        switch (weapon.type)
        {
            case E_Weapon.Knife:
            case E_Weapon.HandGun:
                animator.SetBool("Attacking", true);
                animator.SetTrigger("Attack");
                break;
            case E_Weapon.MachineGun:
                animator.SetBool("Attack", true);
                break;
            case E_Weapon.RocketLauncher:
                break;
        }
        
    }

    public void Wound(float num)
    {
        hp = Mathf.Clamp(hp - num, 0, maxHp);
        if (!GameManger.Instance.isGameOver && hp <= 0)
        {
            animator.SetBool("Dead", true);
            GameManger.Instance.GameOver(false);
            UpdateInfoToPanel();
            return;
        }

        UpdateInfoToPanel();
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
            Crouch();
        }
        
        // 按x翻滚: 在装弹和攻击时无效
        if (Input.GetKeyDown(KeyCode.X) && !animator.GetBool("Reloading") && !animator.GetBool("Attacking"))
        {
            Roll();
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
        
        // 任意方向移动时切换为移动攻击层：不在roll时切换
        if ((xDir != 0 || yDir != 0) && !animator.GetBool("Reloading") && !animator.GetBool("Roll"))
        {
            StopAllCoroutine();
            if (weapon.type != E_Weapon.Knife)
            {
                subWeightCoroutine = StartCoroutine(SubWeight("MoveReload"));
            }
            addWeightCoroutine = StartCoroutine(AddWeight("MoveShoot"));
        }
        // 移动装弹层：不在roll时切换
        else if ((xDir != 0 || yDir != 0) && animator.GetBool("Reloading") && !animator.GetBool("Roll"))
        {
            StopAllCoroutine();
            subWeightCoroutine = StartCoroutine(SubWeight("MoveShoot"));
            if (weapon.type != E_Weapon.Knife)
            {
                addWeightCoroutine = StartCoroutine(AddWeight("MoveReload"));
            }
        }
        // 停下攻击层： roll或者停下时切换
        else if (xDir == 0 && yDir == 0 || animator.GetBool("Roll"))
        {
            // 装弹时不能滚
            if (!animator.GetBool("Reloading"))
            {
                StopAllCoroutine();
                if (weapon.type == E_Weapon.Knife)
                {
                    animator.SetLayerWeight(animator.GetLayerIndex("MoveShoot"), 0f);
                }
                else
                {
                    subWeightCoroutine = StartCoroutine(SubWeight("MoveShoot"));
                    subWeightCoroutine = StartCoroutine(SubWeight("MoveReload"));
                }
            }
        }
        

        // 人物移动
        animator.SetFloat("XSpeed", Mathf.Lerp(animator.GetFloat("XSpeed"), xDir, Time.deltaTime * moveSpeed));
        animator.SetFloat("YSpeed", Mathf.Lerp(animator.GetFloat("YSpeed"), yDir, Time.deltaTime * moveSpeed));
    }

    private void Crouch()
    {
        isCrouch = !isCrouch;
        animator.SetBool("Crouch", isCrouch);
    }
    
    private void Roll()
    {
        StopAllCoroutine();
        animator.SetBool("Roll", true);
    }

    #endregion

    #region 动作过度相关

    /// <summary>
    /// 优化动画分层启动时的过度协程
    /// </summary>
    /// <param name="layerName">动画层名</param>
    /// <returns></returns>
    private IEnumerator AddWeight(string layerName)
    {
        while (animator.GetLayerWeight(animator.GetLayerIndex(layerName)) < 1f)
        {
            animator.SetLayerWeight(animator.GetLayerIndex(layerName), Mathf.Lerp(animator.GetLayerWeight(animator.GetLayerIndex(layerName)), 1, Time.deltaTime * transitionSpeed));
            yield return null;
        }
    }

    /// <summary>
    /// 优化动画分层关闭时的过度协程
    /// </summary>
    /// <param name="layerName">动画层名</param>
    /// <returns></returns>
    private IEnumerator SubWeight(string layerName)
    {
        while (animator.GetLayerWeight(animator.GetLayerIndex(layerName)) > 0.001f)
        {
            animator.SetLayerWeight(animator.GetLayerIndex(layerName), Mathf.Lerp(animator.GetLayerWeight(animator.GetLayerIndex(layerName)), 0, Time.deltaTime * transitionSpeed));
            yield return null;
        }
    }
    
    // 停止所有过渡协程
    private void StopAllCoroutine()
    {
        if (addWeightCoroutine != null)
        {
            StopCoroutine(addWeightCoroutine);
        }

        if (subWeightCoroutine != null)
        {
            StopCoroutine(subWeightCoroutine);
        }
    }
    #endregion

    #region 动作事件

    public void RollEndEvent()
    {
        animator.SetBool("Roll", false);
    }
    
    public void KnifeEvent()
    {
        // 范围检测
        Collider[] enemies =
            Physics.OverlapSphere(transform.position + transform.forward * 2 + transform.up, 1f, 1 << LayerMask.NameToLayer("Enemy"));
        foreach (Collider enemy in enemies)
        {
            enemy.GetComponent<Zombie>().Wound(weapon.atk + baseAtk);
        }
        animator.SetBool("Attacking", false);
    }

    public void ShootEvent()
    {
        if (weapon.nowBulletCount == 0)
        {
            return;
        }
        
        if (isCrouch)
        {
            // 蹲下开火
            Debug.DrawRay(crouchFirePos.position, crouchFirePos.forward * 1000, Color.red);
            GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet/Bullet"));
            bullet.transform.position = crouchFirePos.position;
            bullet.transform.rotation = crouchFirePos.rotation;
            Destroy(bullet, 0.5f);
            RaycastHit info;

            // 开枪射线检测
            if (Physics.Raycast(crouchFirePos.position, crouchFirePos.forward, out info, 1000, 1 << LayerMask.NameToLayer("Enemy")))
            {
                info.transform.gameObject.GetComponent<Zombie>().Wound(weapon.atk + baseAtk);
            }
        }
        else
        {
            // 起身开火
            Debug.DrawRay(firePos.position, firePos.forward * 1000, Color.red);
            GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet/Bullet"));
            bullet.transform.position = firePos.position;
            bullet.transform.rotation = firePos.rotation;
            Destroy(bullet, 0.5f);
            RaycastHit info;

            // 开枪射线检测
            if (Physics.Raycast(firePos.position, firePos.forward, out info, 1000, 1 << LayerMask.NameToLayer("Enemy")))
            {
                info.transform.gameObject.GetComponent<Zombie>().Wound(weapon.atk + baseAtk);
            }
        }

        // 减少子弹
        weapon.SubBullet();
        // 刷新开火图标
        UIManager.Instance.GetPanel<GamePanel>().RefreshBulletImg();
        // 更新面板子弹数
        UpdateInfoToPanel();
    }

    public void EndReloadEvent()
    {
        // 主武器
        if (weapon.type == E_Weapon.MachineGun)
        {
            // 先加回枪里剩下的子弹再重新装填
            mainBullets += weapon.nowBulletCount;
            // 所有子弹加起来少于一个弹夹则填装剩下的全部子弹
            if (mainBullets < weapon.bulletCount)
            {
                weapon.ReLoading(mainBullets);
                mainBullets = 0;
            }
            else
            {
                weapon.ReLoading(weapon.bulletCount);
                mainBullets -= weapon.bulletCount;
            }
        }
        // 手枪
        else
        {
            // 先加回枪里剩下的子弹再重新装填
            hanGunBullets += weapon.nowBulletCount;
            // 所有子弹加起来少于一个弹夹则填装剩下的全部子弹
            if (hanGunBullets < weapon.bulletCount)
            {
                weapon.ReLoading(hanGunBullets);
                hanGunBullets = 0;
            }
            else
            {
                weapon.ReLoading(weapon.bulletCount);
                hanGunBullets -= weapon.bulletCount;
            }
        }
        
        UpdateInfoToPanel();
        animator.SetBool("Reloading", false);
    }

    #endregion

    #region 换武器

    private void GetKeyNumToChangeWeapon()
    {
        // 数字键
        if (Input.GetKeyDown(KeyCode.Alpha1) && weaponIndex != 1)
        {
            weaponIndex = 1;
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
                // 没有副武器就创建有就取出
                if (!weaponBag.ContainsKey(1))
                {
                    weapon = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/A"), handPos).GetComponent<Weapon>();
                    weaponBag.Add(1, weapon.gameObject);
                }
                else
                {
                    weapon = weaponBag[1].GetComponent<Weapon>();
                    weapon.gameObject.SetActive(true);
                }
                animator.runtimeAnimatorController = Instantiate(Resources.Load<RuntimeAnimatorController>("Animator/Role/MachineGun"));
                
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
                break;
        }
        if (isCrouch)
        {
            animator.SetBool("Crouch", true);
        }
        // 其他武器现实子弹数
        UIManager.Instance.GetPanel<GamePanel>().ShowBulletInfo();
        UIManager.Instance.GetPanel<GamePanel>().RefreshBulletImg();
        UpdateInfoToPanel();
        if (weaponIndex == 3)
        {
            // 近战武器隐藏子弹信息
            UIManager.Instance.GetPanel<GamePanel>().HideBulletInfo(); 
        }
    }

    #endregion

    private void UpdateInfoToPanel()
    {
        UIManager.Instance.GetPanel<GamePanel>()?.UpdateBullet(weapon.nowBulletCount, weapon.type == E_Weapon.HandGun?hanGunBullets : mainBullets);
        UIManager.Instance.GetPanel<GamePanel>()?.UpdatePlayerHp(hp, maxHp);
        UIManager.Instance.GetPanel<GamePanel>()?.UpdateAtk(baseAtk);
    }

    #region 补给相关

    public bool RestoreHealth()
    {
        // 禁止满血加血
        if ((int)maxHp == (int)hp)
        {
            UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("血量已满");
            return false;
        }
        
        hp = maxHp;
        UpdateInfoToPanel();
        return true;
    }

    public bool RestoreBullet()
    {
        // 禁止满子弹加子弹
        if (mainBullets == DataManager.Instance.roleInfos[DataManager.Instance.nowRoleID].mainBullets && hanGunBullets == DataManager.Instance.roleInfos[DataManager.Instance.nowRoleID].hanGunBullets)
        {
            UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("后备弹药已满");
            return false;
        }
        
        mainBullets = DataManager.Instance.roleInfos[DataManager.Instance.nowRoleID].mainBullets;
        hanGunBullets = DataManager.Instance.roleInfos[DataManager.Instance.nowRoleID].hanGunBullets;
        UpdateInfoToPanel();
        return true;
    }

    public void AddAtk()
    {
        baseAtk += 0.5f;
        UpdateInfoToPanel();
    }
    #endregion
}