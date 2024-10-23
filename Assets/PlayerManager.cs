using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SG;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SG
{
    

    public class PlayerManager : CharacterManager
    {
        public PlayerInput playerInput;
        public CamerHandler cameraHandler;
        public ActorController actorController;
        public Animator anim;
        public PlayerAttacker playerAttacker;
        public PlayerInventory playerInventory;
        public InteractableUI interactableUI;
        public OnGroundSensor groundSensor;
        public InteractBoolList interactBoolList;
        public WeaponSlotManager weaponSlotManager;
        

        public GameObject interactableUIGameObject;
        public GameObject itemPopUpInteractableUIGameObject;
        
        public bool isInteracting=false;
        public bool isChangeWeapon=false;
        public bool isInvulerable=true;
        public bool isInAir=false;

        public bool isGrounded;
        
        // Start is called before the first frame update
        private void Awake()
        {
            cameraHandler = CamerHandler.Single;
            
        }


        void Start()
        {   Cursor.visible = false;  
            playerInput = GetComponent<PlayerInput>();
            anim = GetComponentInChildren<Animator>();
            actorController = GetComponentInChildren<ActorController>();
            playerAttacker = GetComponentInChildren<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            interactableUI = FindObjectOfType<InteractableUI>();
            groundSensor = GetComponentInChildren<OnGroundSensor>();
            interactBoolList=GetComponent<InteractBoolList>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            isInvulerable=true;
        }

        // Update is called once per frame
        void Update()
        {

            if (playerInput.InventoryFlag == false)
            {
                Cursor.visible = false;
            }
            else
            {
                Cursor.visible = true;
            }
                
            float delta = Time.deltaTime;
           
            CheckForInteractableObject();
            if (isInAir)
            {
                actorController.inAirTimer += Time.deltaTime;
            }

            /*if (pi.rb_Input)
            {   
                playerAttacker.HandleLightAttack(playerInventory.curRightWeapon);
            }*/


            if (playerInput.InteractingBuffers[InputType.scrollUp]&&interactBoolList.CanAnimPlay())
            {   
                playerInventory.ChangeRightWeapon(); 
                StartCoroutine(actorController.OverrideWeaponAnim(playerInventory.curRightWeapon,false,false));
                actorController.PlayTargetAnimFromOther("ChangeWeaponAnim");
                
                playerInput.scrollUp_Input = false;
                actorController.isTwoHand = false;
            }
            

            if (playerInput.InteractingBuffers[InputType.scrollDown]&&interactBoolList.CanAnimPlay())
            {   Debug.Log("donw");
                
                if (actorController.isTwoHand)
                {
                    actorController.isTwoHand = false;
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.curRightWeapon,false);
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.curLeftWeapon,true);
                    weaponSlotManager.UnLoadBackSlotWeapon();
                    StartCoroutine(actorController.OverrideWeaponAnim(playerInventory.curRightWeapon,false,false));
                }
                else
                {   
                    weaponSlotManager.LoadWeaponOnBackSlot(weaponSlotManager.leftHandSlot.weaponItemOfThisSlotCurrentWeaponModel,true);
                    actorController.isTwoHand = true;
                    StartCoroutine(actorController.OverrideWeaponAnim(playerInventory.curRightWeapon,false,true));
                }
                
                playerInput.InteractingBuffers[InputType.scrollDown]=false;
            }
            
            actorController.HandleRollingAndSprinting(delta);
            actorController.HandleRotation(delta);
            actorController.HandleAltMove();
        }

        private void LateUpdate()
        {
            float delta = Time.deltaTime;
            if (cameraHandler != null)
            {   
                cameraHandler.HandleCameraRotation(delta, playerInput.mouseX, playerInput.mouseY);
                cameraHandler.FollowTarget(delta);
            }
        
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
        
          
        
            actorController.HandleMovement(delta);
            actorController.HandleFalling(delta);
          
        }

        public void CheckForInteractableObject()
        {
            
            var interactableColliders = groundSensor.outputCols.Where((s) => s!=null&&s.CompareTag("Interactable")).ToArray(); ;
                  
            if ( interactableColliders.Length>0)
            {
                Interactable interactableObject = interactableColliders[0].GetComponent<Interactable>();
                if (interactableObject != null)
                {
                    string interactableText = interactableObject.interactableText; //获取与物体互动显示的text
                    interactableUI.interactableText.text = interactableText; //赋值给ui脚本的text
                    interactableUIGameObject.SetActive(true); //显示ui（Interactionpopup）
                    if (playerInput.a_Input)
                    {   
                        interactableColliders[0].GetComponent<Interactable>().Interact(this); //调用方法获取物体到inventory
                    }
                }
            }
            else
            {
                if (interactableUIGameObject != null)
                {
                    interactableUIGameObject.SetActive(false);
                }

                if (itemPopUpInteractableUIGameObject != null&&playerInput.a_Input)
                {
                    itemPopUpInteractableUIGameObject.SetActive(false);
                }
            }
            /*for (int i = 0; i < groundSensor.outputCols.Length; i++)
            {
                if (groundSensor.outputCols[i] != null)
                {
                    if (groundSensor.outputCols[i].CompareTag("Interactable"))
                    {   CanInteract = true;
                        Interactable interactableObject = groundSensor.outputCols[i].GetComponent<Interactable>();//物体(上的Interactable的子类)
                        if (interactableObject != null)
                        {
                            string interactableText = interactableObject.interactableText; //获取与物体互动显示的text
                            interactableUI.interactableText.text = interactableText; //赋值给ui脚本的text
                            interactableUIGameObject.SetActive(true); //显示ui（Interactionpopup）
                            if (pi.a_Input)
                            {   print("dsad");
                                groundSensor.outputCols[i].GetComponent<Interactable>().Interact(this); //获取物体
                            }
                        }
                        break;
                    }
                  
                }

                if (i == groundSensor.outputCols.Length - 1)
                {   
                    CanInteract = false;
                }
            }
            if(CanInteract==false)
            {
                if (interactableUIGameObject != null)
                {
                    interactableUIGameObject.SetActive(false);
                }
            }*/
            /*RaycastHit hit;
            if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f,
                    cameraHandler.ignoreLayers))
            {
                if (hit.collider.tag == "Interactable")
                {
                    Interactable interactableObject = hit.collider.GetComponent<Interactable>();
                    if (interactableObject != null)
                    {
                        string interactableText=interactableObject.interactableText;
                        interactableUI.interactableText.text = interactableText;
                        interactableUIGameObject.SetActive(true);
                        if (pi.a_Input)
                        {
                            hit.collider.GetComponent<Interactable>().Interact(this);
                        }
                    }
                }
            }*/
           
        }

      

    }
}