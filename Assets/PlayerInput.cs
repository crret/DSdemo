using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


namespace SG
{
    public enum InputType
    {
        scrollDown,
        scrollUp,
        rb,
        rt,
        lb
    }
    public class PlayerInput : MonoBehaviour
    {
        [Header("key settings")] public string KeyUp = "w";
        public string KeyDown = "s";
        public string KeyLeft = "a";
        public string KeyRight = "d";


        [Header("output signals")] 
        public float Dup = 0;
        public float Dright = 0;
        public float Dup2 = 0;
        public float Dright2 = 0;
        public float Dmag = 0; // 欧氏距离
        public Vector3 Dvec = Vector3.zero; // 方向向量

       
        [Header("others")] private float targetDup;
        private float targetDright;
        private float velocityDup;
        private float velocityDright;

        // public bool inputEnabled=true;

        private Mouse mouse = Mouse.current;
        public float mouseX;
        public float mouseY;
     
        public bool a_Input;
        public bool b_Input;
        public bool rb_Input=false;
        public bool lb_Input = false;
        public bool rt_Input;
        public bool scrollUp_Input;
        public bool scrollDown_Input;
        public bool jump_Input;
        public bool inventory_Input;
        public bool lockOn_Input;
        public bool lockOnLeft_Input;
        public bool lockOnRight_Input;
        public bool Alt_input;
        public float rollInputTimer = 0;
        public float mouseLockCDTimer = 0;
        private PlayerControls inputActions;
        private ActorController actorController;
        public PlayerManager playerManager;
        public UIManager uiManager;
        public CamerHandler cameraHandler;
        public InteractBoolList interactBoolList;
        private Vector2 cameraInput;

        public bool InventoryFlag = false;
        public bool LockOnFlag=false;
        public bool TwoHandFlag = false;
        
        private float lookAngleDetectTimer = 0f;
        private bool isDvecLock=false;
        private float lockDevcTimer = 0f;
        private float lookAngleDetectTimeWindow = 0.02f;
        private float lastLookAngle;
        private Vector3 lastDvec;
        public bool isLookAngleTooBig = false;
        private bool lastIsisLookAngleTooBig;
        
        public Dictionary<InputType, bool> InteractingBuffers = new Dictionary<InputType, bool>();
        public Dictionary<InputType, float> InteractingBuffersTime = new Dictionary<InputType, float>();
        public float BufferTimer;
        public void OnEnable()
        {   
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            }

            inputActions.Enable();
            inputActions.PlayerActions.Inventory.performed += i => inventory_Input = true;
            inputActions.PlayerActions.Jump.performed += i => jump_Input = true;
     
            inputActions.PlayerActions.LockOn.performed += i =>
            {
                if (InventoryFlag == false)
                {
                    lockOn_Input = true;
                }
            };
            inputActions.PlayerMovement.LockOnLeft.performed += i =>
            {
                if (InventoryFlag == false)
                {
                    lockOnLeft_Input = true;
                }
            };
            inputActions.PlayerMovement.LockOnRight.performed += i =>
            {
                if (InventoryFlag == false)
                {
                    lockOnRight_Input = true;
                }
            };
            inputActions.PlayerActions.RB.performed += i =>
            {
                if (InventoryFlag==false)
                {
                    rb_Input = true;
                }
                
            };
            inputActions.PlayerActions.LB.performed += i =>
            {
                if (InventoryFlag==false)
                {
                    lb_Input = true;
                }
                
            };

           
            inputActions.PlayerActions.A.performed += i =>
            {
                if (InventoryFlag == false)
                {
                    a_Input = true;
                }
               
            };

        }

        public void OnDisable()
        {
            inputActions.Disable();
        }

        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
            uiManager=FindObjectOfType<UIManager>();
            actorController = GetComponentInChildren<ActorController>();
            interactBoolList = GetComponent<InteractBoolList>();
        }

        void Start()
        {   Dright = 0;
            Dmag = 0;   
            isDvecLock = false;
            lockDevcTimer = 0;
            isLookAngleTooBig = false;
            lookAngleDetectTimer = 0;
            lastLookAngle = CamerHandler.Single.lookAngle;
            lastIsisLookAngleTooBig = isLookAngleTooBig;
            InteractingBufferInit();
            BufferTimer = 0;
            TwoHandFlag = false;
        }

        // Update is called once per frame
        void Update()
        {   
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
           
            if (playerManager.isInteracting&&interactBoolList.InAttack.Any(func=>func())==false) //关闭输入
            {
                targetDright = 0;
                targetDup = 0;
                Dup = 0;
                Dright = 0;
                Dmag = 0;
            }
            else
            {
                HandleMoveInput();
            }
            HandleRollInput(Time.deltaTime);
            HandleRt_Input(Time.deltaTime);
            HandleQuickSlotsAndTwoHandChangeInput();
            HandleInventoryInput();
            HandleLockOnInput();
            HandleAltInput(Time.deltaTime);
            
            HandleInteractingInputBuffer();
        
        }

        private void LateUpdate()
        {
           
           
            a_Input = false;
            inventory_Input = false;
          
            
        }

        private void FixedUpdate()
        {
            
        }

        private Vector2 squaretocircle(Vector2 input)
        {
            Vector2 output = new Vector2();
            output.x = input.x * Mathf.Sqrt(1 - input.y * input.y / 2.0f);
            output.y = input.y * Mathf.Sqrt(1 - input.x * input.x / 2.0f);
            return output;
        }

        private void HandleMoveInput()
        {
            targetDup = (Input.GetKey(KeyUp) ? 1.0f : 0) - (Input.GetKey(KeyDown) ? 1.0f : 0);
            targetDright = (Input.GetKey(KeyRight) ? 1.0f : 0) - (Input.GetKey(KeyLeft) ? 1.0f : 0);


            Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.01f);
            Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.001f);
            // 变换成圆形区域
     
            
            Vector2 tempDAxis = squaretocircle(new Vector2(Dup, Dright));
            Dup2 = tempDAxis.x;
            Dright2 = tempDAxis.y;
            Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));
           
            if (isLookAngleTooBig == false) 
            {
                Dvec = Dright2 *CamerHandler.Single.transform.right + Dup2 *CamerHandler.Single.transform.forward;
            }
            else
            {
                Dvec = lastDvec;  //转向过大不会马上更新方向
            }
         
            
            lookAngleDetectTimer += Time.deltaTime;
            
            if (lookAngleDetectTimer > lookAngleDetectTimeWindow)
            { 
                if (Mathf.Abs(CamerHandler.Single.lookAngle - lastLookAngle )> 25f)
                {
                    isLookAngleTooBig = true;
                    lookAngleDetectTimer = 0;
                }

                lastDvec = Dvec;
                lastLookAngle = CamerHandler.Single.lookAngle;
                lookAngleDetectTimer = 0;
            }

            if (isLookAngleTooBig)
            {       
                lockDevcTimer+= Time.deltaTime;
                if (lockDevcTimer > 0.12f) //锁死时间
                {   
                    isLookAngleTooBig = false;
                }
            }
            else
            {
                lockDevcTimer = 0;
            }
            
            if (lastIsisLookAngleTooBig == true&&isLookAngleTooBig==false)
            {
                if (actorController.isTurnFast == false)
                {
                    actorController.TurnTimer = 0;
                }
                actorController.isTurnFast = true;
            }

            lastIsisLookAngleTooBig = isLookAngleTooBig;
         
        }

        private void HandleAltInput(float delta)
        {
            
            Alt_input = inputActions.PlayerActions.Alt.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
       
        }

        private void HandleRt_Input(float delta)
        {
            if (InventoryFlag==false)
            {
                rt_Input = inputActions.PlayerActions.RT.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            }
          
        }
        private void HandleRollInput(float delta)
        {
            b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            if (b_Input)
            {
                rollInputTimer += delta;
                if (actorController.inAirTimer > actorController.landThresholdTime)
                {
                    rollInputTimer = 0;
                }
            }
            else
            {
                rollInputTimer = 0;
            }
        }

 
        private void HandleQuickSlotsAndTwoHandChangeInput()
        {
           
            if (mouse.scroll.y.ReadValue() > 0&&InventoryFlag==false)
            {   
                scrollUp_Input = true;
            }

            if (mouse.scroll.y.ReadValue() < 0 && InventoryFlag == false)
            {   
                scrollDown_Input = true;
            }
        }

        private void HandleInventoryInput()
        {
            
            if (inventory_Input)
            {
                InventoryFlag=!InventoryFlag;
                if (InventoryFlag)
                {
                    uiManager.OpenSelectWindow();
                    uiManager.UpdateUI();
                    uiManager.HUDWindow.SetActive(false);
                }
                else
                {
                    uiManager.CloseSelectWindow();
                    uiManager.CloseAllInventoryWindows();
                    uiManager.HUDWindow.SetActive(true);
                }
            }
        }

        public void HandleLockOnInput()
        {
            if (LockOnFlag)
            {
                mouseLockCDTimer += Time.deltaTime;
            }
            
            if (mouseX > 10f&&mouseLockCDTimer > 0.5f)
            {
                mouseLockCDTimer = 0;
                lockOnRight_Input = true;
            }

            if (mouseX < -10f&&mouseLockCDTimer > 0.5f)
            {   mouseLockCDTimer = 0;
                lockOnLeft_Input = true;
            }
            
            if (lockOn_Input && LockOnFlag == false)
            {  
                lockOn_Input = false;
                cameraHandler.HandleLockOn();
          
                if (cameraHandler.nearestLockOnTargetTransform != null)
                {   Debug.Log("Lock On Input");
                    cameraHandler.currentLockOnTargetTransform = cameraHandler.nearestLockOnTargetTransform;
                    LockOnFlag = true;
                }
                else
                {
                   cameraHandler.ResetCameraToCharacter(actorController.model.transform.forward);
                   cameraHandler.ClearLockOnTargets();
                }
              
            }
            else if (lockOn_Input && LockOnFlag == true)
            {
                lockOn_Input = false;
                LockOnFlag=false;
                cameraHandler.ClearLockOnTargets();
            }

            if (LockOnFlag && lockOnLeft_Input)
            {
                lockOnLeft_Input = false;
                cameraHandler.HandleLockOn();
                if (cameraHandler.leftLockOnTargetTransform != null)
                {
                    cameraHandler.currentLockOnTargetTransform = cameraHandler.leftLockOnTargetTransform;
                }
            }
            if (LockOnFlag && lockOnRight_Input)
            {       
                lockOnRight_Input = false;
                Debug.Log("lockOnRight_Input");
                cameraHandler.HandleLockOn();
                if (cameraHandler.rightLockOnTargetTransform != null)
                {
                    cameraHandler.currentLockOnTargetTransform = cameraHandler.rightLockOnTargetTransform;
                }
            }
            cameraHandler.SetCameraHeight();
        }

        public void SetInventoryFlagFalse()
        {
            InventoryFlag = false;
        }
      
        private void InteractingBufferInit()
        {
            InteractingBuffers[InputType.scrollDown] = false;
            InteractingBuffers[InputType.scrollUp] = false;
            InteractingBuffers[InputType.rb] = false;
            InteractingBuffers[InputType.rt] = false;
            InteractingBuffers[InputType.lb] = false;
            InteractingBuffersTime[InputType.scrollDown] = 0.2f;
            InteractingBuffersTime[InputType.scrollUp] = 0.2f;
            InteractingBuffersTime[InputType.rb] = 0.4f;
            InteractingBuffersTime[InputType.lb] = 0.4f;
            InteractingBuffersTime[InputType.rt] = 0.2f;
        }   
        public void HandleInteractingInputBuffer()
        {   
            HandleInputBuffer(ref scrollDown_Input,InputType.scrollDown);
            HandleInputBuffer(ref scrollUp_Input,InputType.scrollUp);
            HandleInputBuffer(ref rb_Input,InputType.rb);
            HandleInputBuffer(ref lb_Input,InputType.lb);
            HandlePressInputBuffer(ref rt_Input,InputType.rt);
            BufferTimer+=Time.deltaTime;

            if (BufferTimer > 100f)
            {
                BufferTimer = 0;
            }
        }
        private void HandleInputBuffer(ref bool inputFlag, InputType inputType)
        {
            if (inputFlag)
            {
                BufferTimer = 0;
                if (!InteractingBuffers[inputType])
                {
                    InteractingBuffers[inputType] = true;
                }

                inputFlag = false;
            }

            if (BufferTimer > InteractingBuffersTime[inputType])
            {
                InteractingBuffers[inputType] = false;
            }
        }
        private void HandlePressInputBuffer(ref bool inputFlag, InputType inputType)
        {
            if (inputFlag)
            {
                BufferTimer = 0;
                if (!InteractingBuffers[inputType])
                {
                    InteractingBuffers[inputType] = true;
                }
            }

            if (BufferTimer > InteractingBuffersTime[inputType])
            {
                InteractingBuffers[inputType] = false;
            }
        }
    }
}