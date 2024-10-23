using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace SG
{
   

    public class PlayerInventory : MonoBehaviour
    {
        private WeaponSlotManager weaponSlotManager; 
        public WeaponItem curRightWeapon;
        public WeaponItem curLeftWeapon;
        public WeaponItem UnarmedWeapon;
        
        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[2];
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[2];

        public int currentRightHandWeaponIndex = 0;
        public int currentLeftHandWeaponIndex = 0;

        public List<WeaponItem> weaponsInventory;
        
        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
           
        }

        private void Start()
        {
            intializeEquipedWeapons();
            curRightWeapon=weaponsInRightHandSlots[0];
            curLeftWeapon=weaponsInLeftHandSlots[0];
            weaponSlotManager.LoadWeaponOnSlot(curRightWeapon,false);
            weaponSlotManager.LoadWeaponOnSlot(curLeftWeapon,true);

        }

        public void ChangeRightWeapon()
        {   weaponSlotManager.UnLoadBackSlotWeapon();
            weaponSlotManager.LoadWeaponOnSlot(curLeftWeapon,true);
            
            int count = weaponsInRightSlotsCount();
         
            if (count >= 2)
            {
                int j = currentRightHandWeaponIndex, k = 0;
                while (k < weaponsInRightHandSlots.Length)
                {
                    if (j == weaponsInRightHandSlots.Length-1)
                    {
                        j = 0;
                    }
                    else
                    {
                        j++;
                    }
                    
                    if (weaponsInRightHandSlots[j] != UnarmedWeapon && j != currentRightHandWeaponIndex)
                    {
                        currentRightHandWeaponIndex = j;
                        curRightWeapon=weaponsInRightHandSlots[j];
                        weaponSlotManager.LoadWeaponOnSlot(curRightWeapon,false);
                        break;
                    }
                    k++;
                }
            }
            else
            {
                if (weaponsInRightHandSlots[currentRightHandWeaponIndex] == UnarmedWeapon)
                {
                    int j = currentRightHandWeaponIndex, k = 0;
                    bool FindWeapon = false;
                    while (k < weaponsInRightHandSlots.Length)
                    {
                        if (j == weaponsInRightHandSlots.Length-1)
                        {
                            j = 0;
                        }
                        else
                        {
                            j++;
                        }
                    
                        if (weaponsInRightHandSlots[j] != UnarmedWeapon && j != currentRightHandWeaponIndex)
                        {   FindWeapon=true;
                            currentRightHandWeaponIndex = j;
                            curRightWeapon=weaponsInRightHandSlots[j];
                            weaponSlotManager.LoadWeaponOnSlot(curRightWeapon,false);
                            break;
                        }
                        k++;
                    }

                    if (FindWeapon == false)
                    {
                        curRightWeapon = UnarmedWeapon;
                        weaponSlotManager.LoadWeaponOnSlot(curRightWeapon,false);
                    }
                }
                else
                {
                    if (currentRightHandWeaponIndex == weaponsInRightHandSlots.Length-1)
                    {
                        currentRightHandWeaponIndex = 0;
                    }
                    else
                    {
                        currentRightHandWeaponIndex++;
                    }

                    curRightWeapon = weaponsInRightHandSlots[currentRightHandWeaponIndex];
                    weaponSlotManager.LoadWeaponOnSlot(curRightWeapon,false);

                }
               
            }
        }
        public void ChangeLeftWeapon()
        {
            int count = weaponsInLeftSlotsCount();
         
            if (count >= 2)
            {
                int j = currentLeftHandWeaponIndex, k = 0;
                while (k < weaponsInLeftHandSlots.Length)
                {
                    if (j == weaponsInLeftHandSlots.Length-1)
                    {
                        j = 0;
                    }
                    else
                    {
                        j++;
                    }
                    
                    if (weaponsInLeftHandSlots[j] != UnarmedWeapon && j != currentLeftHandWeaponIndex)
                    {
                        currentLeftHandWeaponIndex = j;
                        curLeftWeapon=weaponsInLeftHandSlots[j];
                        weaponSlotManager.LoadWeaponOnSlot(curLeftWeapon,false);
                        break;
                    }
                    k++;
                }
            }
            else
            {
                if (weaponsInLeftHandSlots[currentLeftHandWeaponIndex] == UnarmedWeapon)
                {
                    int j = currentLeftHandWeaponIndex, k = 0;
                    bool FindWeapon = false;
                    while (k < weaponsInLeftHandSlots.Length)
                    {
                        if (j == weaponsInLeftHandSlots.Length-1)
                        {
                            j = 0;
                        }
                        else
                        {
                            j++;
                        }
                    
                        if (weaponsInLeftHandSlots[j] != UnarmedWeapon && j != currentLeftHandWeaponIndex)
                        {   FindWeapon=true;
                            currentLeftHandWeaponIndex = j;
                            curLeftWeapon=weaponsInLeftHandSlots[j];
                            weaponSlotManager.LoadWeaponOnSlot(curLeftWeapon,false);
                            break;
                        }
                        k++;
                    }

                    if (FindWeapon == false)
                    {
                        curLeftWeapon = UnarmedWeapon;
                        weaponSlotManager.LoadWeaponOnSlot(curLeftWeapon,false);
                    }
                }
                else
                {
                    if (currentLeftHandWeaponIndex == weaponsInLeftHandSlots.Length-1)
                    {
                        currentLeftHandWeaponIndex = 0;
                    }
                    else
                    {
                        currentLeftHandWeaponIndex++;
                    }

                    curLeftWeapon = weaponsInLeftHandSlots[currentLeftHandWeaponIndex];
                    weaponSlotManager.LoadWeaponOnSlot(curLeftWeapon,false);

                }
               
            }
        }
       

    

        private void intializeEquipedWeapons()
        {
            for (int i = 0; i < weaponsInRightHandSlots.Length; i++)
            {
                if (weaponsInRightHandSlots[i] == null)
                {
                    weaponsInRightHandSlots[i] = UnarmedWeapon;
                }

            }
            for (int i = 0; i < weaponsInLeftHandSlots.Length; i++)
            {
                if (weaponsInLeftHandSlots[i] == null)
                {
                    weaponsInLeftHandSlots[i] = UnarmedWeapon;
                }
            }
        }
        public void AddItemToInventory(WeaponItem weaponItem)
        {
            if (weaponItem != UnarmedWeapon)
            {
                weaponsInventory.Add(weaponItem);
            }
        }

        public int weaponsInRightSlotsCount()
        {   int count=0;
            for (int i = 0; i < weaponsInRightHandSlots.Length; i++)
            {
                if (weaponsInRightHandSlots[i] != null&&weaponsInRightHandSlots[i]!=UnarmedWeapon)
                {
                    count++;
                }
            }
            return count;
        }
        public int weaponsInLeftSlotsCount()
        {   int count=0;
            for (int i = 0; i < weaponsInLeftHandSlots.Length; i++)
            {
                if (weaponsInLeftHandSlots[i] != null&&weaponsInLeftHandSlots[i]!=UnarmedWeapon)
                {
                    count++;
                }
            }
            return count;
        }
    }
}