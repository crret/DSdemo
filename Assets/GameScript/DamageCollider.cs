using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
   
   public class DamageCollider : MonoBehaviour //武器上的碰撞体 
   {
      Collider damageCollider;
      private bool hasTriggered = false;
      public int currentWeaponDamage = 25;

      private void Awake()
      {
         damageCollider = GetComponent<Collider>();
         damageCollider.gameObject.SetActive(true); //显示
         damageCollider.isTrigger = true; //开启trigger 触发一次
         damageCollider.enabled = false; //碰撞关闭
      }

      public void EnableDamageCollider()
      {
         damageCollider.enabled = true;
      }

      public void DisableDamageCollider()
      {
         damageCollider.enabled = false;
      }

      private void OnTriggerEnter(Collider collider)
      {  
         if (collider.tag == "Player")
         {
            PlayerStats playerStats = collider.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
               playerStats.TakeDamage(currentWeaponDamage);
            }
         }

         if (collider.tag == "Enemy")
         {
            EnemyStats enemyStats = collider.GetComponent<EnemyStats>();

            if (enemyStats != null)
            {
               enemyStats.TakeDamage(currentWeaponDamage);
            }
         }
      }
   }
}