using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SG
{

    public class PursueTargetState : State
    { 
        public CombatStanceState combatStance;
        public DeadState deadState;
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            if (enemyAnimatorManager.animator.GetBool("isDead"))
            {
                return deadState;
            }
            if (enemyManager.navMeshAgent.enabled == false)
            {   enemyAnimatorManager.animator.SetFloat("forward",0);
                enemyAnimatorManager.animator.SetFloat("right",0);
                return this;
            }
                
            enemyAnimatorManager.animator.SetFloat("right",0,0.1f,Time.deltaTime);
            float distanceFromTarget=Vector3.Distance(enemyManager.currentTarget.transform.position,enemyManager.transform.position);
           
            HandleMoveToTarget(enemyManager,enemyAnimatorManager,distanceFromTarget);
            HandleRotateTowardsTarget(enemyManager);
            enemyManager.navMeshAgent.transform.localRotation = Quaternion.identity;
            enemyManager.navMeshAgent.transform.localPosition=Vector3.zero;

            if (distanceFromTarget <= enemyManager.maxAttackRange)
            {    enemyAnimatorManager.animator.SetBool("isIdle",false);
                return combatStance;
            }
            else
            {
                return this;
            }
        
        }

        public void HandleMoveToTarget(EnemyManager enemyManager,EnemyAnimatorManager enemyAnimatorManager,float distanceFromTarget)
        {   
            enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
            if (enemyManager.navMeshAgent.isStopped==false)
            {   enemyAnimatorManager.animator.SetBool("isIdle",false);
                enemyAnimatorManager.animator.SetFloat("forward",1.5f,0.1f,Time.deltaTime);
            }
            else 
            {   enemyAnimatorManager.animator.SetFloat("forward",0,0.1f,Time.deltaTime);
                enemyAnimatorManager.animator.SetBool("isIdle",true);
                
            }
            
        }
        public void HandleRotateTowardsTarget(EnemyManager enemyManager)
        {   //rotate manually
            if (enemyManager.navMeshAgent.enabled == false)
            {
                return;
            }
            if (enemyManager.isInteracting)
            {
                Vector3 direction=enemyManager.currentTarget.transform.position - transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero)
                {
                    direction = transform.forward;
                }
                
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                //model.transform.rotation = Quaternion.Slerp(model.transform.rotation, targetRotation, rotationSpeed);
            }
            else//rotate with pathfinding(navmesh)
            { 
                // enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation,navMeshAgent.transform.rotation,Time.deltaTime*rotationSpeed);
                enemyManager.navMeshAgent.updateRotation = false; // 禁用 NavMeshAgent 的自动旋转

                Vector3 direction = enemyManager.navMeshAgent.steeringTarget - enemyManager.transform.position;
                direction.y = 0;

                if (direction != Vector3.zero)
                {   
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

                    // 根据角度差距动态调整旋转速度
                    float desiredSpeed = (angleDifference > 30f) ? 360f : 180f; // 大角度时加速旋转，小角度时减速
                    enemyManager.rotationSpeed = Mathf.Lerp(enemyManager.rotationSpeed, desiredSpeed, Time.deltaTime * 5f); // 平滑地调整旋转速度

                    enemyManager.transform.rotation = Quaternion.RotateTowards(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed * Time.deltaTime);
                }
            }
        }
    }
}