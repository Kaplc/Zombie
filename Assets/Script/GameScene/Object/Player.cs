using System;
using System.Collections;
using System.Collections.Generic;
using Script.FrameWork.MusicManager;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

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

    // private Weapon weapon;
    public WeaponBag weaponBag;
    public Transform handPos;
    private Dictionary<int, WeaponBag> weaponBagDic;

    // 开火点
    public Transform firePos;
    public Transform crouchFirePos;
    public Transform rocketFirePos;
    public Transform rocketCrouchFirePos;
    private Vector3 fireRayOrigin;
    private Ray fireRay;

    // 属性
    public float baseAtk;
    private float maxHp;
    private float hp;

    // 动画过度协程
    public float transitionSpeed;
    private Coroutine addMoveShootWeightCoroutine;
    private Coroutine addReloadShootWeightCoroutine;
    private Coroutine subMoveShootWeightCoroutine;

    private Coroutine subReloadShootWeightCoroutine;

    // 开枪音效组件
    private AudioSource machineGunAudioSource;

    private AudioSource hunGunAudioSource;

    // 
    private Vector3 mousePosToWorldPos;
    private Vector3 mousePosToWorldPosForward;

    private void Start()
    {
        animator = GetComponent<Animator>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // 初始化武器背包
        weaponBagDic = new Dictionary<int, WeaponBag>();
        weaponIndex = 3;
        ChangeWeapon();
        // 属性
        maxHp = hp = DataManager.Instance.roleInfos[DataManager.Instance.nowRoleID].hp;
        baseAtk = DataManager.Instance.roleInfos[DataManager.Instance.nowRoleID].baseAtk;
        RestoreBullet();
        // 初始化面板血量
        UIManager.Instance.GetPanel<GamePanel>().UpdatePlayerHp(hp, maxHp);
        // 初始化音乐
        machineGunAudioSource = gameObject.AddComponent<AudioSource>();
        machineGunAudioSource.clip = Resources.Load<AudioClip>("Music/Tower");
    }

    private void Update()
    {
        Move();

        if (GameManger.Instance.isGameOver || GameManger.Instance.showMenu)
        {
            animator.SetBool("Attack", false);
            return;
        }

        // 读取数字键换武器
        GetKeyNumToChangeWeapon();

        // 攻击
        switch (weaponBag.weapon.type)
        {
            case E_Weapon.Knife:
                if (Input.GetMouseButton(0) && !animator.GetBool("Roll"))
                {
                    Attack();
                }

                break;
            case E_Weapon.RocketLauncher:
            case E_Weapon.HandGun:
                // 点射
                if (Input.GetMouseButtonDown(0) && !animator.GetBool("Roll"))
                {
                    // 有后备子弹重新装填
                    if (weaponBag.spareBullets > 0 && weaponBag.weapon.nowBulletCount == 0)
                    {
                        animator.SetBool("Reloading", true);
                        return;
                    }

                    // 枪里有子弹才攻击
                    if (weaponBag.weapon.nowBulletCount > 0 && weaponBag.spareBullets > 0 ||
                        weaponBag.weapon.nowBulletCount > 0 && weaponBag.spareBullets == 0)
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
                if (Input.GetMouseButton(0) && !animator.GetBool("Roll"))
                {
                    // 有后备子弹重新装填
                    if (weaponBag.spareBullets > 0 && weaponBag.weapon.nowBulletCount == 0)
                    {
                        animator.SetBool("Attack", false);
                        animator.SetBool("Reloading", true);
                        return;
                    }

                    // 枪里有子弹才攻击
                    if (weaponBag.weapon.nowBulletCount > 0 && weaponBag.spareBullets > 0 ||
                        weaponBag.weapon.nowBulletCount > 0 && weaponBag.spareBullets == 0)
                    {
                        Attack();
                    }
                    // 无子弹提示
                    else if (weaponBag.weapon.nowBulletCount == 0 && weaponBag.spareBullets == 0)
                    {
                        animator.SetBool("Attack", false);
                        UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("弹药不足");
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    animator.SetBool("Attack", false);
                }

                break;
        }

        // 手动装弹
        if (Input.GetKeyDown(KeyCode.R) && weaponBag.weapon.nowBulletCount != weaponBag.weapon.bulletCount)
        {
            // 当前武器有后备子弹才能重新装填
            if (weaponBag.spareBullets > 0)
            {
                animator.SetBool("Reloading", true);
            }
        }
    }

    private void Attack()
    {
        // 手枪和机枪有不同的动画状态机
        switch (weaponBag.weapon.type)
        {
            case E_Weapon.Knife:
            case E_Weapon.RocketLauncher:
            case E_Weapon.HandGun:
                animator.SetTrigger("Attack");
                break;
            case E_Weapon.MachineGun:
                animator.SetBool("Attack", true);
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
        if (Input.GetKeyDown(KeyCode.X) && !animator.GetBool("Reloading"))
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
            if (weaponBag.weapon.type != E_Weapon.Knife)
            {
                subReloadShootWeightCoroutine = StartCoroutine(SubWeight("MoveReload"));
            }

            addMoveShootWeightCoroutine = StartCoroutine(AddWeight("MoveShoot"));
        }
        // 移动装弹层：不在roll时切换
        else if ((xDir != 0 || yDir != 0) && animator.GetBool("Reloading") && !animator.GetBool("Roll"))
        {
            StopAllCoroutine();
            subMoveShootWeightCoroutine = StartCoroutine(SubWeight("MoveShoot"));
            if (weaponBag.weapon.type != E_Weapon.Knife)
            {
                addReloadShootWeightCoroutine = StartCoroutine(AddWeight("MoveReload"));
            }
        }
        // 停下攻击层： roll或者停下时切换
        else if (xDir == 0 && yDir == 0 || animator.GetBool("Roll"))
        {
            // 装弹时不能滚
            if (!animator.GetBool("Reloading"))
            {
                StopAllCoroutine();
                if (weaponBag.weapon.type == E_Weapon.Knife)
                {
                    animator.SetLayerWeight(animator.GetLayerIndex("MoveShoot"), 0f);
                }
                else
                {
                    subMoveShootWeightCoroutine = StartCoroutine(SubWeight("MoveShoot"));
                    subReloadShootWeightCoroutine = StartCoroutine(SubWeight("MoveReload"));
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
        animator.SetLayerWeight(animator.GetLayerIndex("MoveShoot"), 0);
        animator.SetBool("Attack", false);
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
            animator.SetLayerWeight(animator.GetLayerIndex(layerName),
                Mathf.Lerp(animator.GetLayerWeight(animator.GetLayerIndex(layerName)), 1, Time.deltaTime * transitionSpeed));
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
            animator.SetLayerWeight(animator.GetLayerIndex(layerName),
                Mathf.Lerp(animator.GetLayerWeight(animator.GetLayerIndex(layerName)), 0, Time.deltaTime * transitionSpeed));
            yield return null;
        }
    }

    // 停止所有过渡协程
    private void StopAllCoroutine()
    {
        if (addMoveShootWeightCoroutine != null)
        {
            StopCoroutine(addMoveShootWeightCoroutine);
        }

        if (addReloadShootWeightCoroutine != null)
        {
            StopCoroutine(addReloadShootWeightCoroutine);
        }

        if (subReloadShootWeightCoroutine != null)
        {
            StopCoroutine(subReloadShootWeightCoroutine);
        }

        if (subMoveShootWeightCoroutine != null)
        {
            StopCoroutine(subMoveShootWeightCoroutine);
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
            enemy.GetComponent<Zombie>().Wound(weaponBag.weapon.atk + baseAtk);
        }

        animator.SetBool("Attacking", false);
    }

    // 枪射击事件
    public void ShootEvent()
    {
        if (weaponBag.weapon.nowBulletCount == 0)
        {
            return;
        }

        // 根据鼠标位置生成开枪射线
        fireRayOrigin.x = Input.mousePosition.x;
        fireRayOrigin.y = Input.mousePosition.y;
        fireRayOrigin.z = 5;

        fireRay.origin = Camera.main.ScreenToWorldPoint(fireRayOrigin);
        fireRay.direction = Camera.main.ScreenPointToRay(Input.mousePosition).direction;

        if (isCrouch)
        {
            // 蹲下开火
            GameObject bullet = PoolManager.Instance.GetObject("Prefabs/Bullet/Bullet");
            if (bullet)
            {
                bullet.SetActive(false);
                bullet.transform.position = crouchFirePos.position;
                bullet.transform.rotation = crouchFirePos.rotation;
                bullet.SetActive(true);
            }
            else
            {
                ResourcesFrameWork.Instance.LoadAsync<GameObject>("Prefabs/Bullet/Bullet", resBullet =>
                {
                    bullet = Instantiate(resBullet);
                    bullet.name = "Prefabs/Bullet/Bullet";
                    bullet.transform.position = crouchFirePos.position;
                    bullet.transform.rotation = crouchFirePos.rotation;
                });
            }

            // 枪口火焰
            GameObject muzzle = PoolManager.Instance.GetObject("Prefabs/Bullet/Muzzle");
            if (muzzle)
            {
                muzzle.transform.position = crouchFirePos.position;
                muzzle.transform.rotation = crouchFirePos.rotation;
            }
            else
            {
                ResourcesFrameWork.Instance.LoadAsync<GameObject>("Prefabs/Bullet/Muzzle", resMuzzle =>
                {
                    muzzle = Instantiate(resMuzzle);
                    bullet.name = "Prefabs/Bullet/Muzzle";
                    muzzle.transform.position = crouchFirePos.position;
                    muzzle.transform.rotation = crouchFirePos.rotation;
                });
            }
        }
        else
        {
            // 起身开火
            GameObject bullet = PoolManager.Instance.GetObject("Prefabs/Bullet/Bullet");
            if (bullet)
            {
                bullet.SetActive(false);
                bullet.transform.position = firePos.position;
                bullet.transform.rotation = firePos.rotation;
                bullet.SetActive(true);
            }
            else
            {
                ResourcesFrameWork.Instance.LoadAsync<GameObject>("Prefabs/Bullet/Bullet", resBullet =>
                {
                    bullet = Instantiate(resBullet);
                    bullet.name = "Prefabs/Bullet/Bullet";
                    bullet.transform.position = firePos.position;
                    bullet.transform.rotation = firePos.rotation;
                });
            }

            // 枪口火焰
            GameObject muzzle = PoolManager.Instance.GetObject("Prefabs/Bullet/Muzzle");
            if (muzzle)
            {
                muzzle.transform.position = firePos.position;
                muzzle.transform.rotation = firePos.rotation;
            }
            else
            {
                ResourcesFrameWork.Instance.LoadAsync<GameObject>("Prefabs/Bullet/Muzzle", resMuzzle =>
                {
                    muzzle = Instantiate(resMuzzle);
                    muzzle.name = "Prefabs/Bullet/Muzzle";
                    muzzle.transform.position = firePos.position;
                    muzzle.transform.rotation = firePos.rotation;
                });
            }
        }

        // 开枪射线检测, 朝向准星方向
        if (Physics.Raycast(fireRay, out RaycastHit hitInfo, 1000, 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Buildings")))
        {
            //创建打击特效
            GameObject hitEff = PoolManager.Instance.GetObject("Prefabs/Bullet/HitEff");
            if (hitEff)
            {
                hitEff.transform.position = hitInfo.point;
                hitEff.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
            }
            else
            {
                ResourcesFrameWork.Instance.LoadAsync<GameObject>("Prefabs/Bullet/HitEff", resHitEff =>
                {
                    hitEff = Instantiate(resHitEff);
                    hitEff.name = "Prefabs/Bullet/HitEff";
                    hitEff.transform.position = hitInfo.point;
                    hitEff.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
                });
            }

            // 受伤
            hitInfo.transform.gameObject.GetComponent<Zombie>()?.Wound(weaponBag.weapon.atk + baseAtk);
        }

        // 减少子弹
        weaponBag.weapon.SubBullet();
        // 刷新开火图标
        UIManager.Instance.GetPanel<GamePanel>().RefreshBulletImg();
        // 更新面板子弹数
        UpdateInfoToPanel();

        // 播放开枪音效
        if (weaponBag.weapon.type == E_Weapon.HandGun)
        {
            MusicManger.Instance.PlaySound("Music/Gun", DataManager.Instance.musicData.soundVolume, false);
        }
        else if (weaponBag.weapon.type == E_Weapon.MachineGun)
        {
            machineGunAudioSource.volume = DataManager.Instance.musicData.soundVolume;
            machineGunAudioSource.mute = DataManager.Instance.musicData.soundMute;
            // 音效裁剪
            if (machineGunAudioSource.time >= 0.4f)
            {
                machineGunAudioSource.time = 0.15f;
                machineGunAudioSource.Play();
            }
            else if (!machineGunAudioSource.isPlaying)
            {
                machineGunAudioSource.Play();
            }
        }
    }

    // 火箭筒射击事件
    public void RocketShootEvent()
    {
        if (isCrouch)
        {
            // 起身开火
            GameObject bullet = PoolManager.Instance.GetObject("Prefabs/Bullet/RocketBullet");
            if (bullet)
            {
                bullet.SetActive(false);
                bullet.transform.position = rocketCrouchFirePos.position;
                bullet.transform.rotation = rocketCrouchFirePos.rotation;


                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                rb.AddForce(bullet.transform.forward * 0.1f, ForceMode.Impulse);
                bullet.SetActive(true);
            }
            else
            {
                ResourcesFrameWork.Instance.LoadAsync<GameObject>("Prefabs/Bullet/RocketBullet", resBullet =>
                {
                    bullet.SetActive(false);
                    bullet = Instantiate(resBullet);
                    bullet.name = "Prefabs/Bullet/RocketBullet";
                    bullet.transform.position = rocketCrouchFirePos.position;
                    bullet.transform.rotation = rocketCrouchFirePos.rotation;

                    Rigidbody rb = bullet.GetComponent<Rigidbody>();
                    rb.velocity = Vector3.zero;
                    rb.AddForce(bullet.transform.forward * 0.1f, ForceMode.Impulse);
                    bullet.SetActive(true);
                });
            }

            // 枪口火焰
            GameObject muzzle = PoolManager.Instance.GetObject("Prefabs/Bullet/Muzzle");
            if (muzzle)
            {
                muzzle.transform.position = rocketCrouchFirePos.position;
                muzzle.transform.rotation = rocketCrouchFirePos.rotation;
            }
            else
            {
                ResourcesFrameWork.Instance.LoadAsync<GameObject>("Prefabs/Bullet/Muzzle", resMuzzle =>
                {
                    muzzle = Instantiate(resMuzzle);
                    muzzle.name = "Prefabs/Bullet/Muzzle";
                    muzzle.transform.position = rocketCrouchFirePos.position;
                    muzzle.transform.rotation = rocketCrouchFirePos.rotation;
                });
            }
        }
        else
        {
            // 起身开火
            GameObject bullet = PoolManager.Instance.GetObject("Prefabs/Bullet/RocketBullet");
            if (bullet)
            {
                bullet.SetActive(false);
                bullet.transform.position = rocketFirePos.position;
                bullet.transform.rotation = rocketFirePos.rotation;


                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                rb.AddForce(bullet.transform.forward * 0.1f, ForceMode.Impulse);
                bullet.SetActive(true);
            }
            else
            {
                ResourcesFrameWork.Instance.LoadAsync<GameObject>("Prefabs/Bullet/RocketBullet", resBullet =>
                {
                    bullet = Instantiate(resBullet);
                    bullet.name = "Prefabs/Bullet/RocketBullet";
                    bullet.transform.position = rocketFirePos.position;
                    bullet.transform.rotation = rocketFirePos.rotation;

                    Rigidbody rb = bullet.GetComponent<Rigidbody>();
                    rb.velocity = Vector3.zero;
                    rb.AddForce(bullet.transform.forward * 0.1f, ForceMode.Impulse);
                    bullet.SetActive(true);
                });
            }

            // 枪口火焰
            GameObject muzzle = PoolManager.Instance.GetObject("Prefabs/Bullet/Muzzle");
            if (muzzle)
            {
                muzzle.transform.position = rocketFirePos.position;
                muzzle.transform.rotation = rocketFirePos.rotation;
            }
            else
            {
                ResourcesFrameWork.Instance.LoadAsync<GameObject>("Prefabs/Bullet/Muzzle", resMuzzle =>
                {
                    muzzle = Instantiate(resMuzzle);
                    muzzle.name = "Prefabs/Bullet/Muzzle";
                    muzzle.transform.position = rocketFirePos.position;
                    muzzle.transform.rotation = rocketFirePos.rotation;
                });
            }
        }

        // 减少子弹
        weaponBag.weapon.SubBullet();
        // 刷新开火图标
        UIManager.Instance.GetPanel<GamePanel>().RefreshBulletImg();
        // 更新面板子弹数
        UpdateInfoToPanel();
    }

    public void EndReloadEvent()
    {
        // 先加回枪里剩下的子弹再重新装填
        weaponBag.spareBullets += weaponBag.weapon.nowBulletCount;
        // 所有子弹加起来少于一个弹夹则填装剩下的全部子弹
        if (weaponBag.spareBullets < weaponBag.weapon.bulletCount)
        {
            weaponBag.weapon.ReLoading(weaponBag.spareBullets);
            weaponBag.spareBullets = 0;
        }
        else
        {
            weaponBag.weapon.ReLoading(weaponBag.weapon.bulletCount);
            weaponBag.spareBullets -= weaponBag.weapon.bulletCount;
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

        if (Input.GetKeyDown(KeyCode.G) && weaponIndex != 0)
        {
            weaponIndex = 0;
            ChangeWeapon();
        }
    }

    private void ChangeWeapon()
    {
        weaponBag?.weapon.gameObject.SetActive(false);

        switch (weaponIndex)
        {
            case 0:
                if (!weaponBagDic.ContainsKey(0))
                {
                    weaponBag = new WeaponBag(DataManager.Instance.roleInfos[DataManager.Instance.nowRoleID].rocketLaunchBullets,
                        Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/RocketLauncher"), handPos).GetComponent<Weapon>());
                    weaponBagDic.Add(0, weaponBag);
                }
                else
                {
                    weaponBag = weaponBagDic[0];
                    weaponBag.weapon.gameObject.SetActive(true);
                }

                animator.runtimeAnimatorController = Instantiate(Resources.Load<RuntimeAnimatorController>("Animator/Role/RocketLauncher"));
                break;
            case 1:
                // 没有副武器就创建有就取出
                if (!weaponBagDic.ContainsKey(1))
                {
                    // 创建机枪
                    weaponBag = new WeaponBag(DataManager.Instance.roleInfos[DataManager.Instance.nowRoleID].mainBullets,
                        Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/A"), handPos).GetComponent<Weapon>());
                    weaponBagDic.Add(1, weaponBag);
                }
                else
                {
                    // 第一把主武器为枪
                    weaponBag = weaponBagDic[1];
                    weaponBag.weapon.gameObject.SetActive(true);
                }
                
                animator.runtimeAnimatorController = Instantiate(Resources.Load<RuntimeAnimatorController>("Animator/Role/MachineGun"));
                break;
            case 2:
                // 没有副武器就创建有就取出
                if (!weaponBagDic.ContainsKey(2))
                {
                    weaponBag = new WeaponBag(DataManager.Instance.roleInfos[DataManager.Instance.nowRoleID].hanGunBullets,
                        Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/weapon_handgun"), handPos).GetComponent<Weapon>());
                    weaponBagDic.Add(2, weaponBag);
                }
                else
                {
                    weaponBag = weaponBagDic[2];
                    weaponBag.weapon.gameObject.SetActive(true);
                }

                animator.runtimeAnimatorController = Instantiate(Resources.Load<RuntimeAnimatorController>("Animator/Role/HandGun"));
                break;
            case 3:
                if (!weaponBagDic.ContainsKey(3))
                {
                    weaponBag = new WeaponBag(0,
                        Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/weapon_knife"), handPos).GetComponent<Weapon>());
                    weaponBagDic.Add(3, weaponBag);
                }
                else
                {
                    weaponBag = weaponBagDic[3];
                    weaponBag.weapon.gameObject.SetActive(true);
                }

                animator.runtimeAnimatorController = Instantiate(Resources.Load<RuntimeAnimatorController>("Animator/Role/Knife"));
                break;
        }

        // 每次切换武器重置动画分层权重
        StopAllCoroutine();
        if (isCrouch)
        {
            animator.SetBool("Crouch", true);
        }

        // 立刻恢复当前人物移动
        animator.SetFloat("XSpeed", Mathf.Lerp(animator.GetFloat("XSpeed"), xDir, Time.deltaTime * moveSpeed));
        animator.SetFloat("YSpeed", Mathf.Lerp(animator.GetFloat("YSpeed"), yDir, Time.deltaTime * moveSpeed));

        if (yDir != 0 || xDir != 0)
        {
            animator.SetLayerWeight(animator.GetLayerIndex("MoveShoot"), 0.5f);
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
        UIManager.Instance.GetPanel<GamePanel>()?.UpdateBullet(weaponBag.weapon.nowBulletCount, weaponBag.spareBullets);
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
        // 默认为false禁止满子弹加子弹
        bool isRestoreBullet = false;
        for (int i = 0; i < weaponBagDic.Count; i++)
        {
            if (weaponBagDic.ContainsKey(i))
            {
                // 遍历所有武器, 不满子弹的加满
                if (weaponBagDic[i].spareBullets != weaponBagDic[i].resSpareBullets)
                {
                    weaponBagDic[i].spareBullets = weaponBagDic[i].resSpareBullets;
                    isRestoreBullet = true;
                }
            }
        }

        UpdateInfoToPanel();
        return isRestoreBullet;
    }

    public void AddAtk()
    {
        baseAtk += 0.25f;
        UpdateInfoToPanel();
    }

    #endregion
}