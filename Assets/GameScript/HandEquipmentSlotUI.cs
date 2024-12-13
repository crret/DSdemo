using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    
    public class HandEquipmentSlotUI : MonoBehaviour
    {   UIManager uIManager;
        public PlayerInventory playerInventory;
        
        public Image icon;
        private WeaponItem weapon;
        
        public bool rightHandSlot01;
        public bool rightHandSlot02;
        public bool leftHandSlot01;
        public bool leftHandSlot02;
        //这些在监视窗口被主动勾选
        
        private void Awake()
        {
            uIManager = FindObjectOfType<UIManager>();
            
        }

        
        public void AddItem(WeaponItem newWeapon)
        {   
            if (newWeapon.isUnarmed==false)
            {
                weapon=newWeapon;
                if (weapon.itemIcon != null)
                {
                    icon.sprite = weapon.itemIcon;
                }
                icon.enabled = true;
                gameObject.SetActive(true);
            }
           
        }

        public void ClearItem()
        {
            weapon = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }

        public void SelectThisSlot()
        {
            if (rightHandSlot01)
            {
                uIManager.rightHandSlot01Selected = true;
            }
            else if (rightHandSlot02)
            {
                uIManager.rightHandSlot02Selected = true;
            }
            else if(leftHandSlot01)
            {
                uIManager.leftHandSlot01Selected = true;
            }
            else
            {
                uIManager.leftHandSlot02Selected = true;
            }
        }
    }
}