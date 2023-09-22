using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
   public float hp;
   public float maxHp;
   
   public void UpdateHpForPanel()
   {
      UIManager.Instance.GetPanel<GamePanel>().UpdateCoreHp(hp , maxHp);
   }

   public void Wound(float woundHp)
   {
      hp -= woundHp;
      
      if (hp < 0)
      {
         hp = 0;
         GameOver();
      }
      UpdateHpForPanel();
   }

   public void GameOver()
   {
      
   }
}
