using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SG
{


    public class IKtest : MonoBehaviour
    {   [SerializeField]
        private Animator anim;
 
        public CapsuleCollider capsuleCollider;
        public SphereCollider sphereColliderleft;
        public SphereCollider sphereColliderright;
        public PlayerInput playerInput;
        private Vector3 tempVec=Vector3.zero;
        private Vector3 Down=Vector3.zero;
        private Vector3 tempVec2;
        private Vector3 leftFootIk,rightFootIk;
        private Vector3 leftFootTargetPosition,rightFootTargetPosition;
        private Quaternion leftFootIKRotation,rightFootIKRotation;
        private Quaternion leftFootTargetRotation,rightFootTargetRotation;
        [SerializeField] private LayerMask IkLayer;
        [SerializeField] [Range(0,0.2f)] private float rayHitOffset=0.15f;
        [SerializeField] [Range(0,1f)] private float leftFootWeight = 1f;
        [SerializeField] [Range(0,1f)] private float rightFootWeight = 1f;
        [SerializeField] [Range(0,1f)] private float legHeight = 1f;
        [SerializeField] [Range(0,0.2f)] private float footCapsuleLenth = 0.2f;
        [SerializeField] private float rayCastDistance;
        [SerializeField] private float maxFootCauseBodyDown=0.6f;
        [SerializeField] private bool enableIK=true;
      //  [SerializeField] private float IKSphereRadius=0.05f;
     //   [SerializeField] private float positionSphereRadius=0.05f;
        public RaycastHit leftrayHit;
        public RaycastHit rightrayHit;
        public bool isLeftHit=false;
        public bool isRightHit=false;
        
        public bool LeftFootIKOn=false;
        public bool RightFootIKOn=false;
        private float vecForSmooth1 = 0;
        private float vecForSmooth2 = 0;
        private float vecForSmooth3 = 0;
        private float vecForSmooth4 = 0;
        private float vecForSmooth5 = 0;
        private float vecForSmooth6= 0;
        private float vecForSmooth7 = 0;
        private float vecForSmooth8 = 0;
        private void Awake()
        {
            anim = GetComponent<Animator>();
            capsuleCollider = GetComponentInParent<CapsuleCollider>();
            playerInput = GetComponentInParent<PlayerInput>();
        }

        private void Update()
        {
            if (playerInput.Dmag>0.6f)
            {   
                leftFootWeight = leftFootWeight = Mathf.SmoothDamp(leftFootWeight,0f,ref vecForSmooth1,Time.deltaTime*20f);
                rightFootWeight =  rightFootWeight = Mathf.SmoothDamp(rightFootWeight,0f,ref vecForSmooth3,Time.deltaTime*20f);
            }
            else if (anim.GetBool("isAnimPlaying"))
            {
                if (LeftFootIKOn)
                {
                    leftFootWeight = Mathf.SmoothDamp(leftFootWeight,1f,ref vecForSmooth1,Time.deltaTime*10f);
                }
                else
                {
                    leftFootWeight = Mathf.SmoothDamp(leftFootWeight,0f,ref vecForSmooth2,Time.deltaTime);
                }

                if (RightFootIKOn)
                {
                    rightFootWeight = Mathf.SmoothDamp(rightFootWeight,1f,ref vecForSmooth3,Time.deltaTime*10f);
                }
                else
                {
                    rightFootWeight = Mathf.SmoothDamp(rightFootWeight,0,ref vecForSmooth4,Time.deltaTime);
                }
            }
            else
            {
                if (Vector3.Angle(Vector3.up, leftrayHit.normal) < 50f&&isLeftHit)
                {
                    leftFootWeight = Mathf.SmoothDamp(leftFootWeight,1f,ref vecForSmooth5,Time.deltaTime*20f);
                }
                else
                {
                    leftFootWeight = Mathf.SmoothDamp(leftFootWeight,0f,ref vecForSmooth6,Time.deltaTime*10f);
                }

                if (Vector3.Angle(Vector3.up, rightrayHit.normal) < 50f&&isRightHit)
                {
                    rightFootWeight = Mathf.SmoothDamp(rightFootWeight,1f,ref vecForSmooth7,Time.deltaTime*20f);
                }
                else
                {
                    rightFootWeight = Mathf.SmoothDamp(rightFootWeight,0,ref vecForSmooth8,Time.deltaTime*10f);
                }
            }
        }

        private void OnAnimatorIK(int layer)
        {
            
            if (!enableIK)
            {
                return;
            }
            leftFootIk=anim.GetIKPosition(AvatarIKGoal.LeftFoot);
            rightFootIk=anim.GetIKPosition(AvatarIKGoal.RightFoot);
            leftFootIKRotation=anim.GetIKRotation(AvatarIKGoal.LeftFoot);
            rightFootIKRotation=anim.GetIKRotation(AvatarIKGoal.RightFoot);
            
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot,leftFootWeight);
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot,rightFootWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot,leftFootWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot,rightFootWeight);
            
            anim.SetIKPosition(AvatarIKGoal.LeftFoot,leftFootTargetPosition);
            anim.SetIKPosition(AvatarIKGoal.RightFoot,rightFootTargetPosition);
            anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootTargetRotation);
            anim.SetIKRotation(AvatarIKGoal.RightFoot, rightFootTargetRotation);
            /*if (anim.GetFloat("forward") < 1f)
            {*/
                float minFootY = Mathf.Min(leftFootTargetPosition.y, rightFootTargetPosition.y);
                float down = anim.bodyPosition.y - (minFootY + legHeight); //腿长legHeight可调节
             //限定一下最大下沉距离
                if (down < 0||down>maxFootCauseBodyDown)
                {
                    down = 0;
                }

//不要直接修改骨盆IK位置，而是叠加上我们腿部的偏移
               // anim.bodyPosition=anim.bodyPosition + new Vector3(0, -down, 0);
               Down = Vector3.SmoothDamp(Down, new Vector3(0, -down, 0), ref tempVec, 0.1f);
                anim.bodyPosition =anim.bodyPosition+Down ;
            /*}
            else
            {
                Down = Vector3.zero;
            }*/
        
        }

        private void FixedUpdate()
        {   var leftFootLocalUp =leftFootIKRotation * Vector3.up;
           
            var rightFootLocalUp = rightFootIKRotation * Vector3.up;
            
          
            Ray leftRay = new Ray(leftFootIk+Vector3.up,-Vector3.up );
            Ray rightRay=new Ray(rightFootIk+Vector3.up,-Vector3.up );
        
            if (Physics.Raycast(leftRay, out leftrayHit, 2f, LayerMask.GetMask("ground")))
            {   isLeftHit=true;  
                Debug.DrawRay(leftrayHit.point,leftrayHit.normal,Color.magenta,Time.deltaTime);
                    leftFootTargetPosition = leftrayHit.point + Vector3.up * rayHitOffset;
                   
                    var rotAxisLeft = Vector3.Cross(leftFootLocalUp, leftrayHit.normal);
                    var angleLeft = Vector3.Angle(leftFootLocalUp, leftrayHit.normal);
                    
                    leftFootTargetRotation = Quaternion.AngleAxis(angleLeft, rotAxisLeft) * leftFootIKRotation;
            }
            else
            {   isLeftHit=false;
                leftFootTargetPosition = leftFootIk;
                leftFootTargetRotation = leftFootIKRotation;

            }
            if (Physics.Raycast(rightRay, out rightrayHit, 2f, LayerMask.GetMask("ground")))
            {   isRightHit=true;
                Debug.DrawRay(rightrayHit.point,rightrayHit.normal,Color.magenta,Time.deltaTime);
                    rightFootTargetPosition = rightrayHit.point + Vector3.up * rayHitOffset;
                    var rotAxisRight = Vector3.Cross(rightFootLocalUp, rightrayHit.normal);
                    var angleRight = Vector3.Angle(rightFootLocalUp, rightrayHit.normal);
             
                    rightFootTargetRotation = Quaternion.AngleAxis(angleRight, rotAxisRight) * rightFootIKRotation;
            }
            else
            {   isRightHit=false;
                rightFootTargetPosition = rightFootIk;
                rightFootTargetRotation = rightFootIKRotation;
            }
         
            /*var leftFootTargetLocalForward = leftFootTargetRotation* Vector3.forward;
            var rightFootTargetLocalForward = rightFootTargetRotation* Vector3.forward;
            var leftFootTargetLocalUp = leftFootTargetRotation* Vector3.up;
            var rightFootTargetLocalUp = rightFootTargetRotation* Vector3.up;
            CapsuleCollider[] lefttmp=new CapsuleCollider[5];
            CapsuleCollider[] righttmp=new CapsuleCollider[5];
            Debug.DrawRay(sphereColliderleft.transform.position,leftFootTargetLocalUp,Color.blue,Time.deltaTime);
            Debug.DrawLine(sphereColliderleft.transform.position,sphereColliderleft.transform.position+leftFootTargetLocalForward*footCapsuleLenth,Color.blue,Time.deltaTime);
            Debug.DrawLine(sphereColliderleft.transform.position-leftFootTargetLocalUp*0.05f,sphereColliderleft.transform.position-leftFootTargetLocalUp*0.05f+leftFootTargetLocalForward*footCapsuleLenth,Color.blue,Time.deltaTime);
            int hitCountLeft = Physics.OverlapCapsuleNonAlloc(sphereColliderleft.transform.position-leftFootTargetLocalUp*0.07f, sphereColliderleft.transform.position-leftFootTargetLocalUp*0.07f+leftFootTargetLocalForward*footCapsuleLenth,
                0.05f, 
                lefttmp, LayerMask.GetMask("ground"));
            if (hitCountLeft > 0)
            {
                leftFootWeight = 1f;
            }
            else
            {
                leftFootWeight = 0;
            }
            
            int hitCountRight = Physics.OverlapCapsuleNonAlloc(sphereColliderright.transform.position-rightFootTargetLocalUp*0.07f, sphereColliderright.transform.position-rightFootTargetLocalUp*0.07f+rightFootTargetLocalForward*footCapsuleLenth,
                0.05f, 
                righttmp, LayerMask.GetMask("ground"));
            if (hitCountRight > 0)
            {
                rightFootWeight = 1f;
            }
            else
            {
                rightFootWeight = 0;
            }*/
            
        }

        public void SetLeftFootIKOn()
        {
            LeftFootIKOn=true;
        }

        public void SetRightFootIKOn()
        {
            RightFootIKOn=true;
        }

        public void SetLeftFootIKOff()
        {
            LeftFootIKOn = false;
        }

        public void SetRightFootIKOff()
        {   
            RightFootIKOn = false;
        }
        
    }
}