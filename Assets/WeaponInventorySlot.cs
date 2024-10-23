using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SG
{


    public class WeaponInventorySlot : MonoBehaviour //武器库存里的一个slot
    {    
         public PlayerInventory playerInventory;
         public UIManager uIManager;
         public WeaponSlotManager weaponSlotManager;
         public Image icon;
         private WeaponItem item;
         public ActorController actorController;
        
         private void Awake()
         {
             playerInventory = FindObjectOfType<PlayerInventory>();
             uIManager = FindObjectOfType<UIManager>();
             weaponSlotManager = FindObjectOfType<WeaponSlotManager>();
             actorController=FindObjectOfType<ActorController>();
         }

         private void Update()
         {
           
             
         }

         public void AddItem(WeaponItem newItem)
         {
             item = newItem;
             icon.sprite = item.itemIcon;
             icon.enabled = true;
             gameObject.SetActive(true);
         }

         public void ClearInventorySlot()
         {   
             item = null;
             icon.sprite = null;
             icon.enabled = false;
             gameObject.SetActive(false);
         }

         public void EquipThisItem()
         {
             int lastCount = playerInventory.weaponsInRightSlotsCount();
             //Remove current item
             if (uIManager.rightHandSlot01Selected)
             {      
                 playerInventory.AddItemToInventory(playerInventory.weaponsInRightHandSlots[0]);
                 playerInventory.weaponsInRightHandSlots[0] = item;
                 playerInventory.weaponsInventory.Remove(item);
                 
             }
             else if (uIManager.rightHandSlot02Selected)
             {   
                 playerInventory.AddItemToInventory(playerInventory.weaponsInRightHandSlots[1]);
                 playerInventory.weaponsInRightHandSlots[1] = item;
                 playerInventory.weaponsInventory.Remove(item);
             }
             else if (uIManager.leftHandSlot01Selected)
             {
                 playerInventory.AddItemToInventory(playerInventory.weaponsInLeftHandSlots[0]);
                 playerInventory.weaponsInLeftHandSlots[0] = item;
                 playerInventory.weaponsInventory.Remove(item);
             }
             else if(uIManager.leftHandSlot02Selected)
             {
                 playerInventory.AddItemToInventory(playerInventory.weaponsInLeftHandSlots[1]);
                 playerInventory.weaponsInLeftHandSlots[1] = item;
                 playerInventory.weaponsInventory.Remove(item);
                 
             }
             else
             {
                 return;
             }

             if (playerInventory.weaponsInRightHandSlots[playerInventory.currentRightHandWeaponIndex] == playerInventory.UnarmedWeapon)
             {
                 
                 if (uIManager.rightHandSlot01Selected)
                 {      playerInventory.curRightWeapon = item;
                     playerInventory.currentRightHandWeaponIndex = 0;
                 }
                 else if (uIManager.rightHandSlot02Selected)
                 {      playerInventory.curRightWeapon = item;
                     playerInventory.currentRightHandWeaponIndex = 1;
                 }
                 
                 weaponSlotManager.LoadWeaponOnSlot(playerInventory.curRightWeapon,false);
             }
             else
             {   
                 playerInventory.curRightWeapon=playerInventory.weaponsInRightHandSlots[playerInventory.currentRightHandWeaponIndex];
                 weaponSlotManager.LoadWeaponOnSlot(playerInventory.curRightWeapon,false);
             }

            
           //重新加载目前装备的武器 以防如果是换的目前装备的武器
             if (playerInventory.weaponsInLeftHandSlots[playerInventory.currentLeftHandWeaponIndex] == playerInventory.UnarmedWeapon)
             {
                
                 if (uIManager.leftHandSlot01Selected)
                 {   playerInventory.curLeftWeapon = item;
                     playerInventory.currentLeftHandWeaponIndex = 0;
                 }
                 else if (uIManager.leftHandSlot02Selected)
                 {    playerInventory.curLeftWeapon = item;
                     playerInventory.currentLeftHandWeaponIndex = 1;
                 }
                 weaponSlotManager.LoadWeaponOnSlot(playerInventory.curLeftWeapon,true);
             }
             else
             {
                 playerInventory.curLeftWeapon=playerInventory.weaponsInLeftHandSlots[playerInventory.currentLeftHandWeaponIndex];
                 weaponSlotManager.LoadWeaponOnSlot(playerInventory.curLeftWeapon,true);
             }
            
             uIManager.equipmentWindowUI.LoadWeaponOnEquipmentScreen(playerInventory);
             uIManager.ResetAllSelectedSlots();
             //Add current item to inventory
             //Remove this item from inventory
         }
         
      
    }

}