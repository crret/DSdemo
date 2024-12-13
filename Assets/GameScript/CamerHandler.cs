using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace SG
{


   public class CamerHandler : MonoBehaviour
   {
      public Transform targetTransform;
      public Transform cameraTransform;
      public Transform CameraPivotTransform;
      private Transform myTransform;
     
      public LayerMask ignoreLayers;

      public static CamerHandler singleton;
      public PlayerInput playerInput;
      public PlayerManager playerManager;
      public ActorController actorController;
      public static CamerHandler Single
      {
         get
         {
            if (singleton == null)
            {
               singleton = FindObjectOfType(typeof(CamerHandler)) as CamerHandler;
            }

            return singleton;
         }
      }

      public float lookSpeed = 0.1f;
      public float followSpeed = 0.1f;
      public float pivotSpeed = 0.03f;
      public float lockOnFollowSpeed = 0.1f;
      private float defaultPosition;
      private float targetPosition;
      public float lookAngle;
      private float pivotAngle;
      [SerializeField] private float minPivot = -35;
      [SerializeField] private float maxPivot = 35;
      [SerializeField] private float lockedPivotHeight= 2f;
      [SerializeField] private float unlockedPivotHeight= 1.6f;
      
      
      [Header("1-10")] public float lookSmooth = 1.0f;
      public float pivotSmooth = 1.0f;
      public float cameraHeight = 1.6f;

      public float cameraSphereRadius = 0.2f;
      public float cameraCollisionOffSet = 0.2f;
      public float minCollisionOffset = 0.2f;

      private Vector3 cameraDampVec = Vector3.zero;
      private Vector3 cameraDampVecLock = Vector3.zero;
      public float maxLockOnDistance = 10f;
      List<CharacterManager> availableTargets = new List<CharacterManager>();
      public Transform nearestLockOnTargetTransform;
      public Transform currentLockOnTargetTransform;
      public Transform leftLockOnTargetTransform;
      public Transform rightLockOnTargetTransform;
      
      private  Vector3 velocityForSmooth = Vector3.zero;
      private void Awake()
      {
         singleton = this;
         myTransform = transform;

         defaultPosition = cameraTransform.localPosition.z;
         ignoreLayers = ~(1 << 8|1<<9|1<<10|1<<12|1<<18);

      }

      private void LateUpdate()
      {
         
      }

      public void FollowTarget(float delta)
      {
         if (playerInput.LockOnFlag)
         {  float multi = 1f;
            if (Vector3.Angle(playerInput.Dvec, myTransform.forward) < 15f)
            {
               multi=Mathf.Lerp(multi,3f,0.3f);
            }
            else if (actorController.rolling)
            {
               multi = Mathf.Lerp(multi,5f,0.5f);
            }
            else
            {
               multi=Mathf.Lerp(multi,1f,delta*10f);
            }
            myTransform.position = Vector3.SmoothDamp(myTransform.position,
               targetTransform.position , ref cameraDampVecLock, delta / (lockOnFollowSpeed*multi));
           
         }
         else
         {  
            myTransform.position = Vector3.SmoothDamp(myTransform.position,
               targetTransform.position /*+ new Vector3(0, cameraHeight, 0)*/, ref cameraDampVec, delta / followSpeed);
         }
        
         // myTransform.position = targetTransform.position + new Vector3(0, cameraHeight, 0);
         Debug.DrawLine(myTransform.position, cameraTransform.position, Color.red);
         HandleCameraCollisions(delta);
      }

      public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
      {
         if (playerInput.LockOnFlag == false && currentLockOnTargetTransform == null && playerInput.InventoryFlag==false)
         {
            lookAngle += (mouseXInput * lookSpeed)/delta ;
            pivotAngle -= (mouseYInput * pivotSpeed)/delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot);
          //  lookAngle = Mathf.Repeat(lookAngle, 360f);
           
            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
         
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, Time.deltaTime*20f);
         
         
            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            targetRotation = Quaternion.Euler(rotation);
            CameraPivotTransform.localRotation =
               Quaternion.Slerp(CameraPivotTransform.localRotation, targetRotation, Time.deltaTime*20f);
         }
         else if(playerInput.LockOnFlag == true && currentLockOnTargetTransform != null)//锁定时
         {
            
            Vector3 dir = currentLockOnTargetTransform.position-transform.position;
            dir.Normalize();
            dir.y = 0;
        
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,  Time.deltaTime*10f);
            
          
            Vector3 rotation = Vector3.zero;
            rotation.x = 30f;
            targetRotation = Quaternion.Euler(rotation);
            CameraPivotTransform.localRotation =
               Quaternion.Slerp(CameraPivotTransform.localRotation, targetRotation, Time.deltaTime*20f);
         }
      }

      private void HandleCameraCollisions(float delta)
      {
         targetPosition = defaultPosition;
         RaycastHit hit;
         Vector3 direction = cameraTransform.position - CameraPivotTransform.position;
         Debug.DrawLine(cameraTransform.position, CameraPivotTransform.position, Color.blue);
         direction.Normalize();
         if (Physics.SphereCast(CameraPivotTransform.position, cameraSphereRadius, direction, out hit,
                Mathf.Abs(targetPosition), ignoreLayers))
         {
            float dis = Vector3.Distance(CameraPivotTransform.position, hit.point);
            targetPosition = -(dis - cameraCollisionOffSet);
         }

         if (Mathf.Abs(targetPosition) < minCollisionOffset)
         {
            targetPosition = minCollisionOffset;
         }

         cameraTransform.localPosition = new(cameraTransform.localPosition.x, cameraTransform.localPosition.y,
            Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f));

      }
      
      public void HandleLockOn()
      {
         float shortestDistance = Mathf.Infinity;
         float shortestDistanceOfLeftTarget = Mathf.Infinity;
         float shortestDistanceOfRightTarget = Mathf.Infinity;
         Collider[] colliders=Physics.OverlapSphere(targetTransform.position, 10f,1<<9);
         print(colliders.Length);
         for (int i = 0; i < colliders.Length; i++)
         {
            CharacterManager character = colliders[i].GetComponent<CharacterManager>();

            if (character != null)
            { 
               Vector3 lockTargetDirection = character.transform.position- targetTransform.position ;
               float distanceFromTarget=Vector3.Distance(targetTransform.position, character.transform.position);
               float viewAngle = Vector3.Angle(lockTargetDirection,cameraTransform.forward); //目标和摄像头方向的角度 角度太大不能锁定
            
               if (character.transform.root != targetTransform.transform.root && viewAngle > -90 && viewAngle < 90 &&
                   distanceFromTarget <= maxLockOnDistance)
               {
                  if (Physics.Linecast(playerManager.lockOnTransform.position, character.lockOnTransform.position,
                         LayerMask.GetMask("ground")) == false)
                  {
                     availableTargets.Add(character);
                  }
               }
            }
         }

         for (int k = 0; k < availableTargets.Count; k++)
         {  
            float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);
            if (distanceFromTarget < shortestDistance)
            {  
               shortestDistance = distanceFromTarget;
               nearestLockOnTargetTransform = availableTargets[k].lockOnTransform;
            }

            if (playerInput.LockOnFlag) //每次计算当前锁定目标的左右 所以每次切换目标都要调用
            {
               if (availableTargets[k].lockOnTransform != currentLockOnTargetTransform)
               {
                  Vector3 relativeEnemyPosition =
                     cameraTransform.InverseTransformPoint(availableTargets[k].transform.position);
                  var distanceFromLeftTarget = Mathf.Abs(relativeEnemyPosition.x);
                  var distanceFromRightTarget = Mathf.Abs(relativeEnemyPosition.x);
                  if (relativeEnemyPosition.x < 0.00 && distanceFromLeftTarget < shortestDistanceOfLeftTarget)
                  {
                     shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                     leftLockOnTargetTransform = availableTargets[k].lockOnTransform;
                  }

                  if (relativeEnemyPosition.x > 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                  {
                     shortestDistanceOfRightTarget = distanceFromRightTarget;
                     rightLockOnTargetTransform = availableTargets[k].lockOnTransform;
                  }
               }
            }
         }
         
      }

      public void ClearLockOnTargets()
      {
         availableTargets.Clear();
         currentLockOnTargetTransform = null;
         nearestLockOnTargetTransform = null;
         leftLockOnTargetTransform = null;
         rightLockOnTargetTransform = null;
      }
      public void ResetCameraToCharacter(Vector3 dir)
      {
        // StopAllCoroutines();
         StartCoroutine(SmoothResetCamera(dir));
         
      }

      private IEnumerator SmoothResetCamera(Vector3 dir)
      {  // 获取人物朝向的向量
         
         dir.Normalize();
         dir.y = 0;

         // 计算摄像头根节点的目标旋转
         Quaternion targetRotation = Quaternion.LookRotation(dir);

         // 设置摄像头根节点的旋转
         // 更新 lookAngle 和 pivotAngle
         lookAngle = myTransform.eulerAngles.y; // 获取摄像头在Y轴上的旋转角度
         ; // 获取摄像头支点在X轴上的旋转角度
         // 假设默认朝向是水平位置（0度俯仰角）
         float t = 0f; // 插值参数
         while (t < 1f)
         {
            t += Time.deltaTime * 5f; // 随时间增加插值参数

            // 平滑插值摄像头根节点的旋转
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, Time.deltaTime * 15f);
            lookAngle = myTransform.eulerAngles.y;
            // 平滑插值摄像头支点的局部旋转（俯仰角）
            pivotAngle = Mathf.Lerp(pivotAngle,0,Time.deltaTime * 15f);
            Vector3 tmprotation = Vector3.zero;
            tmprotation.x = pivotAngle;
            Quaternion rotationr = Quaternion.Euler(tmprotation);
            CameraPivotTransform.localRotation =
               Quaternion.Slerp(CameraPivotTransform.localRotation, rotationr, Time.deltaTime * 15f);
         
            yield return null; // 等待下一帧
         }
      }

      public void SetCameraHeight()
      {  
        
         Vector3 newLockedPosition = new Vector3(0, lockedPivotHeight,0);
         Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotHeight,0);
         if (currentLockOnTargetTransform != null)
         {
            CameraPivotTransform.transform.localPosition=Vector3.SmoothDamp( CameraPivotTransform.transform.localPosition,newLockedPosition,ref velocityForSmooth,Time.deltaTime*20f);
         }
         else
         {
            CameraPivotTransform.transform.localPosition =Vector3.SmoothDamp( CameraPivotTransform.transform.localPosition,newUnlockedPosition,ref velocityForSmooth,Time.deltaTime*20f);
         }
      }
   }
}