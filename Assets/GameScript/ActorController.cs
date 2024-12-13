                                                                                                                                  using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Serialization;

namespace SG
{


    public class ActorController : AnimatorManager
    {
        public GameObject model;
        public PlayerInput pi;
        public PlayerManager playerManager;
        public Animator anim;
        public IKtest iktest;
        public PlayerInventory playerInventory;
        public PlayerAttacker playerAttacker;
        public SkillEventManager skillEventManager;
        
        private Transform myTransform;

        [SerializeField] private float turnspeed = 0.1f;
        [SerializeField] private float runstartspeed = 0.1f;

        [SerializeField] private float fallspeed = 45;
    
        [SerializeField] private float sprintThreshold;
        [SerializeField] private float fallAnimPlayThreshold = 2.0f;
        public float landThresholdTime = 0.25f;
        [SerializeField] private float runMulti = 1.0f;
        [SerializeField] private float rollTimer;   
        public bool SprintFlag=false;
        public bool lockTurn = false;
        public bool rolling;
        public bool backStep;
        
        private bool stopTransitionTriggerFlag = false;
        private float previousValue;
        public Rigidbody rigid;
        
        [SerializeField] private Vector3 planarVec;

        [Header("planarVec的模")] [SerializeField]
        float sp;

        [Header("vector3.up在平面上投影的模")] [SerializeField]
        private float trace;

        private Vector3 normalVector;
        private Vector3 normalVectorLeft;
        private Vector3 normalVectorRight;
        [SerializeField]private Vector3 projectedDown;
        [SerializeField]private Vector3 projectedDownLeft;
        [SerializeField]private Vector3 projectedDownRight;
        private Vector3 thrustVec;
        private bool lockPlanar = false;
        private bool enterPlanar = false;
        private float enterPlanarTime=0f;
      
        public bool isTurnFast=false;
        public float TurnTimer = 0f;
        private Quaternion lastRotation;
        
        [SerializeField] private float groundDetectionRayStartPoint = 0.5f;
        [SerializeField] private float minDistanceNeededToBeginFall = 0.6f;
        [SerializeField] private float groundDetectionRayLength = 0.5f;
        LayerMask ignoreForGroundCheck;

        public float inAirTimer; //或解决短时间滞空导致动画问题

        public float height;
        public bool CheckDeltaPositionMag=false;
        public bool enableProjectDeltaPosition=false;
        // Start is called before the first frame update

        public PhysicMaterial frictionZero;
        public Collider playerCollider;
        public SphereCollider LeftFootCollider;
        public SphereCollider RightFootCollider;
        public bool isOriginHit = false;
        public RaycastHit hit;
        public bool isOnEdge=false;
        
        public bool isTwoHand=false;

        public AnimatorOverrideController overrideController;
        public LoadAnimationsFromFolder loadAnimationsFromFolder;
        public AnimationClipOverrides ClipOverrides;
        public InteractBoolList interactBoolList;
       
        void Awake()
        {
            pi = GetComponentInParent<PlayerInput>();

            anim = model.GetComponent<Animator>();
            rigid = GetComponentInParent<Rigidbody>();
            playerManager = GetComponentInParent<PlayerManager>();
            playerCollider= GetComponentInParent<Collider>();
            iktest = GetComponent<IKtest>();
            loadAnimationsFromFolder = GetComponentInParent<LoadAnimationsFromFolder>();
            interactBoolList = GetComponentInParent<InteractBoolList>();
            playerInventory=GetComponentInParent<PlayerInventory>();
            playerAttacker=GetComponentInParent<PlayerAttacker>();
            skillEventManager=GetComponentInParent<SkillEventManager>();
        }

        private void Start()
        {
            myTransform = pi.transform;
            playerManager.isInAir = false;
            
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11 | 1 << 10);
           
            enterPlanar = false;
            enterPlanarTime=0f;
            isTurnFast=false;
            TurnTimer = 0f;
            CheckDeltaPositionMag = true;
            enableProjectDeltaPosition = true;
            overrideController = new AnimatorOverrideController();
            overrideController.runtimeAnimatorController = anim.runtimeAnimatorController;
            anim.runtimeAnimatorController = overrideController;
            ClipOverrides = new AnimationClipOverrides(overrideController.overridesCount);
            isTwoHand = false;
            
        }

        // Update is called once per frame
        void Update()
        {   
            HandleLockOnFlag();
            HandleSprintFlag();
        
            anim.SetFloat("Dmag",pi.Dmag);
            playerManager.isInteracting = anim.GetBool("isInteracting");
            playerManager.isChangeWeapon = anim.GetBool("isChangeWeapon");
          //  playerAttacker.canDoCombo = anim.GetBool("canDoCombo");
            //  print(anim.IsInTransition(0)||anim.IsInTransition(1)||anim.IsInTransition(2)||anim.IsInTransition(3));
        }

        private void OnAnimatorMove()
        {
            sp = planarVec.magnitude;
            trace = projectedDown.magnitude;
          
         
            //分为在地面和空中的情况
            //地面时,移动时用planarVec控制播放动画用deltapostion
            //空中，可能在移动或播放动画时进入空中，用rigid.velocity.y 
            if (playerManager.isInAir == false)
            {
                    if (anim.GetBool("isAnimPlaying"))
                    {
                        if (anim.IsInTransition(0) && anim.GetBool("CancelRMInTransition")) //过度时取消了RM 用planarVec
                        {

                            rigid.velocity = planarVec;
                        }
                        else
                        {
                            HandleRigidVelocityInAnim();
                        }
                    }
                    else
                    {
                       
                        rigid.velocity = planarVec;
                        if (isOnEdge)
                        {
                            rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z);
                        }
                    }
                    
            }
            else
            {
                if (anim.GetBool("isAnimPlaying") && anim.IsInTransition(0) == false)
                {
                    HandleRigidVelocityInAnimAndAir();
                }
                else //处于边缘时可能进入isInAir 此时用planarVec速度值非常小(投影量小) 此外在inAir状态需要用rigid.velocity.y
                {
                    rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y,planarVec.z);
                }
            }
            

        }

        void FixedUpdate()
        {
           

        }

      
        public Vector3 DeltaPosition()
        {
            if (enableProjectDeltaPosition)
            {
                if (CheckDeltaPositionMag == true)
                {   
                    CheckDeltaPositionMag = false;
                    if (anim.deltaPosition.magnitude > 0.1f)
                    {   
                        return Vector3.Project(anim.deltaPosition,model.transform.forward)*0.01f;
                    }
                
                }
                return Vector3.Project(anim.deltaPosition,model.transform.forward);

            }
            else
            {
                if (CheckDeltaPositionMag == true)
                {
                    CheckDeltaPositionMag = false;
                    if (anim.deltaPosition.magnitude > 0.1f)
                    {   
                        return anim.deltaPosition*0.01f;
                    }
                
                }

                return anim.deltaPosition;
            }
          
        }
        
        public void HandleRigidVelocityInAnim()
        {
            Vector3 deltaPostion = DeltaPosition();
         
            deltaPostion.y = 0;
        
            Vector3 velocity =deltaPostion/Time.deltaTime; ;
            rigid.velocity = Vector3.ProjectOnPlane(velocity,normalVector);
            if (isOnEdge)
            {
                rigid.velocity=new Vector3(velocity.x,rigid.velocity.y,velocity.z);
            }
        }

      
        public void HandleRigidVelocityInAnimAndAir()
        {
            Vector3 deltaPostion = DeltaPosition();
           
            deltaPostion.y = 0;
        
            Vector3 velocity =deltaPostion/Time.deltaTime; 
            rigid.velocity = new Vector3(velocity.x,rigid.velocity.y, velocity.z);
        }
        public void HandleRotation(float delta)
        {
            if (pi.Dmag > 0.01f && (anim.GetBool("isAnimPlaying") == false || anim.IsInTransition(0) == true) && 
                lockPlanar == false) //移动时
            {
                /*model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec,
                    (float)(1 - Math.Pow(Convert.ToDouble(turnspeed / 10000),
                        Convert.ToDouble(delta))));*/

                if (pi.LockOnFlag == false)
                {
                    if (Vector3.Angle(model.transform.forward, pi.Dvec) > 165f)//从A到D时
                    {
                        TurnTimer = 0f;
                        isTurnFast = true;
                    }
                }

                if (isTurnFast == false)
                {   
                    if (pi.LockOnFlag&&SprintFlag==false)
                    {   
                        Vector3 dir = CamerHandler.Single.currentLockOnTargetTransform.position-model.transform.position;
                        dir.Normalize();
                        dir.y = 0;
            
                        Quaternion targetRotation = Quaternion.LookRotation(dir);
                        
                        if (pi.Dmag > 0.8f)//----------
                        {
                            model.transform.rotation = Quaternion.Slerp( model.transform.rotation, targetRotation,  delta*turnspeed*0.5f);
                        }
                      
                        
                    }
                    else
                    {   
                        model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec,delta*turnspeed);
                    }
                    
                }
                else
                {   
                    TurnTimer+=Time.deltaTime;
                    model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec,delta*turnspeed*0.4f);
                    if (TurnTimer > 0.3f) //过渡时间
                    {  
                        isTurnFast = false;
                    }
                    
                }
                
                //模型方向
            }  //动画过渡时就能转 但注意一般是在其他动画转ground的时候
            else if(anim.GetBool("isAnimPlaying"))
            {
                /*if (pi.LockOnFlag&&anim.GetBool("isAnimPlaying") == false)
                {   
                        
                    Vector3 dir = CamerHandler.Single.currentLockOnTargetTransform.position-model.transform.position;
                    dir.Normalize();
                    dir.y = 0;
                    Quaternion targetRotation = Quaternion.LookRotation(dir);
                    model.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,  delta*turnspeed*0.5f);//*0.5和unlock时的区别
                }*/
                if (anim.GetBool("isTurnableAnimPlaying"))
                {
                    if (pi.LockOnFlag)
                    {
                        Vector3 dir = CamerHandler.Single.currentLockOnTargetTransform.position-model.transform.position;
                        dir.Normalize();
                        dir.y = 0;
                        Quaternion targetRotation = Quaternion.LookRotation(dir);
                        model.transform.rotation = Quaternion.Slerp(model.transform.rotation, targetRotation,  delta*turnspeed*0.5f);
                    }
                    else
                    {
                       StartCoroutine(TurnWhenAnimPlaying(pi.Dvec));
                    }
                }
                
                
            }
            else
            {

                if (pi.LockOnFlag)
                {
                    Vector3 dir = CamerHandler.Single.currentLockOnTargetTransform.position-model.transform.position;
                    dir.Normalize();
                    dir.y = 0;
                    
                    Quaternion targetRotation = Quaternion.LookRotation(dir);
                    if (Vector3.Angle(model.transform.forward, dir) > 90f)
                    {
                        PlayTargetAnimFromOther("TurnTransition");
                    }
                    if (lockTurn)
                    {
                        model.transform.rotation = Quaternion.Slerp(model.transform.rotation, targetRotation,  delta*turnspeed*0.5f);
                    }
                }
               
            }
            
        }

        IEnumerator TurnWhenAnimPlaying(Vector3 Direction)
        {   anim.SetBool("isTurnableAnimPlaying", false);
            float t = 0;
            while (t < 0.5f)
            {   
                t += Time.deltaTime;
                model.transform.forward =Vector3.Slerp(model.transform.forward, Direction,  Time.deltaTime*30f);
                yield return null;
            }
        }

        
        public void HandleMovement(float delta)
        {
            if (pi.LockOnFlag == false||SprintFlag)
            {
                /*anim.SetFloat("forward", Mathf.Lerp(anim.GetFloat("forward"), pi.Dmag * runMulti,
                    (float)(1 - Math.Pow(Convert.ToDouble(runstartspeed / 10000),
                        Convert.ToDouble(delta)))));*/
                if (Mathf.Abs(anim.GetFloat("right")) > Mathf.Abs(anim.GetFloat("forward")))
                {
                    
                    anim.SetFloat("forward", Mathf.Abs(anim.GetFloat("right")),0.1f,Time.deltaTime);
                }//避免锁定解锁时的问题
                
                /*if (anim.GetFloat("forward") > 1f)
                {
                    stopTransitionTriggerFlag = true;
                }
                
                if (stopTransitionTriggerFlag  && anim.GetFloat("forward") < 1f&&SprintFlag==false)
                {
                    anim.SetTrigger("StopTransitionTrigger");
                }*/
                
                float targetforward = pi.Dmag * runMulti;
                if (targetforward < 0.1)
                {
                    anim.SetFloat("forward",pi.Dmag*runMulti,0.2f,Time.deltaTime);
                }
                else 
                {
                    anim.SetFloat("forward",pi.Dmag*runMulti,0.078f,Time.deltaTime);
                }
               //前进动画forward值设置
                anim.SetFloat("right",0f);
            }
            else
            {
                HandleLockOnAnimFloat(Time.deltaTime);
            }

            if (lockPlanar == false)
            {   
                if (isOnEdge == false)  //不在边缘 用三个法向量中小于0.6的决定目前平面的坡度（法向量）
                {   
                    Vector3[] NormalVectors = { hit.normal,iktest.leftrayHit.normal, iktest.rightrayHit.normal };
                    List<Vector3> vectors = new List<Vector3>();
                    if (projectedDown.magnitude < 0.6f)
                    {
                            vectors.Add(hit.normal);
                    }
                    if (projectedDownLeft.magnitude < 0.6f)
                    {
                            vectors.Add(iktest.leftrayHit.normal);
                    }
                    if (projectedDownRight.magnitude < 0.6f)
                    {
                            vectors.Add(iktest.rightrayHit.normal);
                    }

                    if (vectors.Count > 0)
                    {
                            // 初始化一个零向量来存储总和
                      Vector3 averageVector = Vector3.zero;
                      // 累加所有符合条件的向量
                      foreach (Vector3 vec in vectors)
                      {
                          averageVector += vec;
                      }
                      // 计算均值 三个法向量的均值
                      averageVector /= vectors.Count;
                      if (enterPlanar) //从空中进入地面时Lerp速度
                      {   
                          enterPlanarTime+=Time.deltaTime;
                          planarVec=Vector3.Lerp(planarVec,Vector3.ProjectOnPlane(pi.Dvec, normalVector) *
                                                           pi.Dmag * anim.velocity.magnitude,Time.deltaTime*10f);
                          if (enterPlanarTime > 0.2f) //意思是在0.2f之内lerp
                          {
                              enterPlanar = false;
                          }
                      }
                      else
                      {
                          
                          if (pi.LockOnFlag&&SprintFlag==false)
                          {
                              if (anim.velocity.magnitude > 3.9f)
                              {   
                                  planarVec = Vector3.ProjectOnPlane(pi.Dvec, normalVector) *
                                              pi.Dmag * 3.9f;
                              }
                              else
                              {
                                  planarVec = Vector3.ProjectOnPlane(pi.Dvec, normalVector) *
                                              pi.Dmag * anim.velocity.magnitude;
                              }
                             
                          }
                          else
                          {  
                              planarVec = Vector3.ProjectOnPlane(pi.Dvec, averageVector) *
                                         pi.Dmag * anim.velocity.magnitude;
                              
                          }
                        
                      }
                   
                    }
                    else //非正常坡度 主要是保证xz轴上有速度不交界处卡住  且给一个斜面向下的速度往下掉
                    {   Vector3 cliifVector = NormalVectors.Aggregate(Vector3.zero, (acc, vec) => acc + vec) / NormalVectors.Length;
                        if (Vector3.ProjectOnPlane(-Vector3.up, cliifVector).magnitude < 0.6f)
                        {
                            planarVec =new Vector3(anim.velocity.magnitude* pi.Dmag*pi.Dvec.x,rigid.velocity.y,anim.velocity.magnitude* pi.Dmag*pi.Dvec.z);
                        }
                        else
                        {  
                            planarVec = pi.Dvec * pi.Dmag * anim.velocity.magnitude +Vector3.ProjectOnPlane(-Vector3.up, cliifVector)* 10;
                            if (pi.Dmag < 0.2f)
                            {
                                planarVec = Vector3.ProjectOnPlane(-Vector3.up, cliifVector) * 10;
                            }
                            
                        }
                    }
                }
                else
                {
                    planarVec =new Vector3(anim.velocity.magnitude* pi.Dmag*pi.Dvec.x,rigid.velocity.y,anim.velocity.magnitude* pi.Dmag*pi.Dvec.z);
                }

            }
            else
            {
                enterPlanarTime = 0;
                enterPlanar=true;
            }
        }


       
     

        public void SetIsTurnableAnimPlayingTrue()
        {
            anim.SetBool("isTurnableAnimPlaying", true);
        }
        public void SetIsTurnableAnimPlayingFalse()
        {
            anim.SetBool("isTurnableAnimPlaying", false);
        }

        public bool PlayComboAnim(string targetAnim, float Transition, bool checkDeltaPosition,bool enableProject)
        {  if (playerAttacker.canDoCombo == false)
            {
                return false;
            }
            CheckDeltaPositionMag = checkDeltaPosition;
            enableProjectDeltaPosition=enableProject;
            
            anim.CrossFadeInFixedTime(targetAnim, Transition); //crossFade无法打断自身动画
            Debug.Log(anim.GetNextAnimatorStateInfo(0).IsName(targetAnim));
            IEnumerator startEvent()
            {
                bool flag = false;

                while (flag == false)
                {
                    foreach (var animClip in anim.GetNextAnimatorClipInfo(0))
                    {
                        skillEventManager.ExecuteAnimationEvents(animClip.clip);
                        flag = true;
                        Debug.Log("execute");
                    }
                    yield return null;
                }

            }
            StartCoroutine(startEvent());
            
            return true;
        }
        
        public bool PlayTargetAnim(string targetAnim,float Transition,bool checkDeltaPosition)
        {
            if (interactBoolList.CanAnimPlay() == false)
            {
                return false;
            }

            if (checkDeltaPosition == true)
            {
                CheckDeltaPositionMag = true;
            }
         
            anim.CrossFadeInFixedTime(targetAnim, Transition); //crossFade无法打断自身动画
            return true;
        }
        public bool PlayTargetAnim(string targetAnim,float Transition,bool checkDeltaPosition,bool enableProject)
        {
            if (interactBoolList.CanAnimPlay() == false)
            {
                return false;
            }

            if (checkDeltaPosition == true)
            {
                CheckDeltaPositionMag = true;
            }

            if (enableProject == false)
            {
              enableProjectDeltaPosition=false;
            }
         
            anim.CrossFadeInFixedTime(targetAnim, Transition); //crossFade无法打断自身动画
            return true;
        }
        
        public bool PlayTargetAnim(string targetAnim)
        {
            if (interactBoolList.CanAnimPlay() == false)
            {
                return false;
            }
            
            anim.CrossFadeInFixedTime(targetAnim, Time.deltaTime); //crossFade无法打断自身动画
            return true;

        }
        public bool PlayTargetAnim(string targetAnim,float TransitionDuration,string animlayer)
        {
            if (interactBoolList.CanAnimPlay() == false)
            {
                return false;
            }
            anim.CrossFadeInFixedTime(targetAnim, TransitionDuration,anim.GetLayerIndex(animlayer)); //crossFade无法打断自身动画
            return true;
        }
        
        public bool PlayTargetAnim(string targetAnim,float Transition)
        {   
            if (interactBoolList.CanAnimPlay() == false)
            {   print(interactBoolList.CanAnimPlay());
                return false;
            }
            
            anim.CrossFadeInFixedTime(targetAnim, Transition); //crossFade无法打断自身动画
            return true;

        }
        public bool PlayTargetAnimFromOther(string targetAnim)
        {
            if (interactBoolList.CanAnimPlay() == false)
            {
                return false;
            }
            anim.CrossFade(targetAnim, Time.deltaTime); //crossFade无法打断自身动画
            return true;
        }
        public bool PlayTargetAnimFromOther(string targetAnim,float transDuration)
        {
            if (interactBoolList.CanAnimPlay() == false)
            {
                return false;
            }
            anim.CrossFade(targetAnim, transDuration); //crossFade无法打断自身动画
            return true;
        }
        public bool PlayTargetAnimImmediately(string targetAnim,float Transition)
        {

            anim.CrossFadeInFixedTime(targetAnim, Transition); //crossFade无法打断自身动画
            return true;

        }

        public void HandleRollingAndSprinting(float delta)
        {

            if (anim.GetBool("isInteracting"))
            {
                return;
            }
            
            
            if (pi.rollInputTimer > 0)
            {
                if (pi.rollInputTimer < sprintThreshold + 0.3f)
                {
                    rollTimer = pi.rollInputTimer;
                }

                if (pi.Dmag > 0.02f)
                {
                    if (rollTimer > sprintThreshold)
                    {
                        runMulti = 3.0f;
                        rollTimer = 0;
                        SprintFlag=true;
                    }
                }
                else
                {
                    if (pi.rollInputTimer > 0.05f && pi.rollInputTimer < 0.06f)
                    {
                        PlayTargetAnim("BackStep");

                        rollTimer = 0;
                    }

                }
            }
            else
            {

                runMulti = 1.5f;
                SprintFlag=false;
                if (rollTimer > 0.01f && pi.Dmag > 0.02f)
                {
                    rollTimer = 0;

                    if (pi.LockOnFlag == false)
                    {
                        Quaternion rollRotation = Quaternion.LookRotation(pi.Dvec);
                        model.transform.rotation = rollRotation;
                        PlayTargetAnim("ROLL",Time.deltaTime,true,true);
                    }
                    else
                    {
                        if (pi.Dup2 > -0.86 && pi.Dup2 < 0.86)
                        {
                            if (pi.Dright2 <-0.3)
                            {
                                model.transform.right=-pi.Dvec;
                                PlayTargetAnim("RollLeft",Time.deltaTime,true,false);
                            }
                            else if (pi.Dright2 > 0.3)
                            {
                                model.transform.right=pi.Dvec;
                                PlayTargetAnim("RollRight",Time.deltaTime,true,false);
                            }
                        }
                        else
                        {
                            if (pi.Dup2 > 0.86)
                            {
                                model.transform.forward=pi.Dvec;
                                PlayTargetAnim("ROLL",Time.deltaTime,true,true);
                            }
                            else
                            {
                                model.transform.forward=-pi.Dvec;
                                PlayTargetAnim("RollBack",Time.deltaTime,true,true);
                            }
                        }
                    
                    }
                    
                }
                else
                {
                    rollTimer = 0;
                }
            }
        }

        public void HandleFalling(float delta)
        {  
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint; //origin 射线发射点
            Ray ray = new Ray(origin, -Vector3.up);
            if (Physics.Raycast(ray, out hit, 0.7f, ignoreForGroundCheck))
            {
                isOriginHit = true;
                normalVector = hit.normal;
                projectedDown = Vector3.ProjectOnPlane(-Vector3.up, normalVector);
            }
            else
            {
                isOriginHit = false;
            }
            RaycastHit hitForHeight;
            //origin 射线发射点
            Ray rayH = new Ray(origin, -Vector3.up);
            Physics.Raycast(rayH, out hitForHeight, 1000, ignoreForGroundCheck);
         
            height = hitForHeight.distance;
           
            normalVectorLeft = iktest.leftrayHit.normal;
            normalVectorRight = iktest.rightrayHit.normal;

            if (iktest.isLeftHit)
            {
                projectedDownLeft = Vector3.ProjectOnPlane(-Vector3.up, normalVectorLeft);
            }

            if (iktest.isRightHit)
            {
                projectedDownRight=Vector3.ProjectOnPlane(-Vector3.up, normalVectorRight);
            }
            
            if (iktest.isRightHit && iktest.isLeftHit && isOriginHit)
            {
                isOnEdge = false;
            }
            else
            {
                isOnEdge = true;
            }
            
            Debug.DrawRay(origin, -Vector3.up * 0.7f, Color.red, 0.1f, false);
            Debug.DrawRay(origin, projectedDown, Color.green, 0.1f, false);
            Debug.DrawRay(iktest.leftrayHit.point, projectedDownLeft, Color.green, 0.1f, false);
            Debug.DrawRay(iktest.rightrayHit.point, projectedDownRight, Color.green, 0.1f, false);
            Debug.DrawRay(hit.point, normalVector*minDistanceNeededToBeginFall, Color.cyan, 0.1f, false);
            
            if (anim.GetBool("isGround"))
            {
                lockPlanar = false;
                playerCollider.material = null;//---------------------
                if (playerManager.isInAir)
                {
                    if (inAirTimer > landThresholdTime) //不能用height来判断 落地时height已经很小
                    {
                        PlayTargetAnim("04200");
                        inAirTimer = 0; //空中时间达到一定时间落地播放land
                    }
                    else
                    {
                        if (playerManager.isInteracting == false) //斜坡上翻滚可能短暂进入isInAir导致ground打断翻滚动画
                        {
                            if (pi.LockOnFlag)
                            {
                                PlayTargetAnimFromOther("Move");
                            }
                            else
                            {
                                PlayTargetAnimFromOther("ground");
                            }
                           
                        }
                        inAirTimer = 0;
                    }

                    playerManager.isInAir = false;
                }
            }
            else
            {
                lockPlanar = true;
                playerManager.isInAir = true;
                playerCollider.material = frictionZero;
                if (playerManager.isInteracting == false && height > fallAnimPlayThreshold) //翻滚后掉落不打断动画
                {   
                    PlayTargetAnimFromOther("fall",0.2f); //掉落动画不要isInteracting
                    
                }

            }


        }

        public void HandleJumping()
        {
         
        }

      
        public void  HandleAltMove()
        {
            if (pi.Alt_input)
            {
                anim.SetBool("isAlt",true);
            }
            else
            {
                anim.SetBool("isAlt", false);
            }
            
        }

        public void HandleLockOnFlag()
        {
            if (pi.LockOnFlag)
            {
                anim.SetBool("isLockOn",true);
            }
            else
            {
                anim.SetBool("isLockOn",false);
            }
        }
        public void HandleSprintFlag()
        {
            if (SprintFlag)
            {
                anim.SetBool("isSprinting",true);
            }
            else
            {
                anim.SetBool("isSprinting",false);
            }
        }

        public void HandleRest()
        {
           
        }

        public void HandleLockOnAnimFloat(float delta)
        {
            Vector3 ModelToTarget = CamerHandler.Single.currentLockOnTargetTransform.position - playerManager.lockOnTransform.position;
            Vector3 CameraToTarget = CamerHandler.Single.currentLockOnTargetTransform.position - CamerHandler.Single.CameraPivotTransform.transform.position;
            Debug.DrawRay(CamerHandler.Single.currentLockOnTargetTransform.position,-ModelToTarget, Color.blue, 0.1f, false);
            Debug.DrawRay(CamerHandler.Single.currentLockOnTargetTransform.position,-CameraToTarget, Color.blue, 0.1f, false);
            
            Vector3 modelToTargetInCameraSpace = CamerHandler.Single.transform.InverseTransformDirection(ModelToTarget);
            Vector3 cameraToTargetInCameraSpace = CamerHandler.Single.transform.InverseTransformDirection(CameraToTarget);

            modelToTargetInCameraSpace.y = 0;  // 忽略Y轴
            cameraToTargetInCameraSpace.y = 0;
            float angle = Vector3.Angle(modelToTargetInCameraSpace, cameraToTargetInCameraSpace);
            
            // 通过叉积判断方向
            Vector3 crossProduct = Vector3.Cross(modelToTargetInCameraSpace, cameraToTargetInCameraSpace);
            Debug.DrawRay(CamerHandler.Single.transform.position,crossProduct,Color.red, 0.1f, false);
            // 判断角度的正负
            if (crossProduct.y < 0)
            {
                angle = -angle;
            }

            
            
            if (angle> -15f)
            {   
                anim.SetFloat("LegFreeLeft",Mathf.Lerp(anim.GetFloat("LegFreeLeft"),1,Time.deltaTime*20f));
                
            }
            else
            {   
                anim.SetFloat("LegFreeLeft",Mathf.Lerp(anim.GetFloat("LegFreeLeft"),0,Time.deltaTime*10f));
            }
            if (angle < 15f)
            {
                anim.SetFloat("LegFreeRight",Mathf.Lerp(anim.GetFloat("LegFreeRight"),1,Time.deltaTime*20f));
            }
            else
            {
                anim.SetFloat("LegFreeRight",Mathf.Lerp(anim.GetFloat("LegFreeRight"),0,Time.deltaTime*10f));
            }

     
            anim.SetFloat("forward",Mathf.Lerp(anim.GetFloat("forward"),pi.Dup*runMulti,delta*15f));
            if (Mathf.Abs(anim.GetFloat("forward")) > 1f)
            {
                if (Mathf.Abs(anim.GetFloat("right")) > 0.3f)
                { 
                    anim.SetFloat("right",Mathf.Lerp(anim.GetFloat("right"),pi.Dright*runMulti,delta*5f));
                }
                else
                {
                    anim.SetFloat("right",Mathf.Lerp(anim.GetFloat("right"),pi.Dright*runMulti,delta*1.5f));
                }
                
            }
            else
            {
                anim.SetFloat("right",Mathf.Lerp(anim.GetFloat("right"),pi.Dright*runMulti,delta*20f));
            }

           
        }

        public IEnumerator OverrideWeaponAnim(WeaponItem weaponItem,bool isLeft,bool isTwoHand)
        {
            /*anim.SetLayerWeight(anim.GetLayerIndex("TwoHand"),1);
            PlayTargetAnim(weaponItem.TWO_HAND_IDLE,"TwoHand");*/
            anim.SetBool("isChangeWeapon",true);
            overrideController.GetOverrides(ClipOverrides);
            ClipOverrides.OverrideTransitionClips(weaponItem,isLeft,isTwoHand);
            overrideController.ApplyOverrides(ClipOverrides);
            float t = 0f; 
            while (t < 0.5f)
            {   float layerWeight = anim.GetLayerWeight(anim.GetLayerIndex("Transition"));
                layerWeight = Mathf.Lerp(layerWeight, 1, Time.deltaTime * 10f);
                anim.SetLayerWeight(anim.GetLayerIndex("Transition"), layerWeight);
                t += Time.deltaTime;
                yield return null; 
            }
            overrideController.GetOverrides(ClipOverrides);
            ClipOverrides.OverrideClips(weaponItem,isLeft,isTwoHand);
            overrideController.ApplyOverrides(ClipOverrides);
            anim.SetLayerWeight(anim.GetLayerIndex("Transition"), 0);
            anim.SetBool("isChangeWeapon",false);
        }

        
        public void SetHeavyAttackSpTrue()
        {
            playerAttacker.heavyAttackSp =true;
        }

        public void SetHeavyAttackSpFalse()
        {
            playerAttacker.heavyAttackSp = false;
        }
        
        public void SetIsInvulerableTrue()
        {
            playerManager.isInvulerable = true;
        }
        public void SetIsInvulerableFalse()
        {
            playerManager.isInvulerable = false;
        }

        public void TestSO()
        {
            Debug.Log("TestSo "+Time.time);
        }
    }

}