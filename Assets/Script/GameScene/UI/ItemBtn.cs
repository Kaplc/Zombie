using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_ItemType
{
    Hp,
    Bullet,
    Atk
}

public class ItemBtn : MonoBehaviour
{
    public int money;
    public E_ItemType type;

    public void Trigger()
    {
        switch (type)
        {
            case E_ItemType.Hp:
                if (GameManger.Instance.money >= money)
                {
                    // 成功加血就扣钱
                    if (GameManger.Instance.player.GetComponent<Player>().RestoreHealth())
                    {
                        GameManger.Instance.AddOrSubMoney(-money);
                    }
                }
                else
                    UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("金钱<color=red>不足</color>"); 
                
                break;
            case E_ItemType.Bullet:
                if (GameManger.Instance.money >= money)
                {
                    if (GameManger.Instance.player.GetComponent<Player>().RestoreBullet())
                    {
                        GameManger.Instance.AddOrSubMoney(-money);
                    }
                    else
                    {
                        UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("后备弹药已满");
                    }
                }
                else
                    UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("金钱<color=red>不足</color>");
                
                break;
            case E_ItemType.Atk:
                if (GameManger.Instance.money >= money)
                {
                    GameManger.Instance.player.GetComponent<Player>().AddAtk();
                    GameManger.Instance.AddOrSubMoney(-money);
                }
                else
                    UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("金钱<color=red>不足</color>");
                
                break;
        }
    }

}
