using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SG
{


    public class OnGroundSensor : MonoBehaviour
    {
        public GameObject model;
        public CapsuleCollider capcol;
       
        public float offset = 0.05f;
        private Vector3 point1;
        private Vector3 point2;
        public Collider[] outputCols;
        public Collider[] groundCols;
        private float radius;
        
     
   
        [SerializeField] private Animator anim;
        [SerializeField] private PlayerInput playerInput;
        // Start is called before the first frame update
        void Awake()
        {   outputCols=new Collider[10];//缓存数组要分配空间
            groundCols=new Collider[10];
          
            radius = capcol.radius-0.1f;
            anim = model.GetComponent<Animator>();
            playerInput=model.GetComponentInParent<PlayerInput>();

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            point1 = transform.position + transform.up * (radius - offset);
            point2 = transform.position + transform.up * capcol.height - transform.up * (radius - offset);
            
            
            if (Physics.OverlapCapsuleNonAlloc(point1,point2,radius,groundCols,LayerMask.GetMask("ground"))!= 0)
            {
                anim.SetBool("isGround", true);
            }
            else
            {
                anim.SetBool("isGround", false);
            }

            Physics.OverlapCapsuleNonAlloc(point1, point2, radius, outputCols);//检测一些别的碰撞体

        }
 
        
    }
}