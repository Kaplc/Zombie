using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform head;
    public Transform barrel; // 枪管
    public Transform target;

    public bool isFire;

    private float lastFindTime;
    private float lastFireTime;
    public float fireCd;

    public int lv;
    public int nextLvMoney;
    
    private bool angelSure;
    private Vector2 tarDir;
    private Vector2 selfDir;

    private RaycastHit hitInfo;

    private void Update()
    {
        // 每1秒进行范围检测敌人
        if (Time.time - lastFindTime >= 0.5f)
        {
            lastFindTime = Time.time;
            // 无目标或者目标已经死亡重新索敌
            if (!target || target.GetComponent<Zombie>().isDead)
            {
                // 重置角度未确认
                angelSure = false;
                FindTarget();
            }
        }
        
        if (target && !target.GetComponent<Zombie>().isDead && !GameManger.Instance.isGameOver)
        {
            Fire();
        }
    }

    private void FindTarget()
    {
        Collider[] zombies = Physics.OverlapSphere(transform.position, 10, 1 << LayerMask.NameToLayer("Enemy"));
        // 只锁定一个敌人
        if (zombies.Length != 0)
        {
            target = zombies[0].transform;
        }
    }

    private void Fire()
    {
        // 开火转动炮管
        barrel.rotation *= Quaternion.AngleAxis(5f, Vector3.forward);
        // 头部锁定
        head.rotation = Quaternion.Lerp(head.rotation,
            Quaternion.LookRotation(new Vector3(target.position.x, head.position.y + 1.5f, target.position.z) - head.position), Time.deltaTime * 5);
        // 进行角度确认
        if (!angelSure && CalAngleLessThen(target.position - transform.position, head.forward, 8f))
        {
            angelSure = true;
        }
        
        // 满足开火cd和角度才开火
        if (Time.time - lastFireTime > fireCd && angelSure)
        {
            GameObject bullet = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet/Bullet"));
            bullet.transform.position = barrel.position;
            bullet.transform.rotation = head.rotation;
            
             
            // 射线检测创建打击特效
            if (Physics.Raycast(head.position, head.forward, out hitInfo, 1000f, 1<< LayerMask.NameToLayer("Enemy")))
            {
                GameObject hitEff = Instantiate(Resources.Load<GameObject>("Prefabs/Bullet/HitEff"), hitInfo.transform);
                hitEff.transform.position = hitInfo.point;
                hitEff.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
                Destroy(hitEff, 1f);
            }
            
            lastFireTime = Time.time;
            target.GetComponent<Zombie>().Wound(1f);
        }
    }
    /// <summary>
    /// 计算水平角度是否小于目标角度
    /// </summary>
    /// <param name="a">向量a</param>
    /// <param name="b">向量b </param>
    /// <param name="tarAngel">目标角度</param>
    /// <returns></returns>
    private bool CalAngleLessThen(Vector3 a , Vector3 b, float tarAngel)
    {
        // 计算水平夹角
        tarDir.x = a.x;
        tarDir.y = a.z;
        selfDir.x = b.x;
        selfDir.y = b.z;
        
        if (Mathf.Abs(Vector2.SignedAngle(tarDir, selfDir)) < tarAngel)
        {
            return true;
        }

        return false;
    }
}