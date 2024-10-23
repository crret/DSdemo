using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class AttackState :State
    {   public CombatStanceState combatStanceState;
        public PursueTargetState pursueTargetState;
        public EnemyAttackAction[] enemyAttacks;
        public EnemyAttackAction currentAttack;
        public DeadState deadState;
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {  if (enemyAnimatorManager.animator.GetBool("isDead"))
            {
                return deadState;
            }
            
            enemyAnimatorManager.animator.SetBool("isIdle",false);
            enemyAnimatorManager.animator.SetFloat("right",0,0.1f,Time.deltaTime);
            Vector3 targetDirection=enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            float distanceFromTarget=Vector3.Distance(enemyManager.currentTarget.transform.position,enemyManager.transform.position);
            float viewableAngle=Vector3.Angle(targetDirection,enemyManager.transform.forward);
       
            if (enemyManager.isPerformingAction)
            {
                return combatStanceState;
            }
            
            
            if (currentAttack != null)
            {
                if (distanceFromTarget < currentAttack.minDistanceNeededToAttack)
                {  
                    return this;
                }
                else if (enemyManager.distanceFromTarget < currentAttack.maxDistanceNeededToAttack)
                {
                    if (viewableAngle <= currentAttack.maxAttackAngle&&viewableAngle>=currentAttack.minAttackAngle)
                    {      
                        if (enemyManager.currentRecoveryTime <= 0 && enemyManager.isPerformingAction == false)
                        {   enemyManager.isPerformingAction=true;
                        
                            enemyAnimatorManager.animator.SetFloat("forward", 0, 0.1f, Time.deltaTime);
                           
                            enemyAnimatorManager.PlayAnimation(currentAttack.actionAnimation);
                            enemyManager.currentRecoveryTime=currentAttack.recoveryTime;
                            currentAttack=null;
                            return combatStanceState;
                        }
                    }
                }
               
            }
            else
            {   
               GetNewAttack(enemyManager);
               if (currentAttack == null)
               {   
                   pursueTargetState.HandleMoveToTarget(enemyManager,enemyAnimatorManager,distanceFromTarget);
                   pursueTargetState.HandleRotateTowardsTarget(enemyManager);
                   enemyManager.navMeshAgent.transform.localRotation = Quaternion.identity;
                   enemyManager.navMeshAgent.transform.localPosition=Vector3.zero;
               }
            }

            return this;
        }

        private void GetNewAttack(EnemyManager enemyManager)
        {
            Vector3 targetDirection=enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            float viewableAngle=Vector3.Angle(targetDirection,enemyManager.transform.forward);
            float distanceFromTarget=Vector3.Distance(enemyManager.currentTarget.transform.position,transform.position);

            int maxScore = 0;

            for (int i = 0; i < enemyAttacks.Length; i++)
            {    
                EnemyAttackAction enemyAttackAction = enemyAttacks[i];
                if (distanceFromTarget <= enemyAttackAction.maxDistanceNeededToAttack&&distanceFromTarget>=enemyAttackAction.minDistanceNeededToAttack)
                {   
                    if (viewableAngle <= enemyAttackAction.maxAttackAngle&&viewableAngle>=enemyAttackAction.minAttackAngle)
                    {   
                        maxScore += enemyAttackAction.attackScore;
                    }
                }
            }
            
            int randomValue = Random.Range(0,maxScore);
            int tempScore = 0;

            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                EnemyAttackAction enemyAttackAction = enemyAttacks[i];
                if (distanceFromTarget <= enemyAttackAction.maxDistanceNeededToAttack &&
                    distanceFromTarget >= enemyAttackAction.minDistanceNeededToAttack)
                {
                    if (viewableAngle <= enemyAttackAction.maxAttackAngle&&viewableAngle>=enemyAttackAction.minAttackAngle)
                    {
                        if (currentAttack != null)
                        {
                            return;
                        }

                        tempScore += enemyAttackAction.attackScore;
                        if (tempScore >= randomValue)
                        {
                            currentAttack = enemyAttackAction;
                        }
                    }
                }
            }
        }
    }
}