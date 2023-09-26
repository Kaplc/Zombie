﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Zombie : MonoBehaviour
{
    private Transform target;
    private Animator animator;
    private NavMeshAgent agent;
    private CapsuleCollider capsuleCollider;

    private float hp;
    private float atkCd;
    private float atk;
    private float speed;

    private float lastAtkTime;
    private float lastLockTarTime; // 锁敌时间

    private Coroutine stopMoveCoroutine;

    private void Awake()
    {
        // 状态机脚本
        animator = gameObject.GetComponent<Animator>();
        // 寻路导航
        agent = gameObject.GetComponent<NavMeshAgent>();
        // 碰撞器
        capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        // 随机目标
        if (Random.Range(1, 101) > 50)
        {
            target = GameManger.Instance.player;
        }
        else
        {
            target = GameManger.Instance.core.transform;
        }
    }

    private void Update()
    {
        // 游戏结束僵尸行为停止
        if (GameManger.Instance.isGameOver)
        {
            animator.SetBool("Walk", false);
            agent.isStopped = true;
            return;
        }
        
        // 距离更近进行攻击
        if (Vector3.Distance(transform.position, target.position) <= 4f)
        {
            // 到攻击距离停止移动动画
            animator.SetBool("Walk", false);

            // 满足攻击cd才攻击
            if (Time.time - lastAtkTime >= atkCd)
            {
                lastAtkTime = Time.time;
                Attack();
            }
        }

        // 已经可以移动时, 3秒重新索敌
        if (Vector3.Distance(transform.position, target.position) > 3 && Time.time - lastLockTarTime >= 2 && animator.GetBool("Walk"))
        {
            lastLockTarTime = Time.time;
            // 重新启动移动动画
            animator.SetBool("Walk", true);
            agent.SetDestination(target.position);
        }

        if (hp > 0)
        {
            transform.LookAt(target);
        }
    }

    public void Init(ZombieInfo info)
    {
        // 添加状态机
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(info.animatorPath);

        hp = info.hp;
        atk = info.atk;
        atkCd = info.atkCd;

        // 设置停止导航距离
        agent.stoppingDistance = 3;
        // 设置速度
        agent.speed = info.speed;
        speed = info.speed;
        agent.angularSpeed = info.rotaSpeed;
    }

    private void Move()
    {
        // 移动动画
        animator.SetBool("Walk", true);

        // 设置目标
        agent.SetDestination(target.position);
    }

    public void Wound(float atk)
    {
        hp -= atk;
        // 受伤动画
        animator.SetTrigger("Damage");

        if (hp <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        // 移除碰撞器
        Destroy(capsuleCollider);
        capsuleCollider = null;
        // 停止寻路
        agent.isStopped = true;
        // 死亡动画
        animator.SetBool("Dead", true);
        // 减少数量
        GameManger.Instance.SubCount();
        // 按照攻击能力加钱
        GameManger.Instance.AddMoney((int)atkCd);
        Destroy(gameObject, 5);
    }

    private void Attack()
    {
        // 攻击动画
        animator.SetTrigger("Attack");
    }

    #region 动作事件函数

    public void AtkEvent()
    {
        // 前面+向上一个单位进行范围检测, 根据目标判断层级

        // 根据
        if (target == GameManger.Instance.player)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward + transform.up, 2, 1 << LayerMask.NameToLayer("Player"));
            // 不为0必定攻击到目标
            if (colliders.Length != 0)
            {
                target.GetComponent<Player>().Wound(atk);
            }
        }
        else
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward + transform.up, 2, 1 << LayerMask.NameToLayer("Core"));
            if (colliders.Length != 0)
            {
                target.GetComponent<Core>().Wound(atk);
            }
        }
            
    }

    public void DeadEvent()
    {
    }

    public void StartDamageEvent()
    {
        // 受伤停止移动
        agent.isStopped = true;
    }

    public void EndDamageEvent()
    {
        // 结束受伤继续
        agent.isStopped = false;
    }

    public void EndIdleEvent()
    {
        Move();
    }

    #endregion
}