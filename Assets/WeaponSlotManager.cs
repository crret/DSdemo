using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{


    public class WeaponSlotManager : MonoBehaviour
    {   PlayerStats playerStats;
        public WeaponHolderSlot leftHandSlot;
        public WeaponHolderSlot rightHandSlot;
        public WeaponHolderSlot backSlot;
        
        DamageCollider leftHandDamageCollider;
        DamageCollider rightHandDamageCollider;
        QuickSlotsUI quickSlotsUI;
        public PlayerInput playerInput;
        public PlayerInventory playerInventory;
        public WeaponItem attackingWeapon;
        private void Awake()
        {   playerStats = GetComponentInParent<PlayerStats>();
            playerInventory = GetComponentInParent<PlayerInventory>();
            playerInput= GetComponentInParent<PlayerInput>();
            quickSlotsUI = FindObjectOfType<QuickSlotsUI>();
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots)
            {   
                if (weaponSlot.isLeftHandSlot)
                {
                    leftHandSlot = weaponSlot;
                }
                else if (weaponSlot.isRightHandSlot)
                {
                    rightHandSlot = weaponSlot;
                }
                else if (weaponSlot.isBackSlot)
                {
                    backSlot = weaponSlot;
                }
            } //将手上的weaponslot获取进来
        }
        

        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft) //装武器
        {
            if (isLeft)
            {
                if (leftHandSlot.weaponItemOfThisSlotCurrentWeaponModel != weaponItem)
                {
                    leftHandSlot.weaponItemOfThisSlotCurrentWeaponModel = weaponItem;
                    leftHandSlot.LoadWeaponModel(weaponItem); //装武器模型
                    LoadLeftWeaponDamageCollider(); //装碰撞体 只是获取component到变量中
                    quickSlotsUI.UpdateWeaponQuickSlotsUI(true, weaponItem); //更新ui
                }
            }
            else
            {
                if (rightHandSlot.weaponItemOfThisSlotCurrentWeaponModel != weaponItem)
                {
                    rightHandSlot.weaponItemOfThisSlotCurrentWeaponModel = weaponItem;
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightHandDamageCollider();
                    quickSlotsUI.UpdateWeaponQuickSlotsUI(false,weaponItem);
                }
            }
        }

        public void LoadWeaponOnBackSlot(WeaponItem weaponItem, bool loadLeftWeapon)
        {
            if (weaponItem.isUnarmed == false)
            {
                if (loadLeftWeapon)
                {
                    backSlot.LoadWeaponModel(weaponItem);
                    Transform weaponPivot = backSlot.currentWeaponModel.transform.Find("WeaponPivot");
                    Transform backPivot=backSlot.currentWeaponModel.transform.Find("BackPivot");
                    weaponPivot.position = backPivot.position;
                    weaponPivot.rotation = backPivot.rotation;
                    leftHandSlot.weaponItemOfThisSlotCurrentWeaponModel = playerInventory.UnarmedWeapon;
                    leftHandSlot.LoadWeaponModel(playerInventory.UnarmedWeapon);
                }
                else
                {
                    backSlot.LoadWeaponModel(weaponItem);
                    Transform weaponPivot = backSlot.currentWeaponModel.transform.Find("WeaponPivot");
                    Transform backPivot=backSlot.currentWeaponModel.transform.Find("BackPivot");
                    weaponPivot.position = backPivot.position;
                    weaponPivot.rotation = backPivot.rotation;
                    rightHandSlot.weaponItemOfThisSlotCurrentWeaponModel = playerInventory.UnarmedWeapon;
                    rightHandSlot.LoadWeaponModel(playerInventory.UnarmedWeapon); //只更改了模型 slot里还在
                }
            }
         
        }

        public void UnLoadBackSlotWeapon()
        {
            backSlot.UnloadWeaponAndDestroy();
        }
        #region Handle Weapon's Damage Collider

        private void LoadLeftWeaponDamageCollider()
        {
            leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        }

        private void LoadRightHandDamageCollider()
        {
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        }

        public void OpenRightHandDamageCollider()
        {
            rightHandDamageCollider.EnableDamageCollider();
        }

        public void OpenLeftHandDamageCollider()
        {
            leftHandDamageCollider.EnableDamageCollider();
        }

        public void CloseRightHandDamageCollider()
        {
            rightHandDamageCollider.DisableDamageCollider();
        }

        public void CloseLeftHandDamageCollider()
        {
            leftHandDamageCollider.DisableDamageCollider();
        }

        public void OpenBothHandDamageCollider()
        {
            if (leftHandSlot.weaponItemOfThisSlotCurrentWeaponModel != playerInventory.UnarmedWeapon)
            {
                leftHandDamageCollider.EnableDamageCollider();
            }
            else if (rightHandSlot.weaponItemOfThisSlotCurrentWeaponModel != playerInventory.UnarmedWeapon)
            {
                rightHandDamageCollider.EnableDamageCollider();
            }
            else
            {
                Debug.Log("openDmamageColliderFail");
            }
        }

        public void CloseBothHandDamageCollider()
        {
            if (leftHandSlot.weaponItemOfThisSlotCurrentWeaponModel != playerInventory.UnarmedWeapon)
            {
                leftHandDamageCollider.DisableDamageCollider();
            }
            else if (rightHandSlot.weaponItemOfThisSlotCurrentWeaponModel != playerInventory.UnarmedWeapon)
            {
                rightHandDamageCollider.DisableDamageCollider();
            }
            else
            {
                Debug.Log("closeDmamageColliderFail");
            }
        }
        #endregion
        
        #region Handle Weapon's Stamina cost
        public void DrainStaminaLightAttack()
        {
            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina*attackingWeapon.lightAttackMultiplier));
        }
        
        public void DrainStaminaHeavyAttack()
        {
            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina*attackingWeapon.heavyAttackMultiplier));
        }
        #endregion
    }
}