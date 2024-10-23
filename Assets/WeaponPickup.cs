using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class WeaponPickup : Interactable   //挂在实际物体上的脚本
    {
        public WeaponItem weapon;

        public override void Interact(PlayerManager playerManager)
        {
          base.Interact(playerManager);
          PickUpItem(playerManager);
          //pick up item and add it to the player's inventory
        }

        private void PickUpItem(PlayerManager playerManager)
        {
            PlayerInventory playerInventory;
            ActorController actorController;
            playerInventory = playerManager.GetComponent<PlayerInventory>();
            actorController = playerManager.GetComponentInChildren<ActorController>();
            
            actorController.rigid.velocity = Vector3.zero; //捡东西时速度为0
            actorController.PlayTargetAnim("60070pickupItem" );
            playerInventory.weaponsInventory.Add(weapon);
            playerManager.itemPopUpInteractableUIGameObject.GetComponentInChildren<Text>().text = weapon.itemName;
            playerManager.itemPopUpInteractableUIGameObject.GetComponentInChildren<RawImage>().texture =
                weapon.itemIcon.texture;
            playerManager.itemPopUpInteractableUIGameObject.SetActive(true);
            Destroy(gameObject);
        }
    }
}