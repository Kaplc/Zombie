using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
   private float hp;
   private float maxHp;

   private void Start()
   {
      maxHp = hp = DataManager.Instance.mapInfo.coreHp;
      UpdateHpForPanel();
   }

   public void UpdateHpForPanel()
   {
      UIManager.Instance.GetPanel<GamePanel>().UpdateCoreHp(hp, maxHp);
   }

   public void Wound(float woundHp)
   {
      
      hp -= woundHp;
      if (hp < 0 && GameManger.Instance.isGameOver == false)
      {
         // 结束BGM
         GameManger.Instance.bgm.clip = Resources.Load<AudioClip>("Music/结束");
         hp = 0;
         GameManger.Instance.GameOver(false);
      }
      
      UpdateHpForPanel();
   }
}
