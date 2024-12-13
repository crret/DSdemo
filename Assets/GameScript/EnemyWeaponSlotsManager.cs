using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{


    public class EnemyWeaponSlotsManager : MonoBehaviour
    {
        public WeaponItem rightHandWeapon;
        
        public WeaponHolderSlot rightHandSlot;
        public WeaponHolderSlot leftHandSlot;
        
        DamageCollider leftHandDamageCollider;
        DamageCollider rightHandDamageCollider;

        private void Awake()
        {
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
            }
        }

        private void Start()
        {
            if (rightHandWeapon != null)
            {   
                LoadWeaponOnSlot(rightHandWeapon,false);
            }
        }

        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
        {
            if (isLeft)
            {
                leftHandSlot.weaponItemOfThisSlotCurrentWeaponModel = weaponItem;
                leftHandSlot.LoadWeaponModel(weaponItem);
                LoadWeaponsDamageCollider(true);
            }
            else
            {
                rightHandSlot.weaponItemOfThisSlotCurrentWeaponModel = weaponItem;
                rightHandSlot.LoadWeaponModel(weaponItem);
                LoadWeaponsDamageCollider(false);
            }
        }

        public void LoadWeaponsDamageCollider(bool isLeft)
        {
            if (isLeft)
            {
                leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            }
            else
            {
                rightHandDamageCollider=rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            }
        }

        public void OpenDamageCollider()
        {
            rightHandDamageCollider.EnableDamageCollider();
        }

        public void CloseDamageCollider()
        {
            rightHandDamageCollider.DisableDamageCollider();
        }
    }
}