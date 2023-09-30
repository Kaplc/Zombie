using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
   private float hp;
   private float maxHp;
   
   private bool within;
   private bool isShow;

   private void Start()
   {
      maxHp = hp = DataManager.Instance.mapInfo.coreHp;
      UpdateHpForPanel();
   }

   private void Update()
   {
      if (within)
      {
         if (Input.GetKeyDown(KeyCode.E))
         {
            UIManager.Instance.GetPanel<GamePanel>().ShowShopPanel();
            isShow = true;
         }

         if (isShow)
         {
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
               UIManager.Instance.GetPanel<GamePanel>().itemBtns[0].Trigger();
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
               UIManager.Instance.GetPanel<GamePanel>().itemBtns[1].Trigger();
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
               UIManager.Instance.GetPanel<GamePanel>().itemBtns[2].Trigger();
            }
         }
      }
   }

   public void UpdateHpForPanel()
   {
      UIManager.Instance.GetPanel<GamePanel>().UpdateCoreHp(hp, maxHp);
   }

   public void Wound(float woundHp)
   {
      hp = hp - woundHp >= 0 ? hp - woundHp : 0;
      if (hp == 0 && GameManger.Instance.isGameOver == false)
      {
         // 结束BGM
         GameManger.Instance.bgm.clip = Resources.Load<AudioClip>("Music/结束");
         GameManger.Instance.bgm.Play();
         GameManger.Instance.bgm.loop = false;
         hp = 0;
         GameManger.Instance.GameOver(false);
      }
      
      UpdateHpForPanel();
   }
   
   private void OnTriggerEnter(Collider other)
   {
      if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
      {
         within = true;
         UIManager.Instance.GetPanel<GamePanel>().ShowGameTips("按<color=red>\"E\"</color>打开补给面板");
      }
   }

   private void OnTriggerExit(Collider other)
   {
      if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
      {
         within = false;
         isShow = false;
         UIManager.Instance.GetPanel<GamePanel>().HideGameTips();
         UIManager.Instance.GetPanel<GamePanel>().HideShopPanel();
      }
   }
}
