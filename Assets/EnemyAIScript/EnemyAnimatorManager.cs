using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SG
{

    public class EnemyAnimatorManager :AnimatorManager
    {   
        
        public Rigidbody enemyRigidBody;
        private EnemyManager enemyManager;
        
      
        public LayerMask detectionLayer;
        // Start is called before the first frame update    
     
        public GameObject model;
        private void Awake()
        {   enemyRigidBody=GetComponentInParent<Rigidbody>();
            animator=GetComponent<Animator>();
            enemyManager = GetComponentInParent<EnemyManager>();
          

        }

        void Start()
        {   
            
            enemyRigidBody.isKinematic=false;
        
        }
        private void OnAnimatorMove()
        {   
           
            float delta=Time.deltaTime;
            /*Vector3 deltaPosition = Vector3.Project(animator.deltaPosition,model.transform.forward);
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition/Time.deltaTime;
            enemyRigidBody.velocity=new Vector3(velocity.x,enemyRigidBody.velocity.y,velocity.z);*/
            enemyRigidBody.velocity = new Vector3(animator.velocity.x, enemyRigidBody.velocity.y, animator.velocity.z);


        }

        private void Update()
        {
   
   
        }
        private void FixedUpdate()
        {
           
        }

        
       

       
    
        
       
    }

}