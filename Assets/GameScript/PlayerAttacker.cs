using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace SG
{


    public class PlayerAttacker : MonoBehaviour
    {
        public ActorController actorController;
        public Animator animator;
        public PlayerManager playerManager;
        public PlayerInput playerInput;
        public PlayerInventory playerInventory;
        public InteractBoolList interactBoolList;
        
        WeaponSlotManager weaponSlotManager;
        public AnimatorStateInfo stateInfo;

        public bool exitHeavyCharge;
        public bool heavyAttackSp=false;

        
        public bool canDoCombo = false;
        
        
        public bool freeGround;
       
        public bool inRightLightAttack01;
        public bool inRightLightAttack02;
        public bool inRightLightAttack03;
        public bool inRightLightDash;
        public bool inRightLightStep;
        public bool inBothLightAttack01;
        public bool inBothLightAttack02;
        public bool inBothLightAttack03; 
        public bool inBothLightDash;
        public bool inBothLightStep;
        public bool inRightHeavy01SubStart;
        public bool inRightHeavy01Start; 
        public bool inRightHeavy01End;
        public bool inRightHeavy02Start;
        public bool inRightHeavy02End;
        public bool inBothHeavy01SubStart;
        public bool inBothHeavy01Start;
        public bool inBothHeavy01End;
        public bool inBothHeavy02Start;
        public bool inBothHeavy02End;

        public bool inLeftLightAttack01;
        public bool inLeftLightAttack02;
        [SerializeField] private float TransitionMulti=20f;
      
        private void Awake()
        {   
            actorController = GetComponentInChildren<ActorController>();
            playerManager = GetComponent<PlayerManager>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            actorController=GetComponentInChildren<ActorController>();
            playerInventory = GetComponent<PlayerInventory>();
            animator = GetComponentInChildren<Animator>();
            playerInput=GetComponent<PlayerInput>();
            interactBoolList=GetComponent<InteractBoolList>();
        }

        private void Start()
        {
            
        }
        private void Update()
        {   stateInfo = animator.GetCurrentAnimatorStateInfo(0);
         
            if ((stateInfo.IsName("ground") || stateInfo.IsName("Alt") || stateInfo.IsName("LockOn.Move") ||
                 stateInfo.IsName("LockOn.Idle")) && interactBoolList.InAttack.Any(func => func()) == false)
            {
                freeGround = true;
               
            }
            else
            {
                freeGround = false;
            }
            
            if (freeGround)
            {   //Debug.Log("freeGround");
                TransitionMulti = 20f;
                if (actorController.isTwoHand == false)
                {   
                    AttackRightLight1_OnUpdate();
                }
                else
                {
                    AttackBothLight1_OnUpdate();
                }
            
            }
            else if (actorController.rolling)
            {
                AttackRightLight1_OnUpdate();
            }
            else if (actorController.backStep)
            {
                AttackRightLight1_OnUpdate();
            }
            else if (inRightLightAttack01)
            {
                AttackRightLight1_OnUpdate();
            }
            else if (inRightLightAttack02)
            {
                TransitionMulti = 50f;
                AttackRightLight2_OnUpdate();
            }
            else if (inRightLightAttack03)
            {
                AttackRightLight3_OnUpdate();
            }
            else if(inBothLightAttack01)
            {
                AttackBothLight1_OnUpdate();
            }
            else if(inBothLightAttack02)
            {
                AttackBothLight2_OnUpdate();
            }
            else if(inBothLightAttack03)
            {
                AttackBothLight3_OnUpdate();
            }
            else if (inRightHeavy01SubStart)
            {   
                AttackRightHeavy1SubStart_onUpdate();
            }
            else if (inRightHeavy01Start)
            {   //Debug.Log("heavy01");
                
                AttackRightHeavy1Start_onUpdate();
            }
            else if(inRightHeavy01End)
            {
                AttackRightHeavy1End_onUpdate();
            }
            else if (inRightHeavy02Start)
            {   
                AttackRightHeavy2Start_onUpdate();
            }   
            else if(inRightHeavy02End)
            {
                AttackRightHeavy2End_onUpdate();
            }
            else if (inBothHeavy01SubStart)
            {
                AttackBothHeavy1SubStart_onUpdate();
            }
            else if (inBothHeavy01Start)
            {     TransitionMulti = 50f;
                AttackBothHeavy1Start_onUpdate();
            }
            else if (inBothHeavy01End)
            {
                AttackBothHeavy1End_onUpdate();
            }
            else if (inBothHeavy02Start)
            {
                AttackBothHeavy2Start_onUpdate();
            }
            else if (inBothHeavy02End)
            {
                AttackBothHeavy2End_onUpdate();
            }
            else if (inRightLightDash)
            {
                AttackRightLightDash_onUpdate();
            }
            else if (inBothLightDash)
            {
                AttackBothLightDash_onUpdate();
            }
            else if (inLeftLightAttack01)
            {
                AttackLeftLight1_onUpdate();
            }
         
      
        }
        
       
        
        
        

        public void AttackRightLight1_OnUpdate()
        {   
            AttackCommonFunction("W_AttackRightLight2", "W_AttackRightHeavy1SubStart", "W_AttackLeftLight1", "W_AttackLeftHeavy1", "W_AttackBothLight2", "W_AttackBothHeavy1Start");
        }
        
        public void AttackRightLight2_OnUpdate()
        {
            AttackCommonFunction("W_AttackRightLight3", "W_AttackRightHeavy1Start", "W_AttackLeftLight1",
                "W_AttackLeftHeavy1", "W_AttackBothLight3", "W_AttackBothHeavy1Start");
        }
        public void AttackRightLight3_OnUpdate()
        {
            AttackCommonFunction("W_AttackRightLight2", "W_AttackRightHeavy1SubStart", "W_AttackLeftLight1",
                "W_AttackLeftHeavy1", "W_AttackBothLight2", "W_AttackBothHeavy1Start");
        }
        
        public void AttackBothLight1_OnUpdate()
        {   
            AttackCommonFunction("W_AttackRightLight2", "W_AttackRightHeavy1SubStart", "W_AttackLeftLight1", "W_AttackLeftHeavy1", "W_AttackBothLight2", "W_AttackBothHeavy1SubStart");
        }
        
        public void AttackBothLight2_OnUpdate()
        {   
            AttackCommonFunction("W_AttackRightLight2", "W_AttackRightHeavy1SubStart", "W_AttackLeftLight1", "W_AttackLeftHeavy1", "W_AttackBothLight3", "W_AttackBothHeavy1Start");
        }
        
        public void AttackBothLight3_OnUpdate()
        {   
            AttackCommonFunction("W_AttackRightLight2", "W_AttackRightHeavy1SubStart", "W_AttackLeftLight1", "W_AttackLeftHeavy1", "W_AttackBothLight2", "W_AttackBothHeavy1SubStart");
        }

      
         public void AttackRightHeavy1SubStart_onUpdate()
        {

            //actorController.PlayComboAnim("W_AttackRightHeavy1Start", Time.deltaTime, true, true);
            if (heavyAttackSp&&exitHeavyCharge==false)
            {
                if (playerInput.rt_Input == false)
                {  
                    exitHeavyCharge=true;
                    playerInput.InteractingBuffers[InputType.rt] = false; 
                }
                else
                {
                    exitHeavyCharge=false;
                }
            }
        }
        public void AttackRightHeavy1Start_onUpdate()
        {   //Debug.Log("heavyAttac1");
            AttackCommonFunction("W_AttackRightLight1", "W_AttackRightHeavy2Start", "W_AttackLeftLight1",
                "W_AttackLeftHeavy1", "W_AttackBothLight1", "W_AttackBothHeavy2Start");
            
           
            if (heavyAttackSp&&exitHeavyCharge==false)
            {
                if (playerInput.rt_Input == false)
                {  
                    exitHeavyCharge=true;
                    playerInput.InteractingBuffers[InputType.rt] = false; 
                }
                else
                {
                    exitHeavyCharge=false;
                }
            }
            if(heavyAttackSp==false&&exitHeavyCharge)
            {
                actorController.PlayTargetAnimImmediately("W_AttackRightHeavy1End",0.5f);
                playerInput.InteractingBuffers[InputType.rt] = false; 
                exitHeavyCharge=false;
            }
           
        }

        public void AttackRightHeavy1End_onUpdate()
        {   Debug.Log("heavyAttackEnd");
            AttackCommonFunction("W_AttackRightLight1", "W_AttackRightHeavy2Start", "W_AttackLeftLight1",
                "W_AttackLeftHeavy1", "W_AttackBothLight1", "W_AttackBothHeavy2Start");
        }

        public void AttackRightHeavy2Start_onUpdate()
        {  //Debug.Log("heavyAttack2");
            AttackCommonFunction("W_AttackRightLight1", "W_AttackRightHeavy1Start", "W_AttackLeftLight1",
                "W_AttackLeftHeavy1", "W_AttackBothLight1", "W_AttackBothHeavy1Start");
            
            if (heavyAttackSp&&exitHeavyCharge==false)
            {
                if (playerInput.rt_Input == false)
                {  
                    exitHeavyCharge=true;
                    playerInput.InteractingBuffers[InputType.rt] = false; 
                }
                else
                {
                    exitHeavyCharge=false;
                }
            }
            if(heavyAttackSp==false&&exitHeavyCharge)
            {
                actorController.PlayTargetAnimImmediately("W_AttackRightHeavy2End",0.5f);
                playerInput.InteractingBuffers[InputType.rt] = false; 
                exitHeavyCharge=false;
            }
        }

        public void AttackRightHeavy2End_onUpdate()
        {
            AttackCommonFunction("W_AttackRightLight1", "W_AttackRightHeavy1Start", "W_AttackLeftLight1",
                "W_AttackLeftHeavy1", "W_AttackBothLight1", "W_AttackBothHeavy1Start");
        }
        public void AttackBothHeavy1SubStart_onUpdate()
        {

            //actorController.PlayComboAnim("W_AttackRightHeavy1Start", Time.deltaTime, true, true);
            if (heavyAttackSp&&exitHeavyCharge==false)
            {
                if (playerInput.rt_Input == false)
                {  
                    exitHeavyCharge=true;
                    playerInput.InteractingBuffers[InputType.rt] = false; 
                }
                else
                {
                    exitHeavyCharge=false;
                }
            }
        }
        public void AttackBothHeavy1Start_onUpdate()
        {   //Debug.Log("heavyAttac1");
            AttackCommonFunction("W_AttackRightLight1", "W_AttackRightHeavy2Start", "W_AttackLeftLight1",
                "W_AttackLeftHeavy1", "W_AttackBothLight1", "W_AttackBothHeavy2Start");
            
           
            if (heavyAttackSp&&exitHeavyCharge==false)
            {
                if (playerInput.rt_Input == false)
                {  
                    exitHeavyCharge=true;
                    playerInput.InteractingBuffers[InputType.rt] = false; 
                }
                else
                {
                    exitHeavyCharge=false;
                }
            }
            if(heavyAttackSp==false&&exitHeavyCharge)
            {
                actorController.PlayTargetAnimImmediately("W_AttackBothHeavy1End",0.5f);
                playerInput.InteractingBuffers[InputType.rt] = false; 
                exitHeavyCharge=false;
            }
           
        }

        public void AttackBothHeavy1End_onUpdate()
        {   Debug.Log("heavyAttackEnd");
            AttackCommonFunction("W_AttackRightLight1", "W_AttackRightHeavy2Start", "W_AttackLeftLight1",
                "W_AttackLeftHeavy1", "W_AttackBothLight1", "W_AttackBothHeavy2Start");
        }

        public void AttackBothHeavy2Start_onUpdate()
        {  //Debug.Log("heavyAttack2");
            AttackCommonFunction("W_AttackRightLight1", "W_AttackRightHeavy1Start", "W_AttackLeftLight1",
                "W_AttackLeftHeavy1", "W_AttackBothLight1", "W_AttackBothHeavy1Start");
            
            if (heavyAttackSp&&exitHeavyCharge==false)
            {
                if (playerInput.rt_Input == false)
                {  
                    exitHeavyCharge=true;
                    playerInput.InteractingBuffers[InputType.rt] = false; 
                }
                else
                {
                    exitHeavyCharge=false;
                }
            }
            if(heavyAttackSp==false&&exitHeavyCharge)
            {
                actorController.PlayTargetAnimImmediately("W_AttackBothHeavy2End",0.5f);
                playerInput.InteractingBuffers[InputType.rt] = false; 
                exitHeavyCharge=false;
            }
        }

        public void AttackBothHeavy2End_onUpdate()
        {
            AttackCommonFunction("W_AttackRightLight1", "W_AttackRightHeavy1Start", "W_AttackLeftLight1",
                "W_AttackLeftHeavy1", "W_AttackBothLight1", "W_AttackBothHeavy1Start");
        }

        public void Roll_onUpdate()
        {
        
            AttackCommonFunction("W_AttackRightLightStep", "W_AttackRightHeavy1Start", "W_AttackLeftLight1",
                "W_AttackLeftHeavy1", "W_AttackBothLight1", "W_AttackBothHeavy1Start");
        }

        public void BackStep_onUpdate()
        {
            AttackCommonFunction("W_AttackRightLighDash", "W_AttackRightHeavy1Start", "W_AttackLeftLight1",
                "W_AttackLeftHeavy1", "W_AttackBothLight1", "W_AttackBothHeavy1Start");
        }
        public void AttackRightLightStep_onUpdate()
        {
            AttackCommonFunction("W_AttackRightLight1", "W_AttackRightHeavy1Start", "W_AttackLeftLight1",
                "W_AttackLeftHeavy1", "W_AttackBothLight1", "W_AttackBothHeavy1Start");
        }

        public void AttackRightLightDash_onUpdate()
        {
            
            AttackCommonFunction("W_AttackRightLight1", "W_AttackRightHeavy1Start", "W_AttackLeftLight1",
                "W_AttackLeftHeavy1", "W_AttackBothLight1", "W_AttackBothHeavy1Start");
        }
        public void AttackBothLightDash_onUpdate()
        {
            
            AttackCommonFunction("W_AttackRightLight1", "W_AttackRightHeavy1Start", "W_AttackLeftLight1",
                "W_AttackLeftHeavy1", "W_AttackBothLight1", "W_AttackBothHeavy1Start");
        }

        public void AttackLeftLight1_onUpdate()
        {
            AttackCommonFunction("W_AttackRightLight1", "W_AttackRightHeavy1Start", "W_AttackLeftLight2",
                "W_AttackLeftHeavy1", "W_AttackBothLight1", "W_AttackBothHeavy1Start");
        }
        
        public void AttackCommonFunction(string r1, string r2,string l1, string l2,string b1, string b2)
        {   
            ExecAttack(r1, r2, l1, l2, b1, b2);
        }
    
    
        public void ExecAttack(string r1, string r2,string l1, string l2,string b1, string b2)
        {   string request = GetAttackRequest();
            if (freeGround)
            {  
                r1 = "W_AttackRightLight1";
                b1 = "W_AttackBothLight1";
                r2 = "W_AttackRightHeavy1Start";
                b2 = "W_AttackBothHeavy1Start";
                l1 = "W_AttackLeftLight1";
                if (actorController.SprintFlag)
                {
                    r1 = "W_AttackRightLightDash";
                    b1 = "W_AttackBothLightDash";
                }
                canDoCombo = true;
            }

            
            else if (actorController.rolling)
            {    
                r1 = "W_AttackRightLightStep"; 
            }
            else if (actorController.backStep)
            {  
                r1 = "W_AttackRightLightDash";
            }
         
            
            if (request == "ATTACK_REQUEST_RIGHT_LIGHT")
            {  
                if (actorController.PlayComboAnim(r1, 0.2f, true, true))
                {
                    playerInput.InteractingBuffers[InputType.rb] = false;
                }
            }
            else if (request == "ATTACK_REQUEST_BOTH_LIGHT")
            {
                if (actorController.PlayComboAnim(b1, 0.2f, true, true))
                {
                    playerInput.InteractingBuffers[InputType.rb] = false;
                }
            }
            else if (request == "ATTACK_REQUEST_RIGHT_HEAVY")
            {
               
                if (actorController.PlayComboAnim(r2, 0.006f * TransitionMulti, true, true)) 
                {
                        playerInput.InteractingBuffers[InputType.rt] = false;
                        canDoCombo = false;
                }
            }
            else if (request == "ATTACK_REQUEST_BOTH_HEAVY")
            {
                if (actorController.PlayComboAnim(b2, 0.006f * TransitionMulti, true, true)) 
                {
                    playerInput.InteractingBuffers[InputType.rt] = false;
                    canDoCombo = false;
                }
            }
            else if (request == "ATTACK_REQUEST_LEFT_LIGHT")
            {
                if (actorController.PlayComboAnim(l1, 0.2f, true, true))
                {
                    playerInput.InteractingBuffers[InputType.lb] = false;
                }
            }
        }

        public string GetAttackRequest()
        {   
            if (playerInput.InteractingBuffers[InputType.rb])
            {   
                if (actorController.isTwoHand)
                {
                    return "ATTACK_REQUEST_BOTH_LIGHT";
                }
                else 
                {
                    return "ATTACK_REQUEST_RIGHT_LIGHT";
                }
                
            }
            else if(playerInput.InteractingBuffers[InputType.rt])
            {
                if (actorController.isTwoHand)
                {
                    return "ATTACK_REQUEST_BOTH_HEAVY";
                }
                else 
                {
                    return "ATTACK_REQUEST_RIGHT_HEAVY";
                }
            }
            else if (playerInput.InteractingBuffers[InputType.lb])
            {
                if (actorController.isTwoHand==false)
                {
                    return "ATTACK_REQUEST_LEFT_LIGHT";
                }
            }
            return null;
        }

        
       
    }
}