using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPos : MonoBehaviour
{
    public Tower tower;
    private bool within;

    private void Update()
    {
        if (within && Input.GetKeyDown(KeyCode.T))
        {
            if (tower)
            {
                if (tower.lv == 3)
                {
                    UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("已达<color=red>最大</color>等级");
                    return;
                }

                if (GameManger.Instance.money <= tower.nextLvMoney)
                {
                    UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("金钱<color=red>不足</color>");
                    return;
                }
                // 建造动作
                GameManger.Instance.player.GetComponent<Player>().animator.SetTrigger("Builting");
                Destroy(tower.gameObject);
                tower = Instantiate(Resources.Load<GameObject>("Prefabs/Tower/Tow" + Mathf.Clamp(tower.lv + 1, 1, 3)), transform)
                    .GetComponent<Tower>();
                // 升级扣钱
                GameManger.Instance.AddOrSubMoney(-tower.nextLvMoney);
            }
            else // 没有塔时建塔
            {
                tower = Instantiate(Resources.Load<GameObject>("Prefabs/Tower/Tow1"), transform).GetComponent<Tower>();
                // 够钱才能放塔
                if (GameManger.Instance.money >= tower.nextLvMoney)
                {
                    GameManger.Instance.player.GetComponent<Player>().animator.SetTrigger("Builting");
                    UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("按<color=red>\"T\"</color>升级");
                    // 建造扣钱
                    GameManger.Instance.AddOrSubMoney(-tower.nextLvMoney);
                }
                else
                {
                    // 金钱不足马上删除并置空
                    DestroyImmediate(tower.gameObject);
                    tower = null;
                    UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("金钱<color=red>不足</color>");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 仅玩家生效
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            within = true;
            // 是否已经建过塔
            if (tower)
            {
                if (tower.lv >= 3)
                {
                    UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("已达<color=red>最大</color>等级");
                }
                else
                {
                    UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("按<color=red>\"T\"</color>升级");
                }
            }
            else
            {
                UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("按<color=red>\"T\"</color>建造防御塔");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            within = false;
            UIManager.Instance.GetPanel<GamePanel>().HideGameTips();
        }
    }
}