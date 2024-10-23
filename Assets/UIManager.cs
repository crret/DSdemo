using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SG
{
    public class UIManager : MonoBehaviour
    {   public PlayerInventory playerInventory;
        public EquipmentWindowUI equipmentWindowUI;
        public PlayerInput playerInput;
        
        [Header("UI Windows")]
        public GameObject HUDWindow;
        public GameObject SelectWindow;
        public GameObject weaponInventoryWindow;
        public GameObject equipmentScreenWindow;
        [Header("Weapon Inventory")]
        public GameObject weaponInventorySlotPrefab;
        public Transform weaponInventorySlotsParent;
        private WeaponInventorySlot[] weaponInventorySlots;//包含一个button和icon
       
        [Header("Equipment Window Slots")]
        public bool rightHandSlot01Selected;
        public bool rightHandSlot02Selected;
        public bool leftHandSlot01Selected;
        public bool leftHandSlot02Selected;
        
        private void Awake()
        {
           
        }

        private void Start()
        {
            weaponInventorySlots =
                weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();//初始化 最开始只有一个slot
            equipmentWindowUI.LoadWeaponOnEquipmentScreen(playerInventory);
        }
        public void UpdateUI()
        {
            #region Weapon Inventory Slots 
            //注意是更新ui 如何更新的 比实际库存小时，实例化slots并加载icon 否则不显示
            if (weaponInventorySlots.Length < playerInventory.weaponsInventory.Count)
            {
                // 计算需要增加的slots数量
                int additionalSlots = playerInventory.weaponsInventory.Count - weaponInventorySlots.Length;

                // 实例化需要的slots
                for (int j = 0; j < additionalSlots; j++)
                {
                    Instantiate(weaponInventorySlotPrefab, weaponInventorySlotsParent); //实例化Prefab位于weaponInventorySlotsParent
                }

                // 实例化完成后，更新slots的引用
                weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
            }

            // 开始填充slots或清空
            for (int i = 0; i < weaponInventorySlots.Length; i++)
            {
                if (i < playerInventory.weaponsInventory.Count) // 小于库存容量时，说明有东西，添加slot
                {
                    weaponInventorySlots[i].AddItem(playerInventory.weaponsInventory[i]); // 添加物品到slot
                }
                else
                {
                    weaponInventorySlots[i].ClearInventorySlot(); // 清空多余的slots
                }
            }

            #endregion
        }
        public void OpenSelectWindow()
        {
            SelectWindow.SetActive(true);
        }

        public void CloseSelectWindow()
        {
            SelectWindow.SetActive(false);
        }

        public void CloseAllInventoryWindows()
        {   ResetAllSelectedSlots();
            weaponInventoryWindow.SetActive(false);
            equipmentScreenWindow.SetActive(false);
            playerInput.InventoryFlag = false;
        }

        public void ResetAllSelectedSlots()
        {
            rightHandSlot01Selected = false;
            rightHandSlot02Selected = false;
            leftHandSlot01Selected = false;
            leftHandSlot02Selected = false;
        }
    }
}